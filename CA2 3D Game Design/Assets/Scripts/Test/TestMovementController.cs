using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovementController : MonoBehaviour
{
    //this script is to test animations by making the player actually move
    CharacterController charC;
    Animator anim;

    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;
    Vector3 velocity;

    public enum Player { P1, P2 };
    //only accessible in this class
    public Player player;
    //defines the varible

    // Start is called before the first frame update
    void Start()
    {
        charC = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal " + player.ToString()), 0, Input.GetAxis("Vertical " + player.ToString()));

        Debug.Log(Input.GetAxis("Horizontal"));
        Debug.Log(Input.GetAxis("Vertical"));
        Debug.Log(movement);
        anim.SetFloat("Move X", movement.x);
        anim.SetFloat("Move Y", movement.z);

        Vector3 displacement = transform.TransformDirection(movement.normalized) * moveSpeed;
        charC.Move((displacement + velocity) * Time.deltaTime);
        transform.Rotate(0, Input.GetAxis("Mouse X " + player.ToString()) * rotationSpeed * Time.deltaTime, 0);


    }

}

