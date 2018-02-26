using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiController : CharacterController
{
    List<Ability> abilities = new List<Ability>();
    List<Target> targets=new List<Target>();
    List<int> scores=new List<int>();
    int scoreInteger = -1;

    protected override string GetAbilityName()
    {
        abilities = new List<Ability>();
        targets = new List<Target>();
        scores = new List<int>();
        scoreInteger = -1;

        Character actor=this.friendlies[this.subjectIndex];
        for(int i = 0; i < actor.abilities.Count; i++)
        {
            magicalAbilityScoreGeneration(actor.abilities[i], actor);
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

        if (scoreInteger == -1)
        {
            this.phase = TurnPhase.SelectCharacter;
            return null;
        }

        return abilities[scoreInteger].abilityName;
    }

    protected override int GetSubjectIndex()
    {
        return this.subjectIndex + 1;
    }

    protected override Character GetSelfSelection()
    {
        if (targets[scoreInteger].GetTargetType() == Target.TargetType.Self)
        {
            this.ConfirmAbility();
            return this.friendlies[this.subjectIndex];
        }
        else
        {
            return null;
        }
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
            this.ConfirmAbility();
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
            this.ConfirmAbility();
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
        
        if (actor.HasAbility(abilityName) && actor.IsAbilityExecutable(abilityName))
        {
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
                        dir = enemy.friendlies[i].GetComponent<Transform>().position - temp;

                    }
                }

                if (dir.magnitude > ability.range)
                {
                    dir = actor.transform.position + (dir.normalized * ability.range);
                }
                else
                {
                    dir = actor.transform.position + dir;
                }

                abilities.Add(ability);
                targets.Add(new Target(dir));
                scores.Add(20);
            }
            if (abilityName == "Team Heal")
            {
                float healValue = 0;
                float healCount = 0;
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
                if (healCount != 0)
                {
                    healValue = (healValue / healCount);
                }
                else
                {
                    healValue = 0;
                }
                abilities.Add(ability);
                targets.Add(new Target(Target.TargetType.Self, actor));
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
                        shieldValue *= .9f;
                    }

                }
                abilities.Add(ability);
                targets.Add(new Target(Target.TargetType.Self, actor));
                scores.Add((int)shieldValue);

            }
            if (abilityName == "Self Shield")
            {
                int shieldScore = 0;
                if (((SelfShieldAbility)actor.GetAbility(abilityName)).ShieldActive)
                {
                    shieldScore += 10;
                }

                abilities.Add(ability);
                targets.Add(new Target(Target.TargetType.Self, actor));
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
                abilities.Add(ability);
                targets.Add(new Target(Target.TargetType.Self, actor));
                scores.Add(healValue);
            }
            if (abilityName == "Flame thrower" || abilityName == "Pistol" || abilityName == "Rifle" || abilityName == "Shotgun" || abilityName == "Sniper")
            {
                int maxValue = 0;
                Character primaryVictim = new Character();
                for (int i = 0; i < enemy.friendlies.Count; i++)
                {
                    Character victim = enemy.friendlies[i];
                    Target tempTarget = new Target(Target.TargetType.Enemy, victim);
                    if (ability.IsTargetInRange(actor, tempTarget))
                    {
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
                }
                abilities.Add(ability);
                targets.Add(new Target(Target.TargetType.Enemy, primaryVictim));
                scores.Add(maxValue);
            }
        }
    }
}
