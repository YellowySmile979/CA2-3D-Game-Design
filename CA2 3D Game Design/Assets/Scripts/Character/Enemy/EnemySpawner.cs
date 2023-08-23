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
    public int amountSpawnedAlready;
    public bool includeSpawnLimit, hasGiven;
    int count = 1;
    static bool hasIncreased;

    // Start is called before the first frame update
    void Start()
    {
        timeBetweenSpawns = maxTimeBetweenSpawns;
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasGiven) SpawnEnemies();
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
                    //does the same as above, just that this has a spawn limit
                    int randomEnemy = Random.Range(minRangeOfEnemiesToSpawn, maxRangeOfEnemiesToSpawn);
                    BaseEnemy spawnedEnemy = Instantiate(enemiesToSpawn[randomEnemy], transform.position, Quaternion.identity);
                    WaveManager.Instance.existingEnemies.Add(spawnedEnemy);
                }
                amountSpawnedAlready++;
                timeBetweenSpawns = maxTimeBetweenSpawns;
            }
            if(amountSpawnedAlready >= spawnLimit && !hasGiven)
            {
                print("Increase Wave Number");
                hasIncreased = false;
                WaveManager.Instance.IncreaseWaveNumber(count);
                WaveManager.Instance.gameState = WaveManager.GameState.Prep;
                if(WaveManager.Instance.waveNumber <= 20 && !hasIncreased)
                {
                    spawnLimit++;
                    hasIncreased = true;
                }
                hasGiven = true;
            }
        }
    }
}
