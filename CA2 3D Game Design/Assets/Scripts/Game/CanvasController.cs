using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [Header("Ability")]
    public Image ability1TankOverlay;
    public Image ability2TankOverlay, ability1HealerOverlay, ability2HealerOverlay;

    [Header("Ultimate")]
    public Image ultimateTankOverlay;
    public Image ultimateHealerOverlay;

    [Header("Wave Info")]
    public Text waveNumber;
    public Text nextWaveTimer;

    [Header("Player Stats")]
    public Text playerLevel;
    public Text playerHealthText;

    //a singleton
    public static CanvasController Instance;

    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        ultimateTankOverlay.fillAmount = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWaveInfo();
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
        else if (player.GetComponent<HealerPlayerController>())
        {
            bool hasCooledDown = false;
            while (!hasCooledDown)
            {
                if (hasCooledDown) break;

                yield return new WaitForSeconds(0.0001f);
            }
        }
    }
    //handles the charging of a character's ultimate based on the amount of enemies killed
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
        else if (player.GetComponent<HealerPlayerController>())
        {

        }
    }
    //updates the wave UI
    public void UpdateWaveInfo()
    {
        //updates wave number
        waveNumber.text = "Wave: " + WaveManager.Instance.waveNumber;

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
    //updates player level
    public void UpdatePlayerStats(BasePlayerController player)
    {
        playerLevel.text = "Player 1 Level: " + WaveManager.Instance.playerLevel;

        if (player.GetComponent<TankPlayerController>())
        {
            playerHealthText.text = "HP: " + player.GetComponent<TankPlayerController>().playerHealth;
        }
        else if (player.GetComponent<HealerPlayerController>())
        {

        }
    }
}
