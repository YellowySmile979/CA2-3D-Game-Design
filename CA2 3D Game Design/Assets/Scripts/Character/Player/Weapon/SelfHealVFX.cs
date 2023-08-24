using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfHealVFX : MonoBehaviour
{
    public float lifetime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lifetime > 0)
        {
            lifetime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
