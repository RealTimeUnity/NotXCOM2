using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    
    [SerializeField]
    private CharacterController playerOne;
    [SerializeField]
    private CharacterController playerTwo;

    public Text nextPlayerText;

    protected int playerNumber;
    
    public void Start()
    {
        playerNumber = 2;
        FinishTurn();
    }

    public void SpawnWave(WaveLoader waveLoader)
    {
        playerOne.CreateFriendlyCharacters(waveLoader.GetRandomSpawnPoint());
        playerTwo.CreateFriendlyCharacters(waveLoader.GetRandomSpawnPoint());

        playerOne.SetEnemy(playerTwo);
        playerTwo.SetEnemy(playerOne);
    }

    IEnumerator Wait(CharacterController player)
    {
        yield return new WaitForSeconds(2);
        nextPlayerText.gameObject.SetActive(false);
        player.StartTurn();
    }

    public void FinishTurn()
    {
        if (playerNumber == 1)
        {
            playerNumber = 2;
            nextPlayerText.text = "Player Two: Start Turn";
            nextPlayerText.gameObject.SetActive(true);
            StartCoroutine(Wait(playerTwo));
        }
        else
        {
            playerNumber = 1;
            nextPlayerText.text = "Player One: Start Turn";
            nextPlayerText.gameObject.SetActive(true);
            StartCoroutine(Wait(playerOne));
        }
    }
}
