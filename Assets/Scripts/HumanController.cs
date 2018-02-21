using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class HumanController : CharacterController
{
    public GameObject locationIndicator;
    public GameObject rangeIndicator;
    public GameObject characterIndicator;

    protected bool selectUIUpdated = false;
    protected int selectedSubjectIndex = -1;
    protected string selectedAbilityName = null;

    public GameObject combatUI;
    public GameObject characterSelectUI;

    public void SelectAbility(string name)
    {
        if (this.friendlies[this.subjectIndex].HasAbility(name))
        {
            this.selectedAbilityName = name;
        }
    }

    public void SelectSubject(int index)
    {
        if (index < this.friendlies.Count && index >= 0)
        {
            this.selectedSubjectIndex = index;
        }
    }

    protected override string GetAbilityName()
    {
        string result = null;
        if (this.selectedAbilityName != null)
        {
            result = this.selectedAbilityName;
            this.selectedAbilityName = null;
        }

        return result;
    }

    protected override int GetSubjectIndex()
    {
        int result = -1;
        if (this.selectedSubjectIndex != -1)
        {
            result = this.selectedSubjectIndex;
            this.selectedSubjectIndex = -1;
        }

        return result;
    }

    protected override Vector3 GetLocationSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return Vector3.zero;
            }
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                NavMeshHit nhit;
                if (NavMesh.SamplePosition(hit.point, out nhit, 10.0f, NavMesh.AllAreas))
                {
                    return nhit.position;
                }
            }
        }

        return Vector3.zero;
    }

    protected override Character GetEnemySelection()
    {
        Character character = GetCharacterClickedOn();
        if (this.enemy.friendlies.Contains(character))
        {
            return character;
        }

        return null;
    }
    
    protected override Character GetFriendlySelection()
    {
        Character character = GetCharacterClickedOn();
        if (this.friendlies.Contains(character))
        {
            return character;
        }

        return null;
    }
    
    protected Character GetCharacterClickedOn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Character selectedChar = hit.collider.GetComponent<Character>();
                if (selectedChar != null)
                {
                    return selectedChar;
                }
            }
        }

        return null;
    }

    public void EndTurn()
    {
        for (int i = 0; i < this.friendlies.Count; ++i)
        {
            this.friendlies[i].hasHadTurn = true;
        }
    }

    new public void Update()
    {
        if (updating)
        {
            this.UpdateTurn();
            this.UpdateVisuals();
        }
    }

    protected void UpdateVisuals()
    {
        switch (this.phase)
        {
            case TurnPhase.SelectCharacter:
                if (!selectUIUpdated && this.friendlies.Count > 0)
                {
                    characterSelectUI.GetComponentInParent<CharacterSelectUI>().UpdateList(this);
                    combatUI.GetComponentInParent<ButtonScript>().Initialize(this);
                    combatUI.GetComponentInParent<ButtonScript>().updateInfo();
                    this.selectUIUpdated = true;
                }

                combatUI.SetActive(false);
                characterSelectUI.SetActive(true);
                break;
            case TurnPhase.SelectAbility:
                combatUI.GetComponentInParent<ButtonScript>().updateInfo();
                characterSelectUI.SetActive(false);
                combatUI.SetActive(true);
                this.selectUIUpdated = false;
                break;
            case TurnPhase.End:
                characterSelectUI.SetActive(false);
                combatUI.SetActive(false);
                this.selectUIUpdated = false;

                MeshRenderer mr;
                for (int i = 0; i < this.friendlies.Count; ++i)
                {
                    mr = this.friendlies[i].GetComponent<MeshRenderer>();
                    if (mr == null)
                    {
                        this.friendlies[i].GetComponentInChildren<SkinnedMeshRenderer>().material.SetInt("_Highlighted", 0);
                    }
                    else
                    {
                        mr.material.SetInt("_Highlighted", 0);
                    }
                }
                break;
        }
        
        // Range Indicator
        if (this.abilityName != null)
        {
            NavMeshHit nhit;
            NavMesh.SamplePosition(this.friendlies[this.subjectIndex].transform.position, out nhit, 1000.0f, NavMesh.AllAreas);

            this.rangeIndicator.transform.position = nhit.position;
            this.rangeIndicator.GetComponent<RangeIndicator>().Initialize(this.friendlies[this.subjectIndex].GetAbility(this.abilityName).range);
        }
        else
        {
            this.rangeIndicator.SetActive(false);
        }

        // Target Indicator
        if (this.target != null)
        {
            if (this.target.GetTargetType() == Target.TargetType.Location)
            {
                this.locationIndicator.transform.position = this.target.GetLocationTarget();
                this.locationIndicator.SetActive(true);
                this.characterIndicator.SetActive(false);
            }
            else if (this.target.GetTargetType() != Target.TargetType.None)
            {
                this.characterIndicator.transform.position = this.target.GetCharacterTarget().transform.position;
                this.characterIndicator.SetActive(true);
                this.locationIndicator.SetActive(false);
            }
        }
        else
        {
            this.locationIndicator.SetActive(false);
            this.characterIndicator.SetActive(false);
        }
    }
}
