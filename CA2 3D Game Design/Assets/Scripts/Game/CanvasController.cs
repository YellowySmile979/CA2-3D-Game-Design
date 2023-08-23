using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    [Header("P1 Ability")]
    public Image ability1TankOverlay;
    public Image ability2TankOverlay;

    public Image ability1TankImage, ability2TankImage;    

    public GameObject tankAbilityOverlay, tankUltimateOverlay;
    public CharacterData tankData;

    [Header("P2 Ability")]
    public Image ability1MageOverlay;
    public Image ability2MageOverlay;

    public Image ability1MageImage, ability2MageImage;

    public GameObject mageAbilityOverlay, mageUltimateOverlay;

    public CharacterData mageData;

    [Header("P1 Ultimate")]
    public Image ultimateTankOverlay;
    public Image ultimateTankImage;

    [Header("P2 Ultimate")]
    public Image ultimateMageOverlay, ultimateMageImage;

    [Header("Wave Info")]
    public Text waveNumber;
    public Text nextWaveTimer;

    [Header("Player1 Stats")]
    public Text player1Level;
    public Image player1HealthIcon;

    [Header("Player2 Stats")]
    public Text player2Level;
    public Image player2HealthIcon;

    [Header("Lose")]
    public GameObject loseScreen;
    public Text finalWaveReached, finalLevelReached;

    [Header("Pause")]
    public GameObject pauseScreen;
    bool onOrOff;

    [Header("Player1 Info")]
    public GameObject P1Info;
    bool onOrOff1;

    [Header("Player2 Info")]
    public GameObject P2Info;
    bool onOrOff2;

    [Header("Gem Objective")]
    public Image portalHealthIcon;
    float maxPortalHealth;

    [Header("Player Death")]
    public GameObject deathScreenP1;
    public GameObject deathScreenP2;
    public Text P1RespawnTimer, P2RespawnTimer;
    bool startP1Timer, startP2Timer;
    //[HideInInspector] public bool isDead;

    //a singleton
    public static CanvasController Instance;

    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //helps update the canvas accordingly
        //this section is for tank
        ultimateTankOverlay.fillAmount = 1f;
        ability1TankImage.sprite = tankData.ability1Sprite;
        ability2TankImage.sprite = tankData.ability2Sprite;
        ultimateTankImage.sprite = tankData.ultimateSprite;
        
        //this section is for mage
        ultimateMageOverlay.fillAmount = 1f;
        ability1MageImage.sprite = mageData.ability1Sprite;
        ability2MageImage.sprite = mageData.ability2Sprite;
        ultimateMageImage.sprite = mageData.ultimateSprite;

        //sets the max portal health
        if (FindObjectOfType<GemObjective>())
        {
            GemObjective gemObjective = FindObjectOfType<GemObjective>();
            maxPortalHealth = gemObjective.portalHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWaveInfo();

        if(loseScreen.activeSelf == true)
        {
            LoseScreen();
        }
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetAxisRaw("Pause") > 0)
        {
            Pause();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DisplayPlayer1Info();
        }
        if (Input.GetAxisRaw("DisplayInfo") < 0)
        {
            DisplayPlayer2Info();
        }

        if (startP1Timer)
        {
            StartP1RespawnTimer();
        }
        if (startP2Timer)
        {
            StartP2RespawnTimer();
        }
    }
    //displays cooldown time (for now its just for the tank)
    //RMB ADD PARITY WITH THE HEALER
    public IEnumerator DisplayCooldownTime(BasePlayerController player, int whichAbility, bool ability1 = true, bool ability2 = true)
    {
        //checks to see if the player is either a tank or healer
        if (player.GetComponent<TankPlayerController>())
        {
            //checks which ability to use
            if (whichAbility == 0) ability1TankOverlay.fillAmount = 1f;
            else if (whichAbility == 1) ability2TankOverlay.fillAmount = 1f;

            bool ability1HasCooledDown = ability1, ability2HasCooledDown = ability2;
            //to ensure the thing repeats itself until it shouldn't
            while (!ability1HasCooledDown)
            {
                print("Cooldown: Tank: Ability 1");

                //to prevent an infinite loop
                if (ability1HasCooledDown) break;

                //handles the tank's 1st ability cooldown
                if (ability1TankOverlay.fillAmount <= 1f 
                    && 
                    player.GetComponent<TankPlayerController>().waitTimeTillNextHeal > 0f)
                {
                    //this sets the fillamount to the timer
                    //it is divided by max time to ensure the float value is <= 1
                    ability1TankOverlay.fillAmount = player.GetComponent<TankPlayerController>().waitTimeTillNextHeal / player.GetComponent<TankPlayerController>().setWaitTimeTillNextHeal;
                }
                else if (ability1TankOverlay.fillAmount <= 0f 
                    && 
                    player.GetComponent<TankPlayerController>().waitTimeTillNextHeal <= 0f)
                {                    
                    ability1HasCooledDown = true;
                }
                yield return new WaitForSeconds(0.0001f);
            }
            while (!ability2HasCooledDown)
            {
                print("Cooldown: Tank: Ability 2");

                //to prevent an infinite loop
                if (ability2HasCooledDown) break;

                //handles the tank's 2nd ability cooldown
                if (ability2TankOverlay.fillAmount <= 1f 
                    && 
                    player.GetComponent<TankPlayerController>().waitTillNextAttract > 0f)
                {
                    //this sets the fillamount to the timer
                    //it is divided by max time to ensure the float value is <= 1
                    ability2TankOverlay.fillAmount = player.GetComponent<TankPlayerController>().waitTillNextAttract / player.GetComponent<TankPlayerController>().setWaitTillNextAttract;
                }
                else if (ability2TankOverlay.fillAmount <= 0f
                    &&
                    player.GetComponent<TankPlayerController>().waitTillNextAttract <= 0f)
                {
                    ability2HasCooledDown = true;
                }
                yield return new WaitForSeconds(0.0001f);
            }
        }
        else if (player.GetComponent<MagePlayerController>())
        {
            //checks which ability to use
            if (whichAbility == 0) ability1MageOverlay.fillAmount = 1f;
            else if (whichAbility == 1) ability2MageOverlay.fillAmount = 1f;

            bool ability1HasCooledDown = ability1, ability2HasCooledDown = ability2;
            //to ensure the thing repeats itself until it shouldn't
            while (!ability1HasCooledDown)
            {
                print("Cooldown: Mage: Ability 1");

                if (ability1HasCooledDown) break;

                //handles mage's 1st ability cooldown
                if (ability1MageOverlay.fillAmount <= 1f
                    &&
                    player.GetComponent<MagePlayerController>().timeTillNextPlacement > 0f)
                {
                    //this sets the fillamount to the timer
                    //it is divided by max time to ensure the float value is <= 1
                    ability1MageOverlay.fillAmount = player.GetComponent<MagePlayerController>().timeTillNextPlacement / player.GetComponent<MagePlayerController>().setTimeTillNextPlacement;
                }
                else if (ability1MageOverlay.fillAmount <= 0f
                    &&
                    player.GetComponent<MagePlayerController>().timeTillNextPlacement <= 0f)
                {
                    ability1HasCooledDown = true;
                }
                yield return new WaitForSeconds(0.0001f);
            }
            while(!ability2HasCooledDown)
            {
                print("Cooldown: Mage: Ability 2");

                //to prevent an infinite loop
                if (ability2HasCooledDown) break;

                //handles the tank's 2nd ability cooldown
                if (ability2MageOverlay.fillAmount <= 1f
                    &&
                    player.GetComponent<MagePlayerController>().timeTillNextFire > 0f)
                {
                    //this sets the fillamount to the timer
                    //it is divided by max time to ensure the float value is <= 1
                    ability2MageOverlay.fillAmount = player.GetComponent<MagePlayerController>().timeTillNextFire / player.GetComponent<MagePlayerController>().setTimeTillNextFire;
                }
                else if (ability2MageOverlay.fillAmount <= 0f
                    &&
                    player.GetComponent<MagePlayerController>().timeTillNextFire <= 0f)
                {
                    ability2HasCooledDown = true;
                }
                yield return new WaitForSeconds(0.0001f);
            }
        }
    }
    //handles the charging UI of a character's ultimate based on the amount of enemies killed
    public void UltimateCharge(float enemiesKilled, BasePlayerController player)
    {
        if (player.GetComponent<TankPlayerController>())
        {
            float tankOverlayFillAmount = 1f;
            if(ultimateTankOverlay.fillAmount > 0f)
            {
                tankOverlayFillAmount -= enemiesKilled / player.GetComponent<TankPlayerController>().requiredKills;
                ultimateTankOverlay.fillAmount = tankOverlayFillAmount;
            }
        }
        else if (player.GetComponent<MagePlayerController>())
        {
            float mageOverlayFillAmount = 1f;
            if(ultimateMageOverlay.fillAmount > 0f)
            {
                mageOverlayFillAmount -= enemiesKilled / player.GetComponent<MagePlayerController>().requiredKills;
                ultimateMageOverlay.fillAmount = mageOverlayFillAmount;
            }
        }
    }
    //updates the wave UI
    public void UpdateWaveInfo()
    {
        //updates wave number
        waveNumber.text = "Wave: " + WaveManager.Instance.waveNumber;
        PlayerPrefs.SetInt("Wave Number", WaveManager.Instance.waveNumber);

        //updates next wave timer
        if (WaveManager.Instance.gameState == WaveManager.GameState.Prep)
        {
            nextWaveTimer.text = "Next Wave In: " + Mathf.Round(WaveManager.Instance.waitTimeBetweenWaves);            
        }
        else if (WaveManager.Instance.gameState == WaveManager.GameState.Combat)
        {
            nextWaveTimer.text = "Wave has started!";
        }
    }
    //updates player level and health
    public void UpdatePlayerStats(BasePlayerController player)
    {
        player1Level.text = "Level: " + WaveManager.Instance.playerLevel;
        player2Level.text = "Level: " + WaveManager.Instance.playerLevel;

        if (player.GetComponent<TankPlayerController>())
        {
            player1HealthIcon.fillAmount = player.GetComponent<TankPlayerController>().playerHealth / player.GetComponent<TankPlayerController>().maxPlayerHealth;
        }
        else if (player.GetComponent<MagePlayerController>())
        {
            player2HealthIcon.fillAmount = player.GetComponent<MagePlayerController>().playerHealth / player.GetComponent<MagePlayerController>().maxPlayerHealth;
        }
    }
    //updates the UI for the portal health
    public void UpdateGemHealth(float portalHP)
    {
        portalHealthIcon.fillAmount = portalHP / maxPortalHealth;
    }
    //returns back to main menu
    public void BackToMainMenu()
    {
        print("MOM");
        SceneManager.LoadScene("MainMenu");
    }
    //handles lose screen
    public void LoseScreen()
    {
        finalWaveReached.text = "Final Wave Reached: " + PlayerPrefs.GetInt("Wave Number");
        finalLevelReached.text = "Final Level Reached: " + PlayerPrefs.GetInt("Level");
    }
    //handles the pausing of the game
    public void Pause()
    {
        onOrOff = !onOrOff;
        if (onOrOff)
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
        }
    }
    //displays player 1's info
    public void DisplayPlayer1Info()
    {
        onOrOff1 = !onOrOff1;
        if (onOrOff1)
        {
            P1Info.SetActive(true);
        }
        else
        {
            P1Info.SetActive(false);
        }
    }
    //displays player 2's info
    public void DisplayPlayer2Info()
    {
        onOrOff2 = !onOrOff2;
        if (onOrOff2)
        {
            P2Info.SetActive(true);
        }
        else
        {
            P2Info.SetActive(false);
        }
    }
    //handles the display of death screen UI
    public void HandlePlayerDeathScreenUI(BasePlayerController player, bool isDead)
    {
        //checks to see if the player is a tank or mage and activates their respective death screens and timer bools
        if (player.GetComponent<TankPlayerController>())
        {
            if (isDead)
            {
                deathScreenP1.SetActive(true);
                startP1Timer = true;
            }
            else
            {
                deathScreenP1.SetActive(false);
                startP1Timer = false;
            }
        }
        else if (player.GetComponent<MagePlayerController>())
        {
            if (isDead)
            {
                deathScreenP2.SetActive(true);
                startP2Timer = true;
            }
            else
            {
                deathScreenP2.SetActive(false);
                startP2Timer = false;
            }
        }
    }
    //handles P1 respawn timer
    void StartP1RespawnTimer()
    {
        TankPlayerController tank = FindObjectOfType<TankPlayerController>();
        P1RespawnTimer.text = "Respawn in: " + Mathf.Round(tank.timeTillNextPlayerSpawn);
    }
    //handles P2 respawn timer
    void StartP2RespawnTimer()
    {
        MagePlayerController mage = FindObjectOfType<MagePlayerController>();
        P2RespawnTimer.text = "Respawn in: " + Mathf.Round(mage.timeTillNextPlayerSpawn);
    }
}
