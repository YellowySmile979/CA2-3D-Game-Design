using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleOfFire : MonoBehaviour
{
    float damage;

    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectOfType<MagePlayerController>())
        {
            MagePlayerController magePlayerController = FindObjectOfType<MagePlayerController>();
            damage = magePlayerController.playerDamage * magePlayerController.dmgMultiplier;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
