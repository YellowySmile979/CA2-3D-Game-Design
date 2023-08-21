using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class BasePlayerController : MonoBehaviour
{
    [Header("Data")]
    public CharacterData playerData;

    [Header("Player")]
    [HideInInspector] public float horizontalInput, verticalInput;
    public float playerSpeed, playerHealth, playerDamage;
    public float maxPlayerHealth;    
    public GameObject player;
    public float rotationSpeed = 200f;

    public enum Player
    {
        P1,
        P2
    }

    public Player whichPlayer;

    [Header("Camera")]
    public Camera cam;

    [Header("Level")]
    public float level = 1;
    [SerializeField] float healthScaleFactor, dmgScaleFactor, speedScaleFactor;
    public bool hasLevelledUp;

    [Header("Animator")]
    public Animator playerAnimator;
    public Animator weaponAnimator;

    //a singleton
    public static BasePlayerController Instance;

    void Awake()
    {
        Instance = this;
        playerHealth = playerData.health;
        playerSpeed = playerData.moveSpeed;
        playerDamage = playerData.damage;
        maxPlayerHealth = playerHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
        {
            cam = FindObjectOfType<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //get player input
        horizontalInput = Input.GetAxis("Horizontal " + whichPlayer.ToString());
        verticalInput = Input.GetAxis("Vertical " + whichPlayer.ToString());
        Move();
        Attack();
        UpdateHealth();
        //replace this with movement anims: HandlePlayerAnims()
        if (whichPlayer == Player.P1) RotatePlayerMouse();
        else if (whichPlayer == Player.P2) RotatePlayerJoystick();

        CanvasController.Instance.UpdatePlayerStats(this);
    }
    //handles the player movement
    void Move()
    {
        //moves player
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed * verticalInput);
        transform.Translate(Vector3.right * Time.deltaTime * playerSpeed * horizontalInput);
    }
    //handles rotation but for mouse
    void RotatePlayerMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            player.transform.LookAt(target);            
        }
    }
    //handles rotation but for a controller
    void RotatePlayerJoystick()
    {
        if(Input.GetAxisRaw("Mouse X " + whichPlayer.ToString()) > 0.1)
        {
            player.transform.Rotate(0, Input.GetAxis("Mouse X " + whichPlayer.ToString()) * rotationSpeed * Time.deltaTime, 0);
        }
    }
    //performs the attack of the player
    public abstract void Attack();

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
        //helps ensure the health never exceeds the max player health
        if (playerHealth >= maxPlayerHealth)
        {
            playerHealth = maxPlayerHealth;
        }

        //sets the new health when player levels up
        /*if (!hasLevelledUp)
        {
            maxPlayerHealth += 1;
            hasLevelledUp = true;
        }*/
    }
    //handles player anims
    public void HandlePlayerAnims()
    {
        if (playerAnimator == null) playerAnimator = GetComponent<Animator>();
        playerAnimator.SetFloat("MoveX", playerSpeed);
        playerAnimator.SetFloat("MoveZ", playerSpeed);
    }
    //handles the levelling up of the player
    public void LevelUp(float playerLevel)
    {
        level = playerLevel;

        //increases health when player levels up by adding their health by scalefactor times level
        playerHealth = playerHealth + (healthScaleFactor * level);
        maxPlayerHealth = maxPlayerHealth + (healthScaleFactor * level);

        //increases speed when player levels up by adding their speed by scalefactor times level
        playerSpeed = playerSpeed + (speedScaleFactor * level);

        //increases dmg when player levels up by adding their dmg by scalefactor times level
        playerDamage = playerDamage + (dmgScaleFactor * level);

        hasLevelledUp = true;
    }
}
