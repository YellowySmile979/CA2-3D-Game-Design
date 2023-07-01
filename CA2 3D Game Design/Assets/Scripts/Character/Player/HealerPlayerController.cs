using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerPlayerController : BasePlayerController
{
    [Header("Projectile")]
    public BaseProjectile projectile;
    public GameObject instantiationPoint;
    public float timeTillNextSpawn = 1f;
    public Vector3 spawnOffset;
    float setTime;
    bool hasSetTime = false;

    //has been overriden with homing projectile attack
    public override void Attack()
    {
        //sets the spawn time
        if (!hasSetTime)
        {
            setTime = timeTillNextSpawn;
            hasSetTime = true;
        }
        //checks to see if player can fire, if not then minus the time
        if (Input.GetMouseButtonDown(0) && timeTillNextSpawn <= 0)
        {
            projectile.GetComponent<HomingProjectile>().target = FindObjectOfType<BaseEnemy>();
            projectile.GetComponent<HomingProjectile>().floor = GameObject.FindWithTag("Floor");

            Instantiate(projectile, instantiationPoint.transform.position + spawnOffset, Quaternion.identity);
            timeTillNextSpawn = setTime;
        }
        else if (timeTillNextSpawn > 0)
        {
            timeTillNextSpawn -= Time.deltaTime;
        }
    }
}
