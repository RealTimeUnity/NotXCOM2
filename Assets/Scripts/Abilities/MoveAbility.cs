using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAbility : Ability
{
    protected Vector3 destination = Vector3.zero;

    public override void Execute(Target target)
    {
        base.Execute(target);
        destination = target.GetLocationTarget();
        
        NavMeshAgent agent = this.owner.GetComponent<NavMeshAgent>();
        agent.SetDestination(destination);
    }

    public void Update()
    {
        if (destination != Vector3.zero)
        {
            FindObjectOfType<CameraController>().FocusLocation(owner.transform.position);
            if (Vector3.Distance(destination, this.owner.gameObject.transform.position) < 2)
            {
                this.isDone = true;
                this.destination = Vector3.zero;
            }
        }
    }
}
