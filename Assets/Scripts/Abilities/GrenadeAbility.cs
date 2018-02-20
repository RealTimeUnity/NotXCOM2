using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrenadeAbility : Ability
{
    public override void Execute(Target target)
    {
        base.Execute(target);
        Character destination = target.GetCharacterTarget();

        NavMeshAgent agent = this.owner.GetComponent<NavMeshAgent>();
        agent.SetDestination(destination.transform.position);
    }
}
