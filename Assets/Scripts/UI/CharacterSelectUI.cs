using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour {

    public Button characterInfoButtonPrefab;

    protected int selectedCharacter;

    protected Text selectedNameText;
    protected Button continueButton;
    protected Button endTurnButton;

    protected HumanController humanController;
    protected RectTransform content;

    protected List<Button> activeButtons;

    void Start()
    {
        activeButtons = new List<Button>();

        continueButton = GetComponentsInChildren<Button>(true)[0];
        endTurnButton = GetComponentsInChildren<Button>(true)[1];
        selectedNameText = GetComponentsInChildren<Text>(true)[1];
        content = GetComponentsInChildren<RectTransform>(true)[8];

        selectedCharacter = -1;
    }
	
	public void UpdateList (HumanController humanController) {
        this.humanController = humanController;

        endTurnButton.onClick.RemoveAllListeners();
        endTurnButton.onClick.AddListener(delegate () { this.humanController.EndTurn(); });

        content.sizeDelta = new Vector2(content.sizeDelta.x, 100 * humanController.friendlies.Count);
        for (int i = 0; i < activeButtons.Count; ++i)
        {
            Destroy(activeButtons[i].gameObject);
        }
        activeButtons.Clear();

        MeshRenderer mr;
        for (int i = 0; i < humanController.friendlies.Count; ++i)
        {
            mr = humanController.friendlies[i].GetComponent<MeshRenderer>();
            if (mr == null)
            {
                humanController.friendlies[i].GetComponentInChildren<SkinnedMeshRenderer>().material.SetInt("_Highlighted", 0);
            }
            else
            {
                mr.material.SetInt("_Highlighted", 0);
            }
        }

        for (int i = 0; i < humanController.friendlies.Count; ++i)
        {
            Character c = humanController.friendlies[i];

            if (c.hasHadTurn)
            {
                continue;
            }

            Button characterInfoButton = Instantiate(characterInfoButtonPrefab, content);
            characterInfoButton.transform.position += new Vector3(0, (-100 * activeButtons.Count), 0);

            Text[] textFields = characterInfoButton.GetComponentsInChildren<Text>();
            Image[] images = characterInfoButton.GetComponentsInChildren<Image>(true);
            Image[] abilityImages = { images[1], images[2], images[3], images[4], images[5], images[6] };

            textFields[0].text = c.Name;
            textFields[1].text = "Health: " + c.currentHealth.ToString() + " / " + c.MaxHealth.ToString();
            textFields[2].text = "Major Abilities: " + c.numMajorAbilities.ToString() + " / " + c.maxMajorAbilities.ToString();
            textFields[3].text = "Minor Abilities: " + c.numMinorAbilities.ToString() + " / " + c.maxMinorAbilities.ToString();

            for (int j = 0; j < c.abilities.Count && j < 6; ++j)
            {
                abilityImages[j].gameObject.SetActive(true);
                abilityImages[j].sprite = c.abilities[j].sprite;
            }

            int index = i;
            characterInfoButton.onClick.AddListener(delegate { SelectCharacter(index); });
            activeButtons.Add(characterInfoButton);
        }
    }

    void SelectCharacter(int characterIndex)
    {
        this.selectedCharacter = characterIndex;
        this.selectedNameText.text = humanController.friendlies[this.selectedCharacter].Name;
        MeshRenderer mr;
        for (int i = 0; i < humanController.friendlies.Count; ++i)
        {
            mr = humanController.friendlies[i].GetComponent<MeshRenderer>();
            if (mr == null)
            {
                humanController.friendlies[i].GetComponentInChildren<SkinnedMeshRenderer>().material.SetInt("_Highlighted", 0);
            }
            else
            {
                mr.material.SetInt("_Highlighted", 0);
            }
        }

        mr = humanController.friendlies[selectedCharacter].GetComponent<MeshRenderer>();
        if (mr == null)
        {
            humanController.friendlies[selectedCharacter].GetComponentInChildren<SkinnedMeshRenderer>().material.SetInt("_Highlighted", 1);
            humanController.friendlies[selectedCharacter].GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_OutlineColor", new Color(0, 1, 0));
        }
        else
        {
            mr.material.SetInt("_Highlighted", 1);
            mr.material.SetColor("_OutlineColor", new Color(0, 1, 0));
        }
    }

    public void Continue()
    {
        if (this.selectedCharacter >= 0)
        {
            humanController.SelectSubject(this.selectedCharacter);
            this.selectedCharacter = -1;
            this.selectedNameText.text = "";
        }
    }
}
