using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPlayerController : BasePlayerController
{
    [Header("Tank's Attack")]
    public GameObject weapon;
    [HideInInspector] public bool hasAttacked;

    [Header("Tank's Abilites: Heal")]
    public float healAmount;
    public float setWaitTimeTillNextHeal = 0.5f;
    public float waitTimeTillNextHeal;
    bool canHeal = true;

    [Header("Tank's Abilities: Attract Enemies")]
    public float setWaitTillNextAttract = 5f;
    public float waitTillNextAttract;
    bool canAttract = true;

    public float setAttractDuration = 2f;
    [SerializeField] float attractDuration;
    bool hasStartedAttract;
    public Collider[] enemyColliders;
    public float overlapSphereRadius = 5f;
    public LayerMask whatIsAnEnemy;

    [Header("Tank's Ultimate: Paralysing Strike")]
    public int enemiesKilled;
    public int requiredKills= 1;

    void Start()
    {
        attractDuration = setAttractDuration;
    }
    public override void Attack()
    {
        DetectEnemies();

        if (Input.GetMouseButtonDown(0))
        {
            PlayerAttackAnims();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canHeal)
        {
            print("Heal");
            //resets the cooldown until next heal
            waitTimeTillNextHeal = setWaitTimeTillNextHeal;

            Heal(healAmount);   
            
            //starts the display for the cooldown overlay
            //from left to right it's, player, which ability it is, if it's ability 1, 2 or ulti
            //type: this, (see CanvasController for which int), false if it's ability1 and true if not and do the same for rest
            StartCoroutine(CanvasController.Instance.DisplayCooldownTime(this, 0, false, true));

            //prevents the player from being able to spam the key
            canHeal = false;
        }
        if (Input.GetKeyDown(KeyCode.E) && canAttract)
        {
            print("Attract Enemies");
            hasStartedAttract = true;

            waitTillNextAttract = setWaitTillNextAttract;

            AttractEnemies();

            //starts the display for the cooldown overlay
            //from left to right it's, player, which ability it is, if it's ability 1 or 2
            //type: this, (see CanvasController for which int), false if it's ability1 and true if not and do the same for rest
            StartCoroutine(CanvasController.Instance.DisplayCooldownTime(this, 1, true, false));

            //prevents player form being able to spam the key
            canAttract = true;
        }
        if (Input.GetKeyDown(KeyCode.Q) && enemiesKilled >= requiredKills)
        {
            Ultimate();
        }
    }
    //handles the player's attack animation
    void PlayerAttackAnims()
    {
        if (weaponAnimator == null) weaponAnimator = GetComponentInChildren<Animator>();
        weaponAnimator.SetTrigger("isAttacking");
    }
    //handles the tank's healing ability
    void Heal(float healAmt)
    {
        if (waitTimeTillNextHeal <= 0)
        {
            if (playerHealth <= maxPlayerHealth)
            {
                playerHealth += healAmt;
            }
            else
            {
                playerHealth = maxPlayerHealth;
            }
        }
        else
        {
            StartCoroutine(HealTimer());
        }
    }
    //handles timer for heal
    IEnumerator HealTimer()
    {        
        bool hasCooledDown = false;
        while (!hasCooledDown)
        {
            if (hasCooledDown) break;

            waitTimeTillNextHeal -= Time.deltaTime;

            if (waitTimeTillNextHeal < 0f)
            {
                hasCooledDown = true;

                canHeal = true;
            }

            yield return new WaitForSeconds(0.0001f);
        }
    }
    //attracts enemies for awhile
    void AttractEnemies()
    {
        if(waitTillNextAttract <= 0)
        {
            if (hasStartedAttract)
            {
                //attract enemies
                foreach (Collider collider in enemyColliders)
                {
                    collider.GetComponent<BaseEnemy>().isAttracted = true;
                }
            }
        }
        else
        {
            StartCoroutine(AttractTimer());
        }
    }
    //handles the timer for attract
    IEnumerator AttractTimer()
    {
        bool hasCooledDown = false;
        while (!hasCooledDown)
        {
            if (hasCooledDown) break;

            waitTillNextAttract -= Time.deltaTime;

            if (waitTillNextAttract < 0f)
            {
                hasCooledDown = true;

                canAttract = true;
            }
            yield return new WaitForSeconds(0.0001f);
        }
    }
    //detects enemies
    void DetectEnemies()
    {
        enemyColliders = Physics.OverlapSphere(transform.position, overlapSphereRadius, whatIsAnEnemy);
        if (hasStartedAttract)
        {
            attractDuration -= Time.deltaTime;
        }
        else
        {
            attractDuration = setAttractDuration;
        }
        if(attractDuration <= 0)
        {
            hasStartedAttract = false;
        }
    }
    //activates the ultimate and also handle the ultimate charge
    void Ultimate()
    {
        CanvasController.Instance.ultimateTankOverlay.fillAmount = 1f;

        //checks to see if player can do ulti
        if (enemiesKilled == requiredKills)
        {
            //do ultimate
            print("ULTIIII");           
        }
        else if (enemiesKilled > requiredKills)
        {
            enemiesKilled = requiredKills;
        }
    }
    public void EnemiesKilled(int enemiesKill)
    {
        enemiesKilled += enemiesKill;

        //sends the info the CanvasController
        CanvasController.Instance.UltimateCharge(enemiesKilled, this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, overlapSphereRadius);
    }
}
