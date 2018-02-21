using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamShieldAbility : Ability
{
    public GameObject selfShieldAbilityPrefab;

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
                if (owner.owner.friendlies[i].HasAbility("Self Shield")) {
                    ((SelfShieldAbility)owner.owner.friendlies[i].GetAbility("Self Shield")).AddShield();
                }
                else
                {
                    SelfShieldAbility a = Instantiate(selfShieldAbilityPrefab).GetComponent<SelfShieldAbility>();
                    a.Initialize(owner.owner.friendlies[i]);
                    a.CreateShieldInstance();
                    owner.owner.friendlies[i].passiveAbilities.Add(a);
                    a.AddShield();
                }
            }
        }
    }
}
