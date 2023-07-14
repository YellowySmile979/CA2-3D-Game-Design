using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class BaseWeapon : MonoBehaviour
{
    public BasePlayerController playerController;
    float damage;

    // Start is called before the first frame update
    void Start()
    {
        if (playerController == null) playerController = GetComponentInParent<BasePlayerController>();       
    }

    // Update is called once per frame
    void Update()
    {
        HandleDamage();
    }
    //handles the damage value of the weapon
    void HandleDamage()
    {
        damage = playerController.playerDamage;
    }
    void OnTriggerEnter(Collider other)
    {
        //if u collide w/ enemy, deal dmg to enemy
        if (other.GetComponent<BaseEnemy>())
        {
            other.GetComponent<BaseEnemy>().TakeDamage(damage);
        }
    }
}
