using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagePlayerController : BasePlayerController
{
    [Header("Projectile")]
    public BaseProjectile projectile;
    public GameObject instantiationPoint;
    public float timeTillNextSpawn = 1f;
    public float coolDownBetweenAttacks = 2f;
    public Vector3 spawnOffset;
    float setTime;
    bool hasSetTime = false;

    [Header("Healer's Passive: Heal Over Time")]
    public float passiveHealthToHeal;
    public float setTimeTillStartHeal, timeTillStartHeal;
    [HideInInspector] public bool isBeingAttacked = false;

    [Header("Mage's Ability: Area Healing")]
    [SerializeField] Collider[] playerColliders;
    public GameObject originAreaOfEffect;
    public float overlapSphereRadius = 1f;
    public LayerMask whatIsAPlayer;

    public float setAreaHealingDuration = 3f;
    [SerializeField] float areaHealingDuration;
    [SerializeField] bool hasStartedAreaHealing;

    public float healAmount;

    public float setTimeTillNextPlacement, timeTillNextPlacement;
    public bool canAreaHeal = true;

    public GameObject healRing;
    bool isHealing;

    [Header("Mage's Ability: AOE Attack")]
    public float dmgMultiplier;
    public GameObject circleOfFire;
    public Transform spawnArea;

    public float setTimeTillNextFire, timeTillNextFire;
    bool canFire = true;

    [Header("Mage's Ultimate: Revive")]
    public float healthToGive;
    public GameObject allyToRevive;

    public int enemiesKilled;
    public int requiredKills = 1;

    public GameObject ultiBall;
    public Transform ultiBallSpawnArea;
    public float ultiBallMoveSpeed = 5f, maxUltiBallSummonWaitTime = 2f, ultiBallSummonWaitTime;
    public float maxUltiBallMoveWaitTime = 2f, ultiBallMoveWaitTime;
    [SerializeField] Transform ultiBallSpawnLocation;
    public float maxHeightReachable = 10f;

    public float waitBeforeActivateUlti = 2f;

    bool hasUltied;
    bool hasPlayed = true;

    [Header("Mage specific SFX")]
    public AudioClip baseAttackSFX;
    public AudioClip areaHealingSFX, aoeAttackSFX, mageUltSFX;

    void Start()
    {
        timeTillStartHeal = setTimeTillStartHeal;
        ultiBallSummonWaitTime = maxUltiBallSummonWaitTime;
        ultiBallMoveWaitTime = maxUltiBallMoveWaitTime;
        ultiBallSpawnLocation = ultiBallSpawnArea.transform;

        if(allyToRevive == null)
        allyToRevive = FindObjectOfType<BasePlayerController>().gameObject;
    }

    //has been overriden with homing projectile attack
    public override void Attack()
    {
        PassiveHeal();
        DetectPlayers();

        //sets the spawn time
        if (!hasSetTime)
        {
            setTime = timeTillNextSpawn;
            hasSetTime = true;
        }
        //checks to see if player can fire, if not then minus the time
        if (Input.GetAxisRaw("Fire1 " + whichPlayer.ToString()) > 0.1 && timeTillNextSpawn <= 0 && hasPlayed == true && canMove)
        {
            hasPlayed = false;
            if(baseAttackSFX != null)
            audioSource.PlayOneShot(baseAttackSFX);
            playerAnimator.SetTrigger("isAttacking");
            Invoke("EnableAttack", coolDownBetweenAttacks);

            
            projectile.GetComponent<HomingProjectile>().floor = GameObject.FindWithTag("Floor");

            Instantiate(projectile, instantiationPoint.transform.position + spawnOffset, Quaternion.identity);
            projectile.GetComponent<HomingProjectile>().target = FindObjectOfType<BaseEnemy>();

            timeTillNextSpawn = setTime;
        }
        else if (timeTillNextSpawn > 0)
        {
            timeTillNextSpawn -= Time.deltaTime;
        }
        if (Input.GetKeyDown("joystick button 4") && canAreaHeal)
        {
            print("Area Healing");
            if(areaHealingSFX != null)
            audioSource.PlayOneShot(areaHealingSFX);

            playerAnimator.SetTrigger("ABL_Heal");
            hasStartedAreaHealing = true;
            isHealing = true;
            if (isHealing) healRing.SetActive(true);
            AreaHealing();

            timeTillNextPlacement = setTimeTillNextPlacement;
            AreaHealing();

            //starts the display for the cooldown overlay
            //from left to right it's, player, which ability it is, if it's ability 1, 2 or ulti
            //type: this, (see CanvasController for which int), false if it's ability1 and true if not and do the same for rest
            StartCoroutine(CanvasController.Instance.DisplayCooldownTime(this, 0, false, true));

            canAreaHeal = false;
        }
        if (Input.GetKeyDown("joystick button 5") && canFire)
        {
            print("AOE Attack");
            playerAnimator.SetTrigger("ABL_Ring");
            audioSource.PlayOneShot(aoeAttackSFX);

            AOEAttack();

            timeTillNextFire = setTimeTillNextFire;
            AOEAttack();

            StartCoroutine(CanvasController.Instance.DisplayCooldownTime(this, 1, true, false));

            canFire = false;
        }
        if (Input.GetKeyDown("joystick button 1") && enemiesKilled >= requiredKills && !hasUltied)
        {
            print("Mage Ultimate");
            ultiBallSummonWaitTime = maxUltiBallSummonWaitTime;
            ultiBallMoveWaitTime = maxUltiBallMoveWaitTime;
            audioSource.PlayOneShot(mageUltSFX);
            StartCoroutine(WaitToActivateUlti());
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            enemiesKilled = 10;
            TankPlayerController tank = FindObjectOfType<TankPlayerController>();
            tank.playerHealth = 0;
            EnemiesKilled(0);
        }
    }
    //handles the passive trait of the healer
    public void PassiveHeal()
    {
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
            if (hasStartedAreaHealing)
            {
                foreach(Collider collider in playerColliders)
                {
                    collider.GetComponent<BasePlayerController>().playerHealth += healAmount;
                }
            }
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

                canAreaHeal = true;
            }

            yield return new WaitForSeconds(0.00001f);
        }       
    }
    //detects all players within the sphere
    void DetectPlayers()
    {
        playerColliders = Physics.OverlapSphere(originAreaOfEffect.transform.position, overlapSphereRadius, whatIsAPlayer);
        if (hasStartedAreaHealing)
        {
            areaHealingDuration -= Time.deltaTime;
        }
        else
        {
            areaHealingDuration = setAreaHealingDuration;
        }
        if (areaHealingDuration <= 0)
        {
            hasStartedAreaHealing = false;

            healRing.SetActive(false);
            isHealing = false;
        }
    }
    //handles the AOE attack of the mage (summoning of the fire)
    public void AOEAttack()
    {
        if(timeTillNextFire <= 0f)
        {
            //activates the AOE attack
            Instantiate(circleOfFire, spawnArea.transform.position, Quaternion.identity);
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

            timeTillNextFire -= Time.deltaTime;

            if (timeTillNextFire < 0f)
            {
                hasCooledDown = true;

                canFire = true;
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
            TankPlayerController tank = FindObjectOfType<TankPlayerController>(true);
            Transform tankCurrentPosition;
            if(tank.meshesToDeactivate[0].activeSelf == false)
            {
                allyToRevive = tank.gameObject;
            }
            else
            {
                allyToRevive = this.gameObject;
            }

            if(allyToRevive != null)
            {
                //revive ally
                tankCurrentPosition = tank.gameObject.transform;
                tank.timeTillNextPlayerSpawn = 0f;
                tank.transform.position = tankCurrentPosition.position;
            }
            else if(allyToRevive != this)
            {
                //heal ally and yourself
                allyToRevive.GetComponent<BasePlayerController>().playerHealth += healthToGive;
                tank.playerHealth += healthToGive;
            }
            hasUltied = false;
            enemiesKilled = 0;
        }
        else if (enemiesKilled > requiredKills)
        {
            enemiesKilled = requiredKills;
        }
    }
    //handles the enemies killed for the ultimate
    public void EnemiesKilled(int enemiesKill)
    {
        enemiesKilled += enemiesKill;

        //sends the info the CanvasController
        CanvasController.Instance.UltimateCharge(enemiesKilled, this);
    }
    //enables the attack (is invoked)
    void EnableAttack()
    {
        hasPlayed = true;
    }
    IEnumerator WaitToActivateUlti()
    {
        playerAnimator.SetTrigger("Mage_Ult");
        hasUltied = true;
        CanvasController.Instance.ultimateMageOverlay.fillAmount = 1f;
        //waits for the mage to put her hands together
        while (ultiBallSummonWaitTime > 0)
        {
            ultiBallSummonWaitTime -= Time.deltaTime;
            if(ultiBallSummonWaitTime <= 0)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        //summons ball
        GameObject summonedUltiBall = Instantiate(ultiBall, ultiBallSpawnArea.transform.position, Quaternion.identity);

        while(ultiBallMoveWaitTime > 0)
        {
            ultiBallMoveWaitTime -= Time.deltaTime;
            if (ultiBallMoveSpeed <= 0)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        while(summonedUltiBall.transform.position.y < ultiBallSpawnLocation.position.y + maxHeightReachable)
        {
            print(ultiBallSpawnLocation.position.y);
            summonedUltiBall.transform.position += new Vector3(0, ultiBallMoveSpeed, 0) * Time.deltaTime;
            if(summonedUltiBall.transform.position.y > ultiBallSpawnLocation.position.y + maxHeightReachable)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(waitBeforeActivateUlti);

        Destroy(summonedUltiBall);

        MageUltimate();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(originAreaOfEffect.transform.position, overlapSphereRadius);
    }
}
