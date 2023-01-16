using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float respawnCooldown;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] respawnPoints;
    public bool stopSpawning;

    void Start()
    {
        StartCoroutine(spawnEnemy());
    }

    IEnumerator spawnEnemy()
    {
        System.Random random = new System.Random();
        while (true)
        {
            yield return new WaitForSeconds(respawnCooldown);
            if (!stopSpawning)
                Instantiate(enemyPrefabs[0], respawnPoints[random.Next(0, respawnPoints.Length)].position, Quaternion.identity);
        }
    }

    /// <summary>
    /// this function append time !
    /// </summary>
    public void changeSpawnCooldown(float cooldown, bool overwrite = false)
    {
        respawnCooldown = overwrite ? cooldown : respawnCooldown + cooldown;
    }
}
