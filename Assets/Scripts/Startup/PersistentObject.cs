using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObject : MonoBehaviour {
    
	void Start () {
        DontDestroyOnLoad(this);
	}
}
