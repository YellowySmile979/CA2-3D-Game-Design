using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagePlayerController : BasePlayerController
{
    [Header("Projectile")]
    public BaseProjectile projectile;
    public GameObject instantiationPoint;
    public float timeTillNextSpawn = 1f;
    public Vector3 spawnOffset;
    float setTime;
    bool hasSetTime = false;

    [Header("Healer's Passive: Heal Over Time")]
    public float passiveHealthToHeal;
    public float setTimeTillStartHeal, timeTillStartHeal;

    [Header("Mage's Ability: Area Healing")]
    public GameObject areaOfEffect;
    public float healAmount;

    public float setTimeTillNextPlacement, timeTillNextPlacement;

    [Header("Mage's Ability: AOE Attack")]
    public float dmgMultiplier;
    public GameObject meteor;

    public float setTimeTillNextMeteor, timeTillNextMeteor;

    [Header("Mage's Ultimate: Revive")]
    public float healthToGive;
    public GameObject allyToRevive;

    public int enemiesKilled;
    public int requiredKills = 1;

    void Start()
    {
        timeTillStartHeal = setTimeTillStartHeal;
    }

    //has been overriden with homing projectile attack
    public override void Attack()
    {
        PassiveHeal();

        //sets the spawn time
        if (!hasSetTime)
        {
            setTime = timeTillNextSpawn;
            hasSetTime = true;
        }
        //checks to see if player can fire, if not then minus the time
        if (Input.GetMouseButtonDown(0) && timeTillNextSpawn <= 0)
        {
            projectile.GetComponent<HomingProjectile>().target = FindObjectOfType<BaseEnemy>();
            projectile.GetComponent<HomingProjectile>().floor = GameObject.FindWithTag("Floor");

            Instantiate(projectile, instantiationPoint.transform.position + spawnOffset, Quaternion.identity);
            timeTillNextSpawn = setTime;
        }
        else if (timeTillNextSpawn > 0)
        {
            timeTillNextSpawn -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            AreaHealing();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AOEAttack();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            MageUltimate();
        }
    }
    //handles the passive trait of the healer
    public void PassiveHeal()
    {
        bool isBeingAttacked = false;
        //if player isnt being attacked, time will be reduced till zero then player can passively heal
        //otherwise, resets the timer constantly until player gets out of combat
        if (isBeingAttacked)
        {
            timeTillStartHeal = setTimeTillStartHeal;
        }
        else
        {
            if (timeTillStartHeal <= 0f)
            {
                if (playerHealth < maxPlayerHealth)
                {
                    playerHealth += passiveHealthToHeal;
                }
                else
                {
                    playerHealth = maxPlayerHealth;
                }
            }
            else
            {
                timeTillStartHeal -= Time.deltaTime;
            }
        }
    }
    //handles the area healing of the healer
    public void AreaHealing()
    {
        if(timeTillNextPlacement <= 0f)
        {
            //activate the area healing part
        }
        else
        {
            StartCoroutine(AreaHealingTimer());
        }
    }
    //handles the timer for the area healing
    IEnumerator AreaHealingTimer()
    {
        bool hasCooledDown = false;
        while (!hasCooledDown)
        {
            if (hasCooledDown) break;

            timeTillNextPlacement -= Time.deltaTime;

            if (timeTillNextPlacement < 0f)
            {
                hasCooledDown = true;
            }

            yield return new WaitForSeconds(0.00001f);
        }       
    }
    //handles the AOE attack of the mage (summoning of the meteor)
    public void AOEAttack()
    {
        if(timeTillNextMeteor <= 0f)
        {
            //activates the AOE attack
        }
        else
        {
            StartCoroutine(AOEAttackTimer());
        }
    }
    //handles the timer for the AOE attack
    IEnumerator AOEAttackTimer()
    {
        bool hasCooledDown = false;
        while (!hasCooledDown)
        {
            if (hasCooledDown) break;

            timeTillNextMeteor -= Time.deltaTime;

            if (timeTillNextMeteor < 0f)
            {
                hasCooledDown = true;
            }

            yield return new WaitForSeconds(0.00001f);
        }
    }
    //handles the mage's ultimate (REVIVE)
    public void MageUltimate()
    {
        CanvasController.Instance.ultimateMageOverlay.fillAmount = 1f;

        //checks to see if player can do ulti
        if (enemiesKilled == requiredKills)
        {
            //do ultimate
            print("MAGE ULTIIII");
        }
        else if (enemiesKilled > requiredKills)
        {
            enemiesKilled = requiredKills;
        }
    }
}
