using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankUlt : MonoBehaviour
{
    public float lifetime = 2.2f;
    [SerializeField] float damage;

    public Collider[] enemyColliders;
    public LayerMask whatIsAnEnemy;

    // Start is called before the first frame update
    void Start()
    {
        //updates damage accordingly
        if (FindObjectOfType<TankPlayerController>())
        {
            TankPlayerController tankPlayerController = FindObjectOfType<TankPlayerController>();
            damage = tankPlayerController.playerDamage * tankPlayerController.dmgMultiplier;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectEnemies();

        //handles how long the fire should remain
        if (lifetime >= 0)
        {
            lifetime -= Time.deltaTime;
        }
        else
        {
            foreach (Collider collider in enemyColliders)
            {
                collider.GetComponent<BaseEnemy>().enemy.speed = collider.GetComponent<BaseEnemy>().enemyData.moveSpeed;
            }
            Destroy(this.gameObject);
        }
    }
    void DetectEnemies()
    {
        enemyColliders = Physics.OverlapBox(transform.position, new Vector3(4.25f, 3.7f, 1.4f),Quaternion.identity, whatIsAnEnemy);

        foreach(Collider collider in enemyColliders)
        {
            collider.GetComponent<BaseEnemy>().enemy.speed = 0;
        }
    }
    //makes any enemy that steps on it take damage
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BaseEnemy>())
        {
            other.GetComponent<BaseEnemy>().TakeDamage(damage);
        }
    }
}
