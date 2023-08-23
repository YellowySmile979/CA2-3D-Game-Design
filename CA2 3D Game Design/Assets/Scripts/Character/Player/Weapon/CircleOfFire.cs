using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleOfFire : MonoBehaviour
{
    public float lifetime = 10f;
    [SerializeField] float damage;

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
        //handles how long the fire should remain
        if(lifetime >= 0)
        {
            lifetime -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    //makes any enemy that steps on it take damage
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BaseEnemy>())
        {
            other.GetComponent<BaseEnemy>().TakeDamage(damage);
        }
    }
}
