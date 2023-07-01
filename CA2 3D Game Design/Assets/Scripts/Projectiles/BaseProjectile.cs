using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    [Header("Data")]
    public ProjectileData projectileData;

    [Header("Projectile Movement")]
    public Rigidbody rb;
    public float projectileMoveSpeed, projectileRotationSpeed, projectileDamage;    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        projectileRotationSpeed = projectileData.rotationSpeed;
        projectileMoveSpeed = projectileData.moveSpeed;
        projectileDamage = projectileData.damage;
    }

    void FixedUpdate()
    {
        rb.velocity = transform.forward * projectileMoveSpeed;
        ProjectileBehaviour();
    }
    public virtual void ProjectileBehaviour()
    {

    }
}
