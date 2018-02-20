using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Ability {

    public int Damage;

    void Start()//Initializes stats
    { }

	public int Aim(Target target)
    {
        int accuracy = 0;
		if(!IsTargetInRange(owner, target))
		{
            float distance = Vector3.Distance(owner.transform.position, target.GetCharacterTarget().transform.position);
            if (distance < (2 * range))
			{
				accuracy = (int)(((distance % range) / range) * 100);
			}
		}
		else {
			accuracy = 100;
		}
		return accuracy;
	}

    // Attack Function
    public override void Execute(Target target)
    {
		int dam = 0;
        int accuracy = Aim(target);
		System.Random rand = new System.Random ();
		int num = rand.Next (0, 100);
		if(num < accuracy){
            dam = Damage;
        }
        if (target.GetTargetType().Equals(Target.TargetType.Enemy))
        {
            target.GetCharacterTarget().TakeDamage(dam);
        }
        var exp = GetComponent<ParticleSystem>();
        exp.Play();
    }
}
