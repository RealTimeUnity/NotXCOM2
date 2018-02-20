using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AreaTeamHealAbility : Ability
{
    public float healAmount = 0.35F;
    public GameObject healParticleEffect;
    public float euler_x, euler_y, euler_z;
    protected GameObject characterGameObject;

    public override void Execute(Target target)
    {
        base.Execute(target);
        Character destination = target.GetCharacterTarget();

        NavMeshAgent agent = this.owner.GetComponent<NavMeshAgent>();
        agent.SetDestination(destination.transform.position);

        for (int i = 0; i < owner.owner.friendlies.Count; i++)
        {
            // Use Evan's function to get players within range
            float distance = Mathf.Sqrt(Mathf.Pow((owner.owner.friendlies[i].gameObject.transform.position.x - this.owner.gameObject.transform.position.x), 2) +
                    Mathf.Pow((owner.owner.friendlies[i].gameObject.transform.position.z - this.owner.gameObject.transform.position.z), 2));

            // check to see if they're in the specified range
            if (distance <= (float)range)
            {
                // add heal amount to the player health
                owner.owner.friendlies[i].currentHealth += healAmount;
                if (owner.owner.friendlies[i].currentHealth > owner.owner.friendlies[i].MaxHealth)
                {
                    owner.owner.friendlies[i].currentHealth = owner.owner.friendlies[i].MaxHealth;
                }
                // Get character's gameobject to get position information
                characterGameObject = owner.owner.friendlies[i].gameObject;
                // create the heal particles
                GameObject healParticles = Instantiate(healParticleEffect, characterGameObject.transform.position + new Vector3(0,2.5f,0), Quaternion.Euler(-90, 0, 0));
                // set the heal particles to be destroyed in 4 seconds
                Destroy(healParticles, 4.0F);
            }
        }
    }
}
