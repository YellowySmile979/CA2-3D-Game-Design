using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPlayerController : BasePlayerController
{
    [Header("Tank's Attack")]
    public GameObject weapon;
    public float setWaitTimeTillNextAttack = 0.5f;
    float waitTimeTillNextAttack;
    [HideInInspector] public bool isAttacking, hasAttacked;

    [Header("Tank's Abilites")]
    public float healAmount;
    public float setWaitTimeTillNextHeal = 0.5f;
    float waitTimeTillNextHeal;

    void Start()
    {
        waitTimeTillNextAttack = setWaitTimeTillNextAttack;
        waitTimeTillNextHeal = setWaitTimeTillNextHeal;
    }
    public override void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayerAttackAnims();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Heal(healAmount);
        }
    }
    //handles the player's attack animation
    void PlayerAttackAnims()
    {
        isAttacking = true;
        if (weaponAnimator == null) weaponAnimator = GetComponentInChildren<Animator>();
        weaponAnimator.SetBool("isAttacking", isAttacking);
        if(weaponAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            isAttacking = false;
            weaponAnimator.SetBool("isAttacking", isAttacking);
        }
    }
    //handles the tank's healing ability
    void Heal(float healAmt)
    {
        if (playerHealth <= maxPlayerHealth)
        {
            playerHealth += healAmt;
        }
        else
        {
            playerHealth = maxPlayerHealth;
        }
    }
}
