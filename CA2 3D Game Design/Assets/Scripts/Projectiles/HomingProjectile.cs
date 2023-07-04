using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : BaseProjectile
{
    [Header("Target")]
    public BaseEnemy target;

    [Header("Prediction")]
    public float maxPredictionDistance = 100f;
    public float minPredictionDistance = 5f;
    public float maxPredictionTime = 5f;
    Vector3 standardPrediction, deviatedPrediction;

    [Header("Deviation")]
    public float deviationAmount = 50f;
    public float deviationSpeed = 2f;

    [Header("Avoid Floor")]
    public GameObject floor;
    public float maxDistFromGround = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        projectileRotationSpeed = projectileData.rotationSpeed;
        projectileMoveSpeed = projectileData.moveSpeed;
        projectileDamage = projectileData.damage;
    }
    //inputs this projectile's behaviour i.e. homing
    public override void ProjectileBehaviour()
    {
        //if there isnt any target, try to find another, otherwise move forward and prevent rest of code from firing
        if (target == null || target != FindObjectOfType<BaseEnemy>())
        {
            print("THIS IS FIRING");
            target = FindObjectOfType<BaseEnemy>();
            rb.velocity = transform.forward * projectileMoveSpeed;
            AvoidFloor();
            return;
        }

        //sets the leadTimePercentage
        var leadTimePercentage = Mathf.InverseLerp(
            minPredictionDistance,
            maxPredictionDistance,
            Vector3.Distance(transform.position, target.transform.position)
            );

        rb.velocity = transform.forward * projectileMoveSpeed;

        PredictEnemyMovement(leadTimePercentage);
        AddDeviation(leadTimePercentage);
        RotateProjectile();
        AvoidFloor();
    }
    //gets the closest enemy within the sphere collider of the detector
    public Transform GetClosestEnemy(List<Transform> enemies, Transform fromThis)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = fromThis.position;
        foreach(Transform potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        return bestTarget;
    }
    
    //prevents the projectile from touching the floor
    void AvoidFloor()
    {
        if (transform.position.y <= floor.transform.position.y + maxDistFromGround)
        {
            transform.position = new Vector3(
                transform.position.x,
                floor.transform.position.y + maxDistFromGround,
                transform.position.z
                );
        }
    }
    //predicts the enemy's movement and homes in
    void PredictEnemyMovement(float leadTimePercentage)
    {
        var predectionTime = Mathf.Lerp(0, maxPredictionTime, leadTimePercentage);
        standardPrediction = target.Rb.position + target.Rb.velocity * predectionTime;
    }
    //deviates the projectile slightly to make the projectile seem more natural
    void AddDeviation(float leadTimePercentage)
    {
        var deviation = new Vector3(Mathf.Cos(Time.time * deviationSpeed), 0, 0);
        var predictionOffset = transform.TransformDirection(deviation) * deviationAmount * leadTimePercentage;
        deviatedPrediction = standardPrediction + predictionOffset;
    }
    //does what the function says
    void RotateProjectile()
    {
        var heading = deviatedPrediction - transform.position;
        var rotation = Quaternion.LookRotation(heading);
        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, projectileRotationSpeed * Time.deltaTime));
    }
    //checks for collisions
    void OnTriggerEnter(Collider collision)
    {
        //if collide with enemy, destroy enemy and self
        //otherwise destroy self
        if (collision.gameObject.GetComponent<BaseEnemy>())
        {
            collision.gameObject.GetComponent<BaseEnemy>().TakeDamage(projectileDamage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
    }
    //shows the projectile either using standard or deviated prediction
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, standardPrediction);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(standardPrediction, deviatedPrediction);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 10, transform.position.z));
    }
}
