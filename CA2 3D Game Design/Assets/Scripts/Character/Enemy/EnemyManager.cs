using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    public List<BaseEnemy> baseEnemies = new List<BaseEnemy>(FindObjectsOfType<BaseEnemy>()
        .Select(baseEnemy => baseEnemy)
        .ToList());

    //a singleton
    public static EnemyManager Instance;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
