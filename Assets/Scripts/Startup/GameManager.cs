using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    [SerializeField]
    private CharacterController playerOne;
    [SerializeField]
    private CharacterController playerTwo;

    public Text screenText;
    public GameObject combatUI;
    public GameObject selectUI;

    protected string MAIN_SCENE_NAME = "MainMenu";
    protected string LOAD_LEVEL_NAME = "LoadLevel";

    protected int playerNumber;

    protected Vector3 cameraStartLocation;
    
    public void Start()
    {
        playerNumber = 2;
        playerOne.gameObject.SetActive(false);
        playerTwo.gameObject.SetActive(false);
    }

    public void SpawnWave(WaveLoader waveLoader)
    {
        cameraStartLocation = FindObjectOfType<CameraController>().transform.localPosition;
        playerOne.gameObject.SetActive(true);
        playerTwo.gameObject.SetActive(true);

        playerOne.Initialize();
        playerTwo.Initialize();
        playerOne.updating = true;
        playerTwo.updating = true;

        playerOne.CreateFriendlyCharacters(waveLoader.GetRandomSpawnPoint());
        playerTwo.CreateFriendlyCharacters(waveLoader.GetRandomSpawnPoint());

        playerOne.SetEnemy(playerTwo);
        playerTwo.SetEnemy(playerOne);

        playerNumber = 2;
        FinishTurn();
    }

    IEnumerator StartTurn(CharacterController player)
    {
        FindObjectOfType<CameraController>().FocusLocation(cameraStartLocation);
        yield return new WaitForSeconds(2);
        screenText.gameObject.SetActive(false);
        player.StartTurn();
    }

    public IEnumerator EndGame(CharacterController player)
    {
        playerOne.updating = false;
        playerTwo.updating = false;

        if (player == playerOne)
        {
            screenText.text = "Player One Wins!";
        }
        else
        {
            screenText.text = "Player Two Wins!";
        }
        screenText.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.01f);

        combatUI.SetActive(false);
        selectUI.SetActive(false);
        
        yield return new WaitForSeconds(5);
        screenText.gameObject.SetActive(false);
        SceneManager.LoadScene(MAIN_SCENE_NAME);

        combatUI.SetActive(false);
        selectUI.SetActive(false);
        playerOne.gameObject.SetActive(false);
        playerTwo.gameObject.SetActive(false);
    }

    public void FinishTurn()
    {
        if (playerNumber == 1)
        {
            playerNumber = 2;
            screenText.text = "Player Two: Start Turn";
            screenText.gameObject.SetActive(true);
            StartCoroutine(StartTurn(playerTwo));
        }
        else
        {
            playerNumber = 1;
            screenText.text = "Player One: Start Turn";
            screenText.gameObject.SetActive(true);
            StartCoroutine(StartTurn(playerOne));
        }
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene(LOAD_LEVEL_NAME);
    }

    public void StartLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
