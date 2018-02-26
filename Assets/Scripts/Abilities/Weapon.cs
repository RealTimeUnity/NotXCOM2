using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Ability {

    public int Damage;

    protected MeshRenderer mesh;

    void Start()//Initializes stats
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;
    }

	public int Aim(Target target)
    {
        int accuracy = 0;
		if(IsTargetInRange(owner, target))
		{
            accuracy = GameObject.Find("CoverCheckCam").GetComponent<CoverCheck>().getShoot(target.GetCharacterTarget().gameObject, this.gameObject);
        }
		else {
            accuracy = 0;
        }
        accuracy = accuracy < 20 ? 0 : accuracy;
		return accuracy;
	}

    IEnumerator Fire(Target target)
    {
        // rotate
        Vector3 targetPoint = new Vector3(target.GetCharacterTarget().transform.position.x, this.owner.transform.position.y,
                   target.GetCharacterTarget().transform.position.z) - this.owner.transform.position;
        this.owner.transform.rotation = Quaternion.LookRotation(targetPoint, Vector3.up);
        this.mesh.enabled = true;

        // move cam and wait
        FindObjectOfType<CameraController>().FocusLocation(owner.transform.position);
        yield return new WaitForSeconds(1);

        // start anim and wait
        GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1);

        // move cam to enemy and damage enemy
        FindObjectOfType<CameraController>().FocusLocation(target.GetCharacterTarget().transform.position);
        
        int dam = 0;
        int accuracy = Aim(target);
        System.Random rand = new System.Random();
        int num = rand.Next(0, 100);
        if (num < accuracy)
        {
            dam = Damage;
        }
        if (target.GetTargetType().Equals(Target.TargetType.Enemy))
        {
            target.GetCharacterTarget().TakeDamage(dam);
        }
        this.isDone = true;

        // wait and re focus owner
        yield return new WaitForSeconds(2);
        FindObjectOfType<CameraController>().FocusLocation(owner.transform.position);
    }

    public override void DoneCallback()
    {
        if (isDone)
        {
            StartCoroutine(owner.owner.FinishAbility(this.waitSecondsAfterDone));
            isDone = false;
            this.mesh.enabled = false;
        }
    }

    // Attack Function
    public override void Execute(Target target)
    {
        StartCoroutine(Fire(target));
    }
}
