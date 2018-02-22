using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class Ability : MonoBehaviour
{
    public enum AbilityType { None, Major, Minor, Passive }
    public enum UseType { NumberPerTurn, AbsoluteNumber }

    public Sprite sprite;
    public Target.TargetType targetType;
    public AbilityType type = AbilityType.None;

    public UseType useType = UseType.NumberPerTurn;
    public int range;
    public int uses;
    public string abilityName;
    public int waitSecondsAfterDone;

    [HideInInspector]
    public Character owner;
    protected int maxUses;
    
    protected bool isDone;

    public AbilityType GetAbilityType()
    {
        return this.type;
    }

    public void Initialize(Character owner)
    {
        this.isDone = false;
        this.owner = owner;
        this.maxUses = uses;
    }

    public void ResetCount()
    {
        if (this.useType == UseType.NumberPerTurn)
        {
            this.uses = maxUses;
        }
    }

    public virtual void DoneCallback()
    {
        if (isDone)
        {
            StartCoroutine(owner.owner.FinishAbility(this.waitSecondsAfterDone));
            isDone = false;
            FindObjectOfType<CameraController>().FocusLocation(owner.transform.position);
        }
    }

    public virtual void DoneWaiting()
    {

    }

    public void FixedUpdate()
    {
        DoneCallback();
    }

    public bool IsTargetInRange(Character startingPoint, Target target)
    {
        Vector3 destination = Vector3.zero;
        Vector3 start = startingPoint.transform.position;
        switch (this.targetType)
        {
            case Target.TargetType.Location:
                destination = target.GetLocationTarget();
                break;
            case Target.TargetType.Enemy:
            case Target.TargetType.Friendly:
                destination = target.GetCharacterTarget().transform.position;
                break;
            case Target.TargetType.Self:
                destination = startingPoint.transform.position;
                break;
        }

        start.y = destination.y;
        float distance = (start - destination).magnitude;
        return distance <= this.range;
    }

    public virtual void Execute(Target target)
    {
        this.isDone = false;
        --this.uses;
    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }
}