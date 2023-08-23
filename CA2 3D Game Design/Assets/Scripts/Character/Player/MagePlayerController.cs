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

    [Header("Mage's Ability: AOE Attack")]
    public float dmgMultiplier;
    public GameObject circleOfFire;

    public float setTimeTillNextFire, timeTillNextFire;

    [Header("Mage's Ultimate: Revive")]
    public float healthToGive;
    public GameObject allyToRevive;

    public int enemiesKilled;
    public int requiredKills = 1;

    bool hasPlayed = true;

    void Start()
    {
        timeTillStartHeal = setTimeTillStartHeal;

        if(allyToRevive == null)
        allyToRevive = FindObjectOfType<BasePlayerController>().gameObject;
    }

    //has been overriden with homing projectile attack
    public override void Attack()
    {
        if (allyToRevive == this) allyToRevive = FindObjectOfType<BasePlayerController>().gameObject;

        PassiveHeal();
        DetectPlayers();

        //sets the spawn time
        if (!hasSetTime)
        {
            setTime = timeTillNextSpawn;
            hasSetTime = true;
        }
        //checks to see if player can fire, if not then minus the time
        if (Input.GetAxisRaw("Fire1 " + whichPlayer.ToString()) > 0.1 && timeTillNextSpawn <= 0 && hasPlayed == true)
        {
            hasPlayed = false;
            playerAnimator.SetTrigger("isAttacking");
            Invoke("EnableAttack", coolDownBetweenAttacks);

            projectile.GetComponent<HomingProjectile>().target = FindObjectOfType<BaseEnemy>();
            projectile.GetComponent<HomingProjectile>().floor = GameObject.FindWithTag("Floor");

            Instantiate(projectile, instantiationPoint.transform.position + spawnOffset, Quaternion.identity);
            timeTillNextSpawn = setTime;
        }
        else if (timeTillNextSpawn > 0)
        {
            timeTillNextSpawn -= Time.deltaTime;
        }
        if (Input.GetAxisRaw("Fire2 " + whichPlayer.ToString()) > 0.1 && canAreaHeal)
        {
            print("Area Healing");
            playerAnimator.SetTrigger("ABL_Heal");
            hasStartedAreaHealing = true;

            AreaHealing();

            timeTillNextPlacement = setTimeTillNextPlacement;
            AreaHealing();

            //starts the display for the cooldown overlay
            //from left to right it's, player, which ability it is, if it's ability 1, 2 or ulti
            //type: this, (see CanvasController for which int), false if it's ability1 and true if not and do the same for rest
            StartCoroutine(CanvasController.Instance.DisplayCooldownTime(this, 0, false, true));

            canAreaHeal = false;
        }
        if (Input.GetAxisRaw("Fire3 " + whichPlayer.ToString()) > 0.1)
        {
            print("AOE Attack");
            playerAnimator.SetTrigger("ABL_Ring");

            AOEAttack();
        }
        if (Input.GetAxisRaw("Ultimate " + whichPlayer.ToString()) > 0.1 && enemiesKilled >= requiredKills)
        {
            print("Mage Ultimate");
            playerAnimator.SetTrigger("Mage_Ult");

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
        }
    }
    //handles the AOE attack of the mage (summoning of the meteor)
    public void AOEAttack()
    {
        if(timeTillNextFire <= 0f)
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

            timeTillNextFire -= Time.deltaTime;

            if (timeTillNextFire < 0f)
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

            if(allyToRevive != null)
            {
                //revive ally
            }
            else if(allyToRevive != this)
            {
                //heal ally
                allyToRevive.GetComponent<BasePlayerController>().playerHealth += healthToGive;
            }
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(originAreaOfEffect.transform.position, overlapSphereRadius);
    }
}
