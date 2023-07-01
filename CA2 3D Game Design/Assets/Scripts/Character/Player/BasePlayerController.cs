using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayerController : MonoBehaviour
{
    [Header("Data")]
    public CharacterData playerData;

    [Header("Player")]
    float horizontalInput, verticalInput, playerSpeed;
    public GameObject player;

    //a singleton
    public static BasePlayerController Instance;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerSpeed = playerData.moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //get player input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        Move();
        Attack();
    }
    //handles the player movement
    void Move()
    {
        //moves player
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed * verticalInput);
        transform.Translate(Vector3.right * Time.deltaTime * playerSpeed * horizontalInput);
    }
    //performs the attack of the player
    public virtual void Attack()
    {
        
    }
}
