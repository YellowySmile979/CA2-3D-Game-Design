using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Data")]
    public CharacterData enemyData;

    [Header("Movement")]
    public Rigidbody rb;
    [SerializeField] float enemyMoveSpeed, enemyDamage, enemyHealth;    
    public Rigidbody Rb => rb;

    [Header("Target")]
    public BasePlayerController targettedPlayer;
    public NavMeshAgent enemy;

    [Header("Deal Damage")]
    public float maxTimeUntilNextAttack;
    [SerializeField] float timeUntilNextAttack;
    public bool canAttack;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        targettedPlayer = FindObjectOfType<BasePlayerController>();

        enemyDamage = enemyData.damage;
        enemyMoveSpeed = enemyData.moveSpeed;
        enemyHealth = enemyData.health;

        timeUntilNextAttack = maxTimeUntilNextAttack;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Timer();
        UpdateHealth();
    }
    //moves enemy in direction of player
    void Move()
    {
        enemy.SetDestination(targettedPlayer.transform.position);
        enemy.speed = enemyMoveSpeed;
    }
    //handles the enemy taking damage
    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
    }
    //updates enemy health
    public void UpdateHealth()
    {
        if(enemyHealth <= 0)
        {
            print("enemy died");
            Destroy(gameObject);
        }
    }
    //helps time between when is the next attack
    void Timer()
    {
        if (timeUntilNextAttack > 0)
        {
            timeUntilNextAttack -= Time.deltaTime;
            canAttack = false;
        }
        else
        {
            canAttack = true;
        }
    }
    //checks to see if collided collider is player and enemy can attack
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<BasePlayerController>() && canAttack)
        {
            collision.gameObject.GetComponentInParent<BasePlayerController>().TakeDamage(enemyDamage);
            timeUntilNextAttack = maxTimeUntilNextAttack;
        }
    }
    //checks to see if collided collider is player and enemy can attack and continues to do so
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<BasePlayerController>() && canAttack)
        {
            collision.gameObject.GetComponentInParent<BasePlayerController>().TakeDamage(enemyDamage);
            timeUntilNextAttack = maxTimeUntilNextAttack;
        }
    }
}
