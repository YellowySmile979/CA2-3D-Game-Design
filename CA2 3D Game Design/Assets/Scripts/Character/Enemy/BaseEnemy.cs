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
    public GameObject targettedObject;
    public NavMeshAgent enemy;

    [Header("Deal Damage")]
    public float maxTimeUntilNextAttack;
    [SerializeField] float timeUntilNextAttack;
    public bool canAttack;

    [Header("Detect Sphere")]
    public float overlapSphereRadius = 1f;
    public LayerMask whatCanISee;
    [SerializeField] Collider[] colliders;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        targettedObject = FindObjectOfType<PortalObjective>().gameObject;

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
        DetectObjects();
    }
    //moves enemy in direction of player
    void Move()
    {
        enemy.SetDestination(targettedObject.transform.position);
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
            WaveManager.Instance.existingEnemies.Remove(this);
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
    //detects the player and causes enemy to pathfind instead to the player than the target
    void DetectObjects()
    {
        colliders = Physics.OverlapSphere(transform.position, overlapSphereRadius, whatCanISee);
        foreach(Collider collider in colliders)
        {
            if (collider.gameObject.GetComponentInParent<BasePlayerController>())
            {
                targettedObject = collider.gameObject;
            }
        }
        if (colliders.Length <= 0)
        {
            targettedObject = FindObjectOfType<PortalObjective>().gameObject;
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, overlapSphereRadius);
    }
}
