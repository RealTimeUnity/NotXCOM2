using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfShieldAbility : Ability {
    public bool ShieldActive=false;
    public float MaxShieldHealth = 100;
    public float CurrentShieldHealth = 0;

    public GameObject shieldPrefab;
    [HideInInspector]
    public GameObject shieldInstance = null;

    public override void Execute(Target target)
    {
        base.Execute(target);
        CurrentShieldHealth = 100;
        shieldInstance.SetActive(true);
    }

    public void AddShield()
    {
        CurrentShieldHealth += 100;
        shieldInstance.SetActive(true);
    }

    public void Update()
    {
        if (owner != null && shieldInstance == null)
        {
            shieldInstance = Instantiate(shieldPrefab, owner.transform);
            shieldInstance.SetActive(true);
        }

        //check if enemy turn over then set CurrentShieldHealth to 0
        if (owner.owner.phase == CharacterController.TurnPhase.Begin)
        {
            CurrentShieldHealth = 0;
        }

        if (CurrentShieldHealth == 0)
        {
            shieldInstance.SetActive(false);
            ShieldActive = false;
        }
    }

}
