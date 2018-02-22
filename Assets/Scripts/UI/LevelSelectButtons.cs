using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButtons : MonoBehaviour {

    void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        GameManager m = FindObjectOfType<GameManager>();
        for (int i = 0; i < buttons.Length; ++i)
        {
            string levelName = buttons[i].GetComponentInChildren<Text>().text;
            buttons[i].onClick.AddListener(delegate () { m.StartLevel(levelName); });
        }
    }
}
