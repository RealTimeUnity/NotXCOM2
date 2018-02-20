using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveLoader : MonoBehaviour {

    public List<SpawnPoint> spawnPoints;

    public void Start()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.SpawnWave(this);
    }

    public SpawnPoint GetRandomSpawnPoint()
    {
        int index = Random.Range(0, this.spawnPoints.Count - 1);
        while (this.spawnPoints[index].isOccupied)
        {
            index = Random.Range(0, this.spawnPoints.Count - 1);
        }

        this.spawnPoints[index].isOccupied = true;
        return this.spawnPoints[index];
    }
}
