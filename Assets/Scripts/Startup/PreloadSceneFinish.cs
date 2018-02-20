using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreloadSceneFinish : MonoBehaviour {
    
	void Start () {
        ScenePreLoader.Singleton.FinishLoad();
	}
}
