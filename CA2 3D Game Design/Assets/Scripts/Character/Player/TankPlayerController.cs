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
    [SerializeField] bool hasStartedAttract;
    public Collider[] enemyColliders;
    public float overlapSphereRadius = 5f;
    public LayerMask whatIsAnEnemy;

    [Header("Tank's Ultimate: Paralysing Strike")]
    public int enemiesKilled;
    public int requiredKills = 1;


    //sorry elijah arrange it later
    bool playOnce = true;

    void Start()
    {
        attractDuration = setAttractDuration;
    }
    public override void Attack()
    {
        DetectEnemies();

        // to elijah: the input not working 
        if (Input.GetButton("Fire1 " + whichPlayer.ToString()) && playOnce == true)
        {
            playOnce = false;
            PlayerAttackAnims();
            Debug.Log("smth");
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canHeal)
        {
            print("Heal");
            Heal(healAmount);

            //play heal anim
                playerAnimator.SetTrigger("ABL_Heal");
                

            //resets the cooldown until next heal
            waitTimeTillNextHeal = setWaitTimeTillNextHeal;
            Heal(0);

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
            playerAnimator.SetTrigger("ABL_Attract");
            hasStartedAttract = true;

            AttractEnemies();

            waitTillNextAttract = setWaitTillNextAttract;
            AttractEnemies();

            //starts the display for the cooldown overlay
            //from left to right it's, player, which ability it is, if it's ability 1 or 2
            //type: this, (see CanvasController for which int), false if it's ability1 and true if not and do the same for rest
            StartCoroutine(CanvasController.Instance.DisplayCooldownTime(this, 1, true, false));

            //prevents player form being able to spam the key
            canAttract = false;
        }

        if (Input.GetKeyDown(KeyCode.Q) && enemiesKilled >= requiredKills)
        {
            TankUltimate();
            playerAnimator.SetTrigger("Tank_Ult");
        }
    }
    //handles the player's attack animation
    void PlayerAttackAnims()
    {
        playerAnimator = GetComponent<Animator>();
        Debug.Log(playerAnimator);
        playerAnimator.SetTrigger("isAttacking");
        Invoke("EnableAttack", 2);
        // the 2s is the cooldown for the attack (EDIT ACCORDINGLY)
    }
    //handles the tank's healing ability
    void Heal(float healAmt)
    {
        if (waitTimeTillNextHeal <= 0)
        {
            if (playerHealth < maxPlayerHealth)
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
            //stop attracting enemies
            foreach (Collider collider in enemyColliders)
            {
                collider.GetComponent<BaseEnemy>().isAttracted = false;
            }
        }
    }
    //activates the ultimate and also handle the ultimate charge
    void TankUltimate()
    {
        CanvasController.Instance.ultimateTankOverlay.fillAmount = 1f;

        //checks to see if player can do ulti
        if (enemiesKilled == requiredKills)
        {
            //do ultimate
            print("TANK ULTIIII");           
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

    //sorry elijah
    void EnableAttack()
    {
        playOnce = true;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, overlapSphereRadius);
    }
}
