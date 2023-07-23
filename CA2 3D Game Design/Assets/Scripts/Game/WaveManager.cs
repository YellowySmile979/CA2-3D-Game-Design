using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave")]
    public int waveNumber = 0;
    public float maxWaitTimeBetweenWaves;
    public float waitTimeBetweenWaves;
    int counterr;
    bool hasFiredOnce = false;

    public List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    public GameState gameState;
    public enum GameState
    {
        Start,
        Prep,
        Combat,
        Lost
    }

    [Header("Player Levelling")]
    public int playerLevel = 0;

    [Header("Enemies")]
    public List<BaseEnemy> existingEnemies = new List<BaseEnemy>();

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
        existingEnemies.AddRange(FindObjectsOfType<BaseEnemy>());

        waitTimeBetweenWaves = maxWaitTimeBetweenWaves;
        gameState = GameState.Start;
    }

    // Update is called once per frame
    void Update()
    {       
        if (gameState == GameState.Prep)
        {
            IncreaseWaveNumber();
            TimerTillNextWave();
            StopEnemySpawners();
        }
        else if (gameState == GameState.Combat)
        {
            StartEnemySpawners();
        }
        else if (gameState == GameState.Start)
        {
            waveNumber++;
            gameState = GameState.Combat;
        }
    }
    //increases wave number and sets the state to prep
    public void IncreaseWaveNumber(int counter = 0)
    {
        counterr += counter;
        if (counterr == enemySpawners.Count && existingEnemies.Count <= 0)
        {
            if (!hasFiredOnce)
            {
                waveNumber++;
                playerLevel++;
                hasFiredOnce = true;
            }
            gameState = GameState.Prep;
        }
    }
    //does as name implies
    void StopEnemySpawners()
    {
        foreach(EnemySpawner enemySpawner in enemySpawners)
        {
            enemySpawner.gameObject.SetActive(false);
        }
    }
    //does as name implies
    void StartEnemySpawners()
    {
        foreach (EnemySpawner enemySpawner in enemySpawners)
        {
            enemySpawner.gameObject.SetActive(true);
        }
    }
    //handles the timer till next wave
    void TimerTillNextWave()
    {
        if (existingEnemies.Count <= 0)
        {
            if (waitTimeBetweenWaves > 0)
            {
                waitTimeBetweenWaves -= Time.deltaTime;
            }
            else
            {
                int count = enemySpawners.Count;
                IncreaseWaveNumber(-count);
                hasFiredOnce = false;
                waitTimeBetweenWaves = maxWaitTimeBetweenWaves;

                foreach(EnemySpawner enemySpawner in enemySpawners)
                {
                    enemySpawner.amountSpawnedAlready = 0;
                    enemySpawner.hasGiven = false;
                }

                gameState = GameState.Combat;
            }
        }
    }
}
