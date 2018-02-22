using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiController : CharacterController
{
    List<Ability> abilities;
    List<Target> targets=new List<Target>();
    List<int> scores=new List<int>();
    int scoreInteger;
    protected override string GetAbilityName()
    {
        Character actor=this.friendlies[this.subjectIndex];
        abilities = actor.abilities;
        for(int i = 0; i < abilities.Count; i++)
        {
            magicalAbilityScoreGeneration(abilities[i], actor);
        }
        int currentScore = 0;
        for(int i = 0; i < scores.Count; i++)
        {
            if (currentScore < scores[i])
            {
                currentScore = scores[i];
                scoreInteger = i;
            }
        }

        return abilities[scoreInteger].abilityName;
    }

    protected override int GetSubjectIndex()
    {
        return this.subjectIndex + 1;
    }

    protected override Vector3 GetLocationSelection()
    {
        if (targets[scoreInteger].GetTargetType() == Target.TargetType.Location)
        {
            this.ConfirmAbility();
            return (targets[scoreInteger].GetLocationTarget());
        }
        else
        {
    
            return Vector3.zero;

        }
    }

    protected override Character GetEnemySelection()
    {
        if (targets[scoreInteger].GetTargetType()==Target.TargetType.Enemy)
        {
            return (targets[scoreInteger].GetCharacterTarget());
        }
        else
        {
            return null;
        }
    }

    protected override Character GetFriendlySelection()
    {
        if (targets[scoreInteger].GetTargetType()==Target.TargetType.Friendly)
        {
            return (targets[scoreInteger].GetCharacterTarget());
        }
        else {
            return null;
        }
    }
    protected void magicalAbilityScoreGeneration(Ability ability,Character actor)
    {
        int abilityIndex = abilities.IndexOf(ability);
        string abilityName = ability.abilityName;
        if (abilityName == "Move")
        {
            int distance = 100000;
            Vector3 dir = Vector3.down;
            Vector3 temp = actor.GetComponent<Transform>().position;
            for (int i = 0; i < enemy.friendlies.Count; i++)
            {
                int tempRange = (int)Vector3.Distance(temp, enemy.friendlies[i].GetComponent<Transform>().position);
                if (tempRange < distance)
                {
                    distance = tempRange;
                    dir = enemy.friendlies[i].GetComponent<Transform>().position-temp;

                }
            }
            targets.Add(new Target());
            targets[abilityIndex].SetTargetType(Target.TargetType.Location);

            if (dir.magnitude > ability.range)
            {
                dir = actor.transform.position + (dir.normalized * ability.range);
            }
            else
            {
                dir = actor.transform.position + dir;
            }
            targets[abilityIndex].SetLocationTarget(dir);
            scores.Add(20);
        }
        if (abilityName == "Team Heal")
        {
            float healValue=0;
            float healCount=0;
            for (int i = 0; i < friendlies.Count; i++)
            {
                // Use Evan's function to get players within range
                float distance = Mathf.Sqrt(Mathf.Pow((friendlies[i].transform.position.x - actor.transform.position.x), 2) +
                        Mathf.Pow((friendlies[i].transform.position.z - actor.transform.position.z), 2));

                // check to see if they're in the specified range
                if (distance <= (float)actor.GetAbility(abilityName).range)
                {
                    // add heal amount to the player health
                    healValue += (friendlies[i].MaxHealth - friendlies[i].currentHealth);
                    healCount += 1;
                }
               
            }

            healCount = healCount * (9 / 10);
            healValue = (healValue / healCount);
            targets[abilityIndex].SetCharacterTarget(actor);
            scores.Add((int)healValue);
        }
        if (abilityName == "Team Shield")
        {

            float shieldValue = 0;
            for (int i = 0; i < friendlies.Count; i++)
            {
                // Use Evan's function to get players within range
                float distance = Mathf.Sqrt(Mathf.Pow((friendlies[i].transform.position.x - actor.transform.position.x), 2) +
                        Mathf.Pow((friendlies[i].transform.position.z - actor.transform.position.z), 2));

                // check to see if they're in the specified range
                if (distance <= (float)actor.GetAbility(abilityName).range)
                {
                    // add heal amount to the player health
                    shieldValue += 10;
                    shieldValue *=.9f;
                }

            }
            targets[abilityIndex].SetCharacterTarget(actor);
            scores.Add((int)shieldValue);

        }
        if (abilityName == "Self Shield")
        {
            int shieldScore = 0;
            if (((SelfShieldAbility)actor.GetAbility(abilityName)).ShieldActive)
            {
                shieldScore += 10;
            }

            targets[abilityIndex].SetCharacterTarget(actor);
            scores.Add((int)shieldScore);
        }
        if (abilityName == "Self Heal")
        {
            int healValue = 0;
            if (actor.MaxHealth - actor.currentHealth < ((SelfHealAbility)actor.GetAbility(abilityName)).healAmount)
            {
                healValue = 40;
                if (actor.currentHealth < actor.MaxHealth / 2)
                {
                    healValue += 20;
                }
            }
            targets[abilityIndex].SetCharacterTarget(actor);
            scores.Add(healValue);
        }
        if (abilityName == "Flame thrower" || abilityName == "Pistol" || abilityName == "Rifle" || abilityName == "Shotgun" || abilityName == "Sniper")
        {
            int maxValue = 0;
            Character primaryVictim=new Character();
            for (int i = 0; i < enemy.friendlies.Count; i++)
            {
                Character victim = enemy.friendlies[i];
                Target tempTarget=new Target();
                tempTarget.SetTargetType(Target.TargetType.Enemy);
                tempTarget.SetCharacterTarget(victim);
                int hurtValue = 0;
                int accuracy = ((Weapon)actor.GetAbility(abilityName)).Aim(tempTarget);
                int damage = ((Weapon)actor.GetAbility(abilityName)).Damage;
                hurtValue = (accuracy / 100) * damage;
                if (damage > victim.currentHealth)
                {
                    hurtValue = hurtValue * 2 + 10;
                }
                if (hurtValue > maxValue)
                {
                    maxValue = hurtValue;
                    primaryVictim = victim;
                }
            }
            targets[abilityIndex].SetCharacterTarget(primaryVictim);
            scores.Add(maxValue);

        }
    }
    protected int magicalHurtFormula(Character actor, Character victim)
    {
        /*magical formula used to determine the value of shooting at each target*/
        /*
        int hurtValue = 0;
        //int accuracy=actor.primary_weapon.Target(victim);
        int accuracy = 20;
        int damage = actor.primary_weapon.Get_Damage();
        hurtValue = accuracy * damage;
        if (damage > victim.Currenthealth)
        {
            hurtValue=hurtValue*2+10;
        }
        return (hurtValue);
        */
        return 0;
    }
    /* protected Character selectVictim(Character actor)
     {
         /*this code will select the best target for each enemy to shoot at
          * If no target is worth shooting at, it will move to the closest enemy
          * defaults to enemy[0] for convenience

    Character tempVictim =enemies[0];
        int hurtocity = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            int tempVal = magicalHurtFormula(actor, enemies[i]);
            if (tempVal > hurtocity)
            {
                hurtocity = tempVal;
                tempVictim = enemies[i];
            }
        }
        if (hurtocity < actor.primary_weapon.Get_Damage() * .20)
        {
            /*movement target selection
            //int speedLimit = (int)actor.move_distance_max;
            int distance = 100000;
            Vector3 dir = Vector3.down;
            Vector3 temp = actor.GetComponent<Transform>().position;
            for (int i = 0; i < enemies.Count; i++)
            {
                int tempRange = (int)Vector3.Distance(temp, enemies[i].GetComponent<Transform>().position);
                if (tempRange < distance)
                {
                    distance = tempRange;
                    dir = enemies[i].GetComponent<Transform>().position;
                    tempVictim = enemies[i];
                }
            }
        }
        return (tempVictim);

    }
    protected bool attackingOrMoving(Character actor)
    {
        /*true is attack, false is moving
        int hurtocity = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            int tempVal = magicalHurtFormula(actor, enemies[i]);
            if (tempVal > hurtocity)
            {
                hurtocity = tempVal;
            }
        }
        if (hurtocity < actor.primary_weapon.Get_Damage() * .20)
        {
            return (true);
        }
        else
        {
            return (false);
        }
    }
    public void Update()
    {
        int range = 10000;
        int tempRange = range;
        Vector3 dir = Vector3.down;
        Vector3 temp;
        temp = p1.GetComponent<Transform>().position;
        tempRange = (int)Vector3.Distance(temp, enemy1.GetComponent<Transform>().position);
        if (tempRange < range)
        {
            range = tempRange;
            dir = temp;
        }
        temp = p2.GetComponent<Transform>().position;
        tempRange = (int)Vector3.Distance(temp, enemy1.GetComponent<Transform>().position);
        if (tempRange < range)
        {
            range = tempRange;
            dir = temp;
        }
        temp = p3.GetComponent<Transform>().position;
        tempRange = (int)Vector3.Distance(temp, enemy1.GetComponent<Transform>().position);
        if (tempRange < range)
        {
            range = tempRange;
            dir = temp;
        }
        enemy1.GetComponent<NavMeshAgent>().SetDestination(dir);

        temp = p1.GetComponent<Transform>().position;
        tempRange = (int)Vector3.Distance(temp, enemy2.GetComponent<Transform>().position);
        if (tempRange < range)
        {
            range = tempRange;
            dir = temp;
        }
        temp = p2.GetComponent<Transform>().position;
        tempRange = (int)Vector3.Distance(temp, enemy2.GetComponent<Transform>().position);
        if (tempRange < range)
        {
            range = tempRange;
            dir = temp;
        }
        temp = p3.GetComponent<Transform>().position;
        tempRange = (int)Vector3.Distance(temp, enemy2.GetComponent<Transform>().position);
        if (tempRange < range)
        {
            range = tempRange;
            dir = temp;
        }
        enemy2.GetComponent<NavMeshAgent>().SetDestination(dir);

        temp = p1.GetComponent<Transform>().position;
        tempRange = (int)Vector3.Distance(temp, enemy3.GetComponent<Transform>().position);
        if (tempRange < range)
        {
            range = tempRange;
            dir = temp;
        }
        temp = p2.GetComponent<Transform>().position;
        tempRange = (int)Vector3.Distance(temp, enemy3.GetComponent<Transform>().position);
        if (tempRange < range)
        {
            range = tempRange;
            dir = temp;
        }
        temp = p3.GetComponent<Transform>().position;
        tempRange = (int)Vector3.Distance(temp, enemy3.GetComponent<Transform>().position);
        if (tempRange < range)
        {
            range = tempRange;
            dir = temp;
        }
        enemy3.GetComponent<NavMeshAgent>().SetDestination(dir);
    }*/
}
