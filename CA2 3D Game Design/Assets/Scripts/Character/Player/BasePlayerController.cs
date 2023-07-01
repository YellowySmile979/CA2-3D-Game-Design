using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayerController : MonoBehaviour
{
    [Header("Data")]
    public CharacterData playerData;

    [Header("Player")]
    float horizontalInput, verticalInput, playerSpeed, playerHealth, playerDamage;
    public GameObject player;

    [Header("Camera")]
    public Camera cam;

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
        playerHealth = playerData.health;
        playerDamage = playerData.damage;
    }

    // Update is called once per frame
    void Update()
    {
        //get player input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        Move();
        Attack();
        UpdateHealth();
        RotatePlayer();
    }
    //handles the player movement
    void Move()
    {
        //moves player
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed * verticalInput);
        transform.Translate(Vector3.right * Time.deltaTime * playerSpeed * horizontalInput);
    }
    void RotatePlayer()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            player.transform.LookAt(target);            
        }
    }
    //performs the attack of the player
    public virtual void Attack()
    {
        
    }
    //allows palyer to take damage
    public void TakeDamage(float damage)
    {
        playerHealth -= damage;
    }
    //updates the health of the player
    public void UpdateHealth()
    {
        //print("Player Health: " + playerHealth);
        if (playerHealth <= 0)
        {
            //player dies
            print("Player died");
        }
    }
}
