using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [Header("Wave")]
    public int waveNumber = 0;
    public float maxWaitTimeBetweenWaves;
    public float waitTimeBetweenWaves;
    int counterr;
    bool hasFiredOnce = false;

    public List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    [Header("Players")]
    public List<BasePlayerController> players = new List<BasePlayerController>();

    public GameState gameState;
    public enum GameState
    {
        Start,
        Prep,
        Combat,
        Lost,
        Win
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

        players.AddRange(FindObjectsOfType<BasePlayerController>());

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
            StartCoroutine(StopEnemySpawners());
        }
        else if (gameState == GameState.Combat)
        {
            foreach(BasePlayerController basePlayerController in players)
            {
                if (basePlayerController.hasLevelledUp)
                {
                    basePlayerController.hasLevelledUp = false;
                }
            }
            StartEnemySpawners();
        }
        else if (gameState == GameState.Start)
        {
            waveNumber++;
            gameState = GameState.Combat;
        }
        else if(gameState == GameState.Lost)
        {
            Time.timeScale = 0;
            PlayerPrefs.SetInt("Level", playerLevel);
            CanvasController.Instance.loseScreen.SetActive(true);
        }
        else if (gameState == GameState.Win)
        {
            SceneManager.LoadScene("Win");
        }
        if(waveNumber >= 26)
        {
            gameState = GameState.Win;
        }
    }
    int counter3 = 0;
    //increases wave number and sets the state to prep
    public void IncreaseWaveNumber(int counter = 0)
    {
        counterr += counter;
        if (counterr == enemySpawners.Count && existingEnemies.Count <= 0)
        {
            if (!hasFiredOnce)
            {
                waveNumber++;
                //restricts how much players can level up
                if (waveNumber <= 20)
                {
                    if (counter3 >= 5)
                    {
                        playerLevel++;
                        //levels up the player
                        foreach (BasePlayerController basePlayerController in players)
                        {
                            if (!basePlayerController.hasLevelledUp)
                            {
                                basePlayerController.LevelUp(playerLevel);
                            }
                        }
                    }
                    else if(counter < 5)
                    {
                        counter3++;
                    }
                }

                hasFiredOnce = true;
            }
            gameState = GameState.Prep;
        }
    }
    //does as name implies
    IEnumerator StopEnemySpawners()
    {
        yield return new WaitForSeconds(0.01f);
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
