using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Character : MonoBehaviour {
    
    public string Name;

    [HideInInspector]
    public float currentHealth;
    public float MaxHealth;
    public bool isHealthBarActive = true;

    [HideInInspector]
    public CharacterController owner;

    public int maxMajorAbilities;
    public int maxMinorAbilities;
    [HideInInspector]
    public int numMajorAbilities;
    [HideInInspector]
    public int numMinorAbilities;

    [HideInInspector]
    public bool hasHadTurn = false;

    public List<Ability> abilityPrefabs;

    [HideInInspector]
    public List<Ability> abilities;
    [HideInInspector]
    public List<Ability> passiveAbilities;

    private NavMeshAgent agent;
    [SerializeField]
    private Animator anim;

    public GameObject healthbarPrefab;
    protected GameObject healthbar;
    protected SpriteRenderer healthbarValue;
    protected Camera mainCamera;
    protected float baseHealthbarScale;
    [SerializeField]
    private bool dead = false;
    [SerializeField]
    private float die = 0;
    private float dx;

    [SerializeField]
    private SkinnedMeshRenderer renderer;

    void Start()
    {
        currentHealth = MaxHealth;
        this.abilities = new List<Ability>();
        this.passiveAbilities = new List<Ability>();
        for (int i = 0; i < this.abilityPrefabs.Count; ++i)
        {
            Ability ability = Instantiate(abilityPrefabs[i], this.gameObject.transform);
            ability.Initialize(this);
            this.abilities.Add(ability);
        }
        this.ResetTurn();

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        healthbar = Instantiate(healthbarPrefab, this.gameObject.transform);
        healthbarPrefab.SetActive(true);
        healthbarValue = healthbar.GetComponentsInChildren<SpriteRenderer>()[1];
        baseHealthbarScale = healthbarValue.transform.localScale.x;
    }


    public void Die()
    {
        dead = true;
    }
    public void DieForReal()
    {
        Destroy(gameObject);
    }

    public void Update()
    {
        renderer.material.SetFloat("_Die", die);

        if(dead)
        {
            die = Mathf.SmoothDamp(die, 1.0f, ref dx, 1.5f);
            if (die > 0.95f)
                Destroy(gameObject);
        }


        anim.SetFloat("Forward", agent.velocity.magnitude / 3.0f);
        healthbar.transform.rotation = mainCamera.transform.rotation;
        healthbarValue.transform.localScale = new Vector3((currentHealth / MaxHealth) * baseHealthbarScale, healthbarValue.transform.localScale.y, healthbarValue.transform.localScale.z);
    }

    public bool HasAbility(string abilityName)
    {
        bool result = false;
        for (int i = 0; i < this.abilities.Count; i++)
        {
            if (this.abilities[i].abilityName == abilityName)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    public Ability GetAbility(string abilityName)
    {
        Ability result = null;
        for (int i = 0; i < this.abilities.Count; i++)
        {
            if (this.abilities[i].abilityName == abilityName)
            {
                result = this.abilities[i];
            }
        }

        return result;
    }

    public bool IsAbilityExecutable(string abilityName)
    {
        Ability ability = this.GetAbility(abilityName);
        bool result = false;
        Ability.AbilityType abilityType = ability.GetAbilityType();

        if (ability.uses > 0)
        {
            if (abilityType == Ability.AbilityType.Major &&
            this.numMajorAbilities > 0)
            {
                result = true;
            }
            if (abilityType == Ability.AbilityType.Minor &&
                this.numMinorAbilities > 0)
            {
                result = true;
            }
        }

        return result;
    }

    public bool HasMoreAbilities()
    {
        bool result = false;

        for (int i = 0; i < this.abilities.Count; ++i)
        {
            if (this.abilities[i].GetAbilityType() == Ability.AbilityType.Major)
            {
                if (this.abilities[i].uses > 0 && this.numMajorAbilities > 0)
                {
                    result = true;
                    break;
                }
            }
            else
            {
                if (this.abilities[i].uses > 0 && this.numMinorAbilities > 0)
                {
                    result = true;
                    break;
                }
            }
        }

        return result;
    }

    public void ResetTurn()
    {
        for (int i = 0; i < this.abilities.Count; ++i)
        {
            this.abilities[i].ResetCount();
        }

        this.numMajorAbilities = maxMajorAbilities;
        this.numMinorAbilities = maxMinorAbilities;
        this.hasHadTurn = false;

        for (int i = 0; i < this.passiveAbilities.Count; i++)
        {
            passiveAbilities[i].Die();
        }
        this.passiveAbilities.Clear();
    }

    public void ExecuteAbility(string abilityName, Target target)
    {
        Ability ability = this.GetAbility(abilityName);
        Ability.AbilityType abilityType = ability.GetAbilityType();
        if (abilityType == Ability.AbilityType.Major)
        {
            --this.numMajorAbilities;
        }
        if (abilityType == Ability.AbilityType.Minor)
        {
            --this.numMinorAbilities;
        }

        ability.Execute(target);
    }

    public void TakeDamage(int damage)
    {
        SelfShieldAbility shield = null;
        if (HasAbility("Self Shield"))
        {
            shield = GetAbility("Self Shield").GetComponent<SelfShieldAbility>();
        }

        for (int i = 0; i < this.passiveAbilities.Count; i++)
        {
            if (this.passiveAbilities[i].abilityName == "Self Shield")
            {
                shield = this.passiveAbilities[i].GetComponent<SelfShieldAbility>();
                break;
            }
        }
        
        if (shield != null)
        {
            damage = shield.TakeDamage(damage);
        }

        currentHealth += damage;
    }
}
