using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemyScript : MonoBehaviour
{
    NavMeshAgent nmAgent;
    Animator anim;


    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        nmAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        nmAgent.SetDestination(target.position);
        anim.SetFloat("Move Y", nmAgent.velocity.magnitude / nmAgent.speed);


    }
}
