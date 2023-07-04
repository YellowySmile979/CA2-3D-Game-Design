using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class BaseProjectile : MonoBehaviour
{
    [Header("Data")]
    public ProjectileData projectileData;

    [Header("Projectile Movement")]
    public Rigidbody rb;
    public float projectileMoveSpeed = 0, projectileRotationSpeed = 0, projectileDamage = 0;

    [Header("Destroy Self After Awhile")]
    public float lifeTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        projectileRotationSpeed += projectileData.rotationSpeed;
        projectileMoveSpeed += projectileData.moveSpeed;
        projectileDamage += projectileData.damage;
    }

    void FixedUpdate()
    {        
        ProjectileBehaviour();
        DestroySelf();
    }
    //this is to allow for the child scripts to be able to input their own attacks
    public virtual void ProjectileBehaviour()
    {
        rb.velocity = transform.forward * projectileMoveSpeed;
    }
    //self explanatory
    void DestroySelf()
    {
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
