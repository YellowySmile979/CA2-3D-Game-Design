using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

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

    [Header("Related to Abilities")]
    public bool isAttracted = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        targettedObject = FindObjectOfType<GemObjective>().gameObject;

        enemyDamage = enemyData.damage;
        enemyMoveSpeed = enemyData.moveSpeed;
        enemyHealth = enemyData.health;

        timeUntilNextAttack = maxTimeUntilNextAttack;

        StartCoroutine(DetectObjects());
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
            //updates the amount of enemies killed
            if (FindObjectOfType<TankPlayerController>())
            {
                TankPlayerController tank = FindObjectOfType<TankPlayerController>();
                tank.EnemiesKilled(1);
            }
            if (FindObjectOfType<MagePlayerController>())
            {
                MagePlayerController mage = FindObjectOfType<MagePlayerController>();
                mage.EnemiesKilled(1);
            }

            //removes the enemy from the list of existing enemies
            WaveManager.Instance.existingEnemies.Remove(this);
            //enemy dies
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
    IEnumerator DetectObjects()
    {
        /*if (FindObjectOfType<TankPlayerController>() != null)
        {
            TankPlayerController tank = FindObjectOfType<TankPlayerController>();
            if (!tank.enemyColliders.Contains(this.GetComponent<BoxCollider>()))
            {
                isAttracted = false;
            }
        }*/
        //checks to see if enemy should be attracted to enemy
        while (true)
        {
            if (!isAttracted)
            {
                colliders = Physics.OverlapSphere(transform.position, overlapSphereRadius, whatCanISee);

                //checks to see if there are colliders within it
                if (colliders.Length > 0 && !isAttracted)
                {
                    //if there are, perform the following,
                    //check if the collider contains a portal objective, if yes, set targettedObject to that and break the loop
                    //otherwise set the targettedObject to the player if it is within
                    //if both, focus only on the portal tks to the break;
                    foreach (Collider collider in colliders)
                    {
                        if (collider.GetComponent<GemObjective>())
                        {
                            targettedObject = collider.gameObject;
                            break;
                        }
                        else if (collider.gameObject.GetComponent<BasePlayerController>())
                        {
                            targettedObject = collider.gameObject;
                        }
                    }
                }
                else
                {
                    //otherwise targets the portal
                    targettedObject = FindObjectOfType<GemObjective>().gameObject;
                }
            }
            else
            {
                targettedObject = FindObjectOfType<TankPlayerController>().gameObject;
            }

            yield return new WaitForSeconds(1f);
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
