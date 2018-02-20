using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ButtonScript : MonoBehaviour {
    protected enum CombatPhase
    {
        None,
        ActionSelection,
        TargetSelection,
        ActionExecution
    }

    protected HumanController hugh_man;
    protected Character current_char;
    
    protected Image backGround;
    protected Button[] abilityButtons;
    protected Text[] abilityTexts;
    protected Slider healthSlider;
    protected Button cancelButton;
    protected Button endTurnButton;
    protected Text cancelText;
    protected Text nameText;
    protected Text majorText;
    protected Text minorText;
    protected Text[] useTexts;

    protected CombatPhase curr_phase = CombatPhase.None;
    public int max_abilities = 6;
    protected int prev_index = -1;
    protected int ability_count = 0;
    protected bool buttonClicked = false;
    protected int current_button = -1;

	void Start () {
        hugh_man = FindObjectOfType<HumanController>();

        backGround = this.GetComponentInChildren<Image>(true);
        Slider[] sliders = backGround.gameObject.GetComponentsInChildren<Slider>(true);
        Button[] buttons = backGround.gameObject.GetComponentsInChildren<Button>(true);
        Text[] text = backGround.gameObject.GetComponentsInChildren<Text>(true);

        abilityButtons = new Button[] { buttons[0], buttons[1], buttons[2], buttons[3], buttons[4], buttons[5] };
        cancelButton = buttons[6];
        endTurnButton = buttons[7];
        abilityTexts = new Text[] { text[1], text[4], text[7], text[10], text[13], text[16] };
        useTexts = new Text[] { text[3], text[6], text[9], text[12], text[15], text[18] };
        majorText = text[22];
        minorText = text[23];
        cancelText = text[7];
        nameText = text[0];
        healthSlider = sliders[0];
    }

    int buttonRollCall()
    {
        int ret = 0;
        for(int i = 0; i < max_abilities; i++)
        {
            if (abilityButtons[i].interactable)
            {
                ret++;
            }
        }
        return ret;
    }

    void cancel()
    {
        hugh_man.CancelAbility();
        buttonClicked = false;
        curr_phase = CombatPhase.None;
        prev_index = -1;
    }

    void endTurn()
    {
        hugh_man.friendlies[hugh_man.subjectIndex].hasHadTurn = true;
        hugh_man.phase = CharacterController.TurnPhase.SelectCharacter;
    }

    void updateInfo()
    {
        current_char = hugh_man.friendlies[hugh_man.subjectIndex];

        nameText.text = current_char.Name;

        healthSlider.minValue = 0;
        healthSlider.maxValue = current_char.MaxHealth;
        healthSlider.value = current_char.currentHealth;

        majorText.text = current_char.numMajorAbilities.ToString();
        minorText.text = current_char.numMinorAbilities.ToString();

        for (int i = 0; i < max_abilities; i++)
        {
            abilityTexts[i].text = "";
            abilityButtons[i].interactable = false;
            abilityButtons[i].onClick.RemoveAllListeners();
        }
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(cancel);
        cancelButton.interactable = false;

        endTurnButton.onClick.RemoveAllListeners();
        endTurnButton.onClick.AddListener(endTurn);
        endTurnButton.interactable = true;

        //filling in values
        for (int i = 0; i < current_char.abilities.Count; i++)
        {
            int index = i;
            abilityTexts[i].text = current_char.abilities[i].abilityName;
            useTexts[i].text = current_char.abilities[i].uses.ToString();
            abilityButtons[i].onClick.AddListener(delegate { buttonOnClick(index); });
            ability_count++;
            abilityButtons[i].interactable = true;
            abilityButtons[i].GetComponent<Image>().sprite = current_char.abilities[i].sprite;
        }
    }
	
    void buttonOnClick(int buttonIndex)
    {
        if (!buttonClicked)
        {
            current_button = buttonIndex;
            buttonClicked = true;
            hugh_man.SelectAbility(abilityTexts[current_button].text);
            curr_phase = CombatPhase.TargetSelection;
        }
        else
        {
            if (hugh_man.target != null)
            {
                buttonClicked = false;
                hugh_man.ConfirmAbility();
                curr_phase = CombatPhase.ActionExecution;
            }
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (current_char != null)
        {
            healthSlider.value = current_char.currentHealth;
            majorText.text = current_char.numMajorAbilities.ToString();
            minorText.text = current_char.numMinorAbilities.ToString();
        }

        switch (curr_phase)
        {
            case CombatPhase.None:
                if (hugh_man.subjectIndex != -1)
                {
                    this.backGround.gameObject.SetActive(true);
                    if (hugh_man.subjectIndex != prev_index)
                    {
                        updateInfo();
                        prev_index = hugh_man.subjectIndex;
                        curr_phase = CombatPhase.ActionSelection;
                    }
                }
                else
                {
                    this.backGround.gameObject.SetActive(false);
                }
                break;
            case CombatPhase.ActionSelection:
                if(hugh_man.subjectIndex == -1 || hugh_man.subjectIndex != prev_index)
                {
                    curr_phase = CombatPhase.None;
                }


                for (int i = 0; i < current_char.abilities.Count; i++)
                {
                    Ability ability = current_char.GetAbility(abilityTexts[i].text);
                    useTexts[i].text = ability.uses.ToString();
                    if (ability.uses <= 0 || (ability.type == Ability.AbilityType.Major && current_char.numMajorAbilities <= 0) || (ability.type == Ability.AbilityType.Minor && current_char.numMinorAbilities <= 0))
                    {
                        abilityButtons[i].interactable = false;
                    }
                }
                endTurnButton.interactable = true;
                break;
            case CombatPhase.TargetSelection:
                if(buttonRollCall() != 1)
                {
                    for(int i = 0; i < max_abilities; i++)
                    {
                        abilityButtons[i].interactable = false;
                    }
                    abilityButtons[current_button].interactable = true;
                }
                
                cancelButton.interactable = true;
                endTurnButton.interactable = false;
                break;
            case CombatPhase.ActionExecution:
                //wait for an undetermined ammount of time
                cancelButton.interactable = false;
                prev_index = -1;
                curr_phase = CombatPhase.None;
                break;
        }
    }
}
