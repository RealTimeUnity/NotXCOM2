using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePreLoader : MonoBehaviour {

    private const string PRELOAD_SCENE_NAME = "PreloadScene";

    public static ScenePreLoader Singleton;
    private string nextScene;

    public void Awake()
    {
        if(Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);

            nextScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(PRELOAD_SCENE_NAME);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    
    public void FinishLoad()
    {
        SceneManager.LoadScene(nextScene);
    }
}
