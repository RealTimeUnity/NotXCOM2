using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour {
    
	void Start ()
    {
        GameManager m = FindObjectOfType<GameManager>();
        this.gameObject.GetComponent<Button>().onClick.AddListener(delegate () { m.Exit(); });
	}
}
