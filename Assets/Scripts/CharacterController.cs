using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CharacterController : MonoBehaviour
{
    public enum TurnPhase { None, Begin, SelectCharacter, SelectAbility, SelectTarget, Execution, End }

    public List<Character> characterPrefabs;

    [HideInInspector]
    public List<Character> friendlies { get; set; }
    [HideInInspector]
    public CharacterController enemy;

    [HideInInspector]
    public bool updating = true;

    [HideInInspector]
    public TurnPhase phase;
    [HideInInspector]
    public int subjectIndex;

    protected string abilityName;
    [HideInInspector]
    public Target target;
    protected bool abilityCanceled;
    protected bool abilityConfirmed;
    protected bool abilityExecuted;

    public void Start()
    {
        this.friendlies = new List<Character>();
        Initialize();
    }

    public void Initialize()
    {
        this.friendlies = new List<Character>();
        this.enemy = null;
        this.phase = TurnPhase.None;
        this.subjectIndex = -1;
        this.abilityName = null;
        this.target = null;
        this.abilityCanceled = false;
        this.abilityConfirmed = false;
        this.abilityExecuted = false;
    }

    public void FocusAverageLocation()
    {
        Vector3 averagePosition = Vector3.zero;
        for (int i = 0; i < this.friendlies.Count; ++i)
        {
            averagePosition += this.friendlies[i].transform.position;
        }
        averagePosition /= this.friendlies.Count;

        FindObjectOfType<CameraController>().FocusLocation(averagePosition);
    }

    public void StartTurn()
    {
        this.phase = TurnPhase.Begin;
        FocusAverageLocation();
    }
    
    public void CreateFriendlyCharacters(SpawnPoint spawnPoint)
    {
        for (int i = 0; i < this.characterPrefabs.Count; ++i)
        {
            Character newCharacter = Instantiate(characterPrefabs[i], spawnPoint.transform.position, spawnPoint.transform.rotation).GetComponent<Character>();
            float randomX = Random.Range(-5, 5);
            float randomY = Random.Range(-5, 5);
            newCharacter.transform.Translate(new Vector3(randomX, randomY, 0));
            newCharacter.owner = this;
            this.friendlies.Add(newCharacter);
        }
    }

    public void SetEnemy(CharacterController enemyController)
    {
        this.enemy = enemyController;
    }

    public void FixedUpdate()
    {
        if (updating)
        {
            this.UpdateTurn();
        }
    }

    public IEnumerator FinishAbility(int time)
    {
        yield return new WaitForSeconds(time);

        this.friendlies[this.subjectIndex].GetAbility(abilityName).DoneWaiting();

        this.abilityExecuted = false;

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
    }

    protected void UpdateTurn()
    {
        // Turn Progression
        switch (this.phase)
        {
            case TurnPhase.Begin:
                for (int i = 0; i < this.friendlies.Count; ++i)
                {
                    this.friendlies[i].ResetTurn();
                }

                this.phase = TurnPhase.SelectCharacter;
                break;
            case TurnPhase.SelectCharacter:
                int newSubjectIndex = this.GetSubjectIndex();

                if (newSubjectIndex >= 0 && newSubjectIndex < this.friendlies.Count)
                {
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
                if (!this.abilityExecuted)
                {
                    this.abilityExecuted = true;

                    this.friendlies[this.subjectIndex].ExecuteAbility(this.abilityName, this.target);
                }
                break;
            case TurnPhase.End:
                FindObjectOfType<GameManager>().FinishTurn();
                this.subjectIndex = -1;
                this.phase = TurnPhase.None;
                break;
            case TurnPhase.None:
                break;
        }

        // Remove and Destroy characters that are dead
        List<int> charactersToRemove = new List<int>();
        for (int i = 0; i < this.friendlies.Count; ++i)
        {
            if (this.friendlies[i].currentHealth <= 0 && this.friendlies[i].initialized)
            {
                charactersToRemove.Add(i);
            }
        }
        for (int i = 0; i < charactersToRemove.Count; ++i)
        {
            if (charactersToRemove[i] < this.friendlies.Count)
            {
                this.friendlies[charactersToRemove[i]].Die();
                this.friendlies.RemoveAt(charactersToRemove[i]);
            }
        }

        // Check win condition
        if (this.enemy != null)
        {
            if (this.enemy.friendlies.Count <= 0 && this.friendlies.Count > 0)
            {
                StartCoroutine(FindObjectOfType<GameManager>().EndGame(this));
            }
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
