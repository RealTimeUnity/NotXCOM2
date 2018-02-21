using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CharacterController : MonoBehaviour
{
    public enum TurnPhase { None, Begin, SelectCharacter, SelectAbility, SelectTarget, Execution, End }

    public GameObject characterPrefab;
    public List<string> names;

    [HideInInspector]
    public List<Character> friendlies { get; set; }
    protected List<Character> enemies;

    [HideInInspector]
    public TurnPhase phase;
    [HideInInspector]
    public int subjectIndex;

    protected string abilityName;
    [HideInInspector]
    public Target target;
    protected bool abilityCanceled;
    protected bool abilityConfirmed;

    public void Start()
    {  
        this.friendlies = new List<Character>();
        this.enemies = new List<Character>();
        this.phase = TurnPhase.None;
        this.subjectIndex = -1;
        this.abilityName = null;
        this.target = null;
        this.abilityCanceled = false;
        this.abilityConfirmed = false;
    }

    public void StartTurn()
    {
        this.phase = TurnPhase.Begin;
    }

    protected void EndTurn()
    {
        this.subjectIndex = -1;

        for (int i = 0; i < this.friendlies.Count; ++i)
        {
            this.friendlies[i].hasHadTurn = false;
            this.friendlies[i].ResetTurn();
        }

        FindObjectOfType<GameManager>().FinishTurn();
    }

    protected abstract void EndChildGame();

    public void CreateFriendlyCharacters(SpawnPoint spawnPoint)
    {
        this.friendlies = new List<Character>();
        for (int i = 0; i < 1; ++i)
        {
            GameObject newCharacter = Instantiate(characterPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            float randomX = Random.Range(-5, 5);
            float randomY = Random.Range(-5, 5);
            newCharacter.transform.Translate(new Vector3(randomX, randomY, 0));
            newCharacter.GetComponent<Character>().owner = this;
            newCharacter.GetComponent<Character>().Name = names[i];
            this.friendlies.Add(newCharacter.GetComponent<Character>());
        }
    }

    public void SetEnemy(CharacterController enemyController)
    {
        for (int i = 0; i < enemyController.friendlies.Count; ++i)
        {
            this.enemies.Add(enemyController.friendlies[i]);
        }
    }

    public void Update()
    {
        this.UpdateTurn();
    }

    protected void UpdateTurn()
    {
        List<int> charactersToRemove = new List<int>();
        for (int i = 0; i < this.enemies.Count; ++i)
        {
            if (this.enemies[i].currentHealth <= 0)
            {
                charactersToRemove.Add(i);
            }
        }
        for (int i = 0; i < charactersToRemove.Count; ++i)
        {
            Character c = this.enemies[charactersToRemove[i]];
            this.enemies.RemoveAt(charactersToRemove[i]); try
            {
                c.Die();
            }
            catch (MissingReferenceException e)
            {
                int x = 2;
            }
        }

        charactersToRemove.Clear();
        for (int i = 0; i < this.friendlies.Count; ++i)
        {
            try
            {
                GameObject go = this.friendlies[i].gameObject;
            }
            catch (MissingReferenceException e)
            {
                charactersToRemove.Add(i);
            }
        }
        for (int i = 0; i < charactersToRemove.Count; ++i)
        {
            if (charactersToRemove[i] < enemies.Count)
            {
                this.enemies.RemoveAt(charactersToRemove[i]);
            }
        }

        switch (this.phase)
        {
            case TurnPhase.Begin:
                this.phase = TurnPhase.SelectCharacter;
                break;
            case TurnPhase.SelectCharacter:
                int newSubjectIndex = this.GetSubjectIndex();

                if (newSubjectIndex >= 0 && newSubjectIndex < this.friendlies.Count)
                {
                    this.friendlies[newSubjectIndex].ResetTurn();
                    this.subjectIndex = newSubjectIndex;
                    this.phase = TurnPhase.SelectAbility;
                }

                bool allTurnsDone = true;
                for (int i = 0; i < this.friendlies.Count; ++i)
                {
                    if (!this.friendlies[i].hasHadTurn)
                    {
                        allTurnsDone = false;
                        break;
                    }
                }

                if (allTurnsDone || this.subjectIndex >= this.friendlies.Count)
                {
                    this.phase = TurnPhase.End;
                }
                break;
            case TurnPhase.SelectAbility:
                string abilityName = GetAbilityName();
                if (abilityName != null && this.friendlies[this.subjectIndex].IsAbilityExecutable(abilityName))
                {
                    this.abilityName = abilityName;
                    this.phase = TurnPhase.SelectTarget;
                }
                break;
            case TurnPhase.SelectTarget:
                if (this.abilityCanceled)
                {
                    this.abilityName = null;
                    this.target = null;
                    this.abilityCanceled = false;
                    this.abilityConfirmed = false;
                    this.phase = TurnPhase.SelectAbility;
                }
                else if (this.abilityConfirmed)
                {
                    if (this.target != null)
                    {
                        this.phase = TurnPhase.Execution;
                    }
                    else
                    {
                        this.abilityConfirmed = false;
                    }
                }
                else
                {
                    Target target = GetTargetSelection();
                    if (target != null && this.friendlies[this.subjectIndex].GetAbility(this.abilityName).IsTargetInRange(this.friendlies[this.subjectIndex], target))
                    {
                        this.target = target;
                    }
                }
                break;
            case TurnPhase.Execution:
                this.friendlies[this.subjectIndex].ExecuteAbility(this.abilityName, this.target);

                if (this.friendlies[this.subjectIndex].HasMoreAbilities())
                {
                    this.phase = TurnPhase.SelectAbility;
                }
                else
                {
                    this.phase = TurnPhase.SelectCharacter;
                }

                this.friendlies[this.subjectIndex].hasHadTurn = true;
                this.abilityName = null;
                this.target = null;
                this.abilityCanceled = false;
                this.abilityConfirmed = false;
                break;
            case TurnPhase.End:
                this.EndTurn();
                this.phase = TurnPhase.None;
                break;
        }

        if (this.enemies.Count <= 0 && this.friendlies.Count > 0)
        {
            EndChildGame();
            StartCoroutine(FindObjectOfType<GameManager>().EndGame(this));
        }
    }

    protected Target GetTargetSelection()
    {
        Target target = new Target();
        target.SetTargetType(this.friendlies[this.subjectIndex].GetAbility(this.abilityName).targetType);

        Character character = null;
        Vector3 location = Vector3.zero;
        switch (target.GetTargetType())
        {
            case Target.TargetType.Self:
                if (this.friendlies[this.subjectIndex] != null)
                {
                    target.SetCharacterTarget(this.friendlies[this.subjectIndex]);
                    return target;
                }
                break;
            case Target.TargetType.Friendly:
                character = GetFriendlySelection();
                if (character != null)
                {
                    target.SetCharacterTarget(character);
                    return target;
                }
                break;
            case Target.TargetType.Enemy:
                character = GetEnemySelection();
                if (character != null)
                {
                    target.SetCharacterTarget(character);
                    return target;
                }
                break;
            case Target.TargetType.Location:
                location = GetLocationSelection();
                if (location != Vector3.zero)
                {
                    target.SetLocationTarget(location);
                    return target;
                }
                break;
        }

        return null;
    }

    public void CancelAbility()
    {
        this.abilityCanceled = true;
    }

    public void ConfirmAbility()
    {
        this.abilityConfirmed = true;
    }

    protected abstract int GetSubjectIndex();

    protected abstract string GetAbilityName();

    protected abstract Vector3 GetLocationSelection();

    protected abstract Character GetEnemySelection();

    protected abstract Character GetFriendlySelection();
}
