using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave")]
    public int waveNumber = 0;
    public float maxWaitTimeBetweenWaves;
    float waitTimeBetweenWaves;

    public List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    public GameState gameState;
    public enum GameState
    {
        Prep,
        Combat,
        Lost
    }

    [Header("Player Levelling")]
    public int playerLevel = 0;

    //a singleton
    public static WaveManager Instance;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        enemySpawners.AddRange(FindObjectsOfType<EnemySpawner>());

        waitTimeBetweenWaves = maxWaitTimeBetweenWaves;
        gameState = GameState.Prep;
    }

    // Update is called once per frame
    void Update()
    {
        IncreaseWaveNumber();
    }
    void IncreaseWaveNumber()
    {
        foreach(EnemySpawner enemySpawner in enemySpawners)
        {
            if (!enemySpawner.canSpawn)
            {
                waveNumber += 1;
                playerLevel += 1;
                gameState = GameState.Prep;
                if(waitTimeBetweenWaves > 0)
                {
                    waitTimeBetweenWaves -= Time.deltaTime;
                }
                else
                {
                    waitTimeBetweenWaves = maxWaitTimeBetweenWaves;
                    gameState = GameState.Combat;
                    enemySpawner.canSpawn = true;
                }
            }
        }
    }
}
