using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrenadeAbility : Ability
{
    public GameObject GrenadePrefab, ExplosionPrefab;
    public float TimeToAsplode = 6.0f;
    public float AsplodingHurtDistance = 13.0f;
    public float NadeDamage = 125.0f;
    protected GameObject characterGameObject;
    protected GameObject dat_nade;
    protected bool GrenadePrimed;

    public override void Execute(Target target)
    {
        base.Execute(target);
        Vector3 destination = target.GetLocationTarget();
        characterGameObject = this.owner.gameObject;

        dat_nade = Instantiate(GrenadePrefab, characterGameObject.transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity);

        Rigidbody GrenadeRB = dat_nade.GetComponent<Rigidbody>();

        Vector3 ThrowVector = destination - characterGameObject.transform.position;

        ThrowVector = ThrowVector * 18.0f;

        GrenadeRB.AddForce(ThrowVector);

        for (int i = 0; i < owner.owner.friendlies.Count; i++)
        {
            float distanceToNade = Vector3.Distance(owner.owner.friendlies[i].transform.position, dat_nade.transform.position);
            if (distanceToNade < AsplodingHurtDistance)
            {
                owner.owner.friendlies[i].TakeDamage((int)(0.6 * NadeDamage * (1 / distanceToNade)));
            }
        }
        for (int i = 0; i < owner.owner.enemies.Count; i++)
        {
            float distanceToNade = Vector3.Distance(owner.owner.enemies[i].transform.position, dat_nade.transform.position);
            if (distanceToNade < AsplodingHurtDistance)
            {
                owner.owner.enemies[i].TakeDamage((int)(NadeDamage * (1 / distanceToNade)));
            }
        }
        // create the explosion
        GameObject my_nade_asplode = Instantiate(ExplosionPrefab, dat_nade.transform.position, Quaternion.identity);
        // set the heal particles to be destroyed in 4 seconds
        Destroy(dat_nade, 0.01F);
        Destroy(my_nade_asplode, 5.0F);

    }


    IEnumerator NadeAsplode(float explosion_time)
    {
        //if grenade
        yield return new WaitForSeconds(explosion_time - 0.1f);

    }
}
