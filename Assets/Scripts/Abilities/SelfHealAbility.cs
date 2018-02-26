using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SelfHealAbility : Ability
{
    public float healAmount=0.75F;
    public GameObject healParticleEffect;
    protected GameObject characterGameObject;

    public override void Execute(Target target)
    {
        base.Execute(target);

        // Get character's gameobject to get position information
        characterGameObject = this.owner.gameObject;
      
        // add heal amount to the player health
        this.owner.currentHealth += healAmount * this.owner.MaxHealth;
        if (this.owner.currentHealth > this.owner.MaxHealth)
        {
            this.owner.currentHealth = this.owner.MaxHealth;
        }
        // create the heal particles
        GameObject healParticles = Instantiate(healParticleEffect, characterGameObject.transform.position + new Vector3(0, 2.5f, 0), Quaternion.Euler(-90, 0, 0));
        // set the heal particles to be destroyed in 4 seconds
        Destroy(healParticles, 4.0F);
        this.isDone = true;
    }
}