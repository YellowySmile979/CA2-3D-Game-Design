using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public Vector3 targetOffset;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
        {
            target = FindObjectOfType<BasePlayerController>().gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }
    //follows the player
    void FollowPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position + targetOffset, moveSpeed * Time.deltaTime);
    }
}
