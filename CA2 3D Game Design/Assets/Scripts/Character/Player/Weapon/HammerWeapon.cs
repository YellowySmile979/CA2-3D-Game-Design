using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerWeapon : BaseWeapon
{
    void OnTriggerEnter(Collider other)
    {
        //if u collide w/ enemy, deal dmg to enemy
        if (other.GetComponent<BaseEnemy>())
        {
            other.GetComponent<BaseEnemy>().TakeDamage(damage);
        }
    }
}
