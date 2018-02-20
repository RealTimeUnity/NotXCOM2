using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public Canvas MainCanvas;
    public Canvas OptionsCanvas;

    void Awake()
    {
        OptionsCanvas.enabled = false;
    }
    public void OptionsOn()
    {
        OptionsCanvas.enabled = true;
        MainCanvas.enabled = false;
    }
    public void ReturnOn()
    {
        OptionsCanvas.enabled = false;
        MainCanvas.enabled = true;
    }
    public void LoadOn()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        Application.LoadLevel(1);
#pragma warning restore CS0618 // Type or member is obsolete
    }
    public void ExitOn()
    {
        Application.Quit();
    }
}
