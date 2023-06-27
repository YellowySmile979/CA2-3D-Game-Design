using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    float horizontal, vertical;

    public GameObject player;

    //a singleton
    public static PlayerController Instance;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    void Move()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.position = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        }
    }
}
