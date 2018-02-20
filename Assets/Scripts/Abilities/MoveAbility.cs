using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAbility : Ability
{
    public override void Execute(Target target)
    {
        base.Execute(target);
        Vector3 destination = target.GetLocationTarget();
        
        NavMeshAgent agent = this.owner.GetComponent<NavMeshAgent>();
        agent.SetDestination(destination);
    }
}
