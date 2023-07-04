using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbFireProjectile : BaseProjectile
{
    [Header("Rotation to Follow")]
    public Vector3 direction;

    //since this is a dumbfire projectile, it wwill travel in a straight line
    public override void ProjectileBehaviour()
    {        
        rb.velocity += direction * projectileMoveSpeed;        
    }
    //checks for collisions
    void OnTriggerEnter(Collider collision)
    {
        //if collide with enemy, destroy enemy and self
        //otherwise destroy self
        if (collision.gameObject.GetComponent<BaseEnemy>())
        {
            collision.gameObject.GetComponent<BaseEnemy>().TakeDamage(projectileDamage);
            Destroy(gameObject);
        }
    }
    //detects if hit floor
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
    }
}
