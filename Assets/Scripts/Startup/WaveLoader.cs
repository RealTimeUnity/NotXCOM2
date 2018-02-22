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
        int i = 0;
        for (; i < this.spawnPoints.Count; ++i)
        {
            if (!this.spawnPoints[i].isOccupied == true)
            {
                break;
            }
        }

        this.spawnPoints[i].isOccupied = true;
        return this.spawnPoints[i];
    }
}
