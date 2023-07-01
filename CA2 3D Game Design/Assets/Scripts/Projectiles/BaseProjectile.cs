using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    [Header("Data")]
    public ProjectileData homingProjectileData;

    [Header("Projectile Movement")]
    public float projectileMoveSpeed;
    public float projectileRotationSpeed;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        projectileRotationSpeed = homingProjectileData.rotationSpeed;
        projectileMoveSpeed = homingProjectileData.moveSpeed;
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
