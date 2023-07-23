using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalObjective : MonoBehaviour
{
    public float portalHealth = 100f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePortalHealth();
    }
    //checks to see if portal is destroyed
    public void UpdatePortalHealth()
    {
        if (portalHealth <= 0f)
        {
            WaveManager.Instance.gameState = WaveManager.GameState.Lost;
        }
    }
}
