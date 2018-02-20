using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamShieldAbility : Ability
{

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Execute(Target target)
    {
        base.Execute(target);
        for (int i = 0; i < owner.owner.friendlies.Count; i++)
        {
            float distance = Mathf.Sqrt(Mathf.Pow((owner.owner.friendlies[i].gameObject.transform.position.x - this.owner.gameObject.transform.position.x), 2) +
                    Mathf.Pow((owner.owner.friendlies[i].gameObject.transform.position.z - this.owner.gameObject.transform.position.z), 2));
            if (distance < 30)
            {
                SelfShieldAbility other = owner.owner.friendlies[i].gameObject.GetComponent<SelfShieldAbility>();
                if (!other) {
                     other = owner.owner.friendlies[i].gameObject.AddComponent<SelfShieldAbility>();
                   
                }
                other.Execute(null);
            }
        }
    }
}
