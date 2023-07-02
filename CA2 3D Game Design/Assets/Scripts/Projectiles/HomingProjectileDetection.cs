using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectileDetection : MonoBehaviour
{
    public List<Transform> listOfTransforms = new List<Transform>();

    void OnTriggerEnter(Collider other)
    {
        //if object is not already in list, add object to list
        if (!listOfTransforms.Contains(other.transform) && other.GetComponent<BaseEnemy>())
        {
            listOfTransforms.Add(other.transform);
            gameObject.GetComponentInParent<HomingProjectile>().target = this.gameObject.GetComponentInParent<HomingProjectile>().GetClosestEnemy(listOfTransforms, this.transform).gameObject.GetComponent<BaseEnemy>();
            print("Current projectile target is " + gameObject.GetComponentInParent<HomingProjectile>().target);
        }
    }
    void OnTriggerExit(Collider other)
    {
        //if object is in list, remove it when it exits the collider
        if (listOfTransforms.Contains(other.transform) && other.GetComponent<BaseEnemy>())
        {
            listOfTransforms.Remove(other.transform);
        }
    }
}
