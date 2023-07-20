using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class BaseWeapon : MonoBehaviour
{
    public BasePlayerController playerController;
    float damage;

    public BoxCollider boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        if (playerController == null) playerController = GetComponentInParent<BasePlayerController>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
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
    //disables collider
    void DisableCollider()
    {
        print("Disables collider");
        boxCollider.enabled = false;
    }
    //enables collider
    void EnableCollider()
    {
        print("Enables collider");
        boxCollider.enabled = true;
    }
}
