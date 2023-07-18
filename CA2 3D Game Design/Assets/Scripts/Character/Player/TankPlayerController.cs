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
    float waitTimeTillNextHeal;

    [Header("Tank's Abilities: Attract Enemies")]
    public float setWaitTillNextAttract = 5f;
    float waitTillNextAttract;

    public float setAttractDuration = 2f;
    float attractDuration;
    bool hasStartedAttract;
    public Collider[] enemyColliders;
    public float overlapSphereRadius = 5f;
    public LayerMask whatIsAnEnemy;

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
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            print("Heal");
            Heal(healAmount);
            //resets the cooldown until next heal
            waitTimeTillNextHeal = setWaitTimeTillNextHeal;
        }
        if (Input.GetKeyDown(KeyCode.E) && waitTillNextAttract <= 0)
        {
            print("Attract Enemies");
            hasStartedAttract = true;
            AttractEnemies();
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
            waitTimeTillNextHeal -= Time.deltaTime;
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
            waitTillNextAttract = setWaitTillNextAttract;
        }
        else
        {
            waitTillNextAttract -= Time.deltaTime;
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, overlapSphereRadius);
    }
}
