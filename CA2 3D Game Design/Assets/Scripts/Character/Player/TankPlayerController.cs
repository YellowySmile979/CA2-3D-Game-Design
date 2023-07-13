using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPlayerController : BasePlayerController
{
    [Header("Tank")]
    public float setWaitTimeTillNextAttack = 0.5f;
    float waitTimeTillNextAttack;
    bool isAttacking, hasAttacked;

    void Start()
    {
        waitTimeTillNextAttack = setWaitTimeTillNextAttack;
    }
    public override void Attack()
    {
        
    }
}
