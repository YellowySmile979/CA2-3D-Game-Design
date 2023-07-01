using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "ProjectileData/ScriptableProjectile")]
public class ProjectileData : ScriptableObject
{
    public float moveSpeed;
    public float rotationSpeed;
    public float damage;
    public enum ProjectileType
    {
        dumbFire,
        homing
    }
    public ProjectileType projectileType;
}
