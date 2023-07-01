using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemies to Spawn")]
    public List<BaseEnemy> enemiesToSpawn = new List<BaseEnemy>();
    public int minRangeOfEnemiesToSpawn, maxRangeOfEnemiesToSpawn;

    [Header("Time between Spawns")]
    public float maxTimeBetweenSpawns = 10f;
    float timeBetweenSpawns;

    [Header("Limits")]
    public int spawnLimit;
    int amountSpawnedAlready;
    public bool includeSpawnLimit;

    // Start is called before the first frame update
    void Start()
    {
        timeBetweenSpawns = maxTimeBetweenSpawns;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnEnemies();
    }
    void SpawnEnemies()
    {
        //if we want spawn limit than tick bool
        //if no spawm limit, is infinite spawn
        //otherwise everytime enemy is spawned, increase the amountSpawnedAlready by 1 until spawnlimit
        if (!includeSpawnLimit)
        {
            if (timeBetweenSpawns > 0)
            {
                timeBetweenSpawns -= Time.deltaTime;
            }
            else
            {
                //randomises which enemy spawns
                int randomEnemy = Random.Range(minRangeOfEnemiesToSpawn, maxRangeOfEnemiesToSpawn);
                //instantiates that enemy
                Instantiate(enemiesToSpawn[randomEnemy], transform.position, Quaternion.identity);
                timeBetweenSpawns = maxTimeBetweenSpawns;
            }
        }
        else
        {
            if (timeBetweenSpawns > 0)
            {
                timeBetweenSpawns -= Time.deltaTime;
            }
            else
            {
                if (amountSpawnedAlready < spawnLimit)
                {
                    int randomEnemy = Random.Range(minRangeOfEnemiesToSpawn, maxRangeOfEnemiesToSpawn);
                    Instantiate(enemiesToSpawn[randomEnemy], transform.position, Quaternion.identity);
                }
                amountSpawnedAlready++;
                timeBetweenSpawns = maxTimeBetweenSpawns;
            }
        }
    }
}
