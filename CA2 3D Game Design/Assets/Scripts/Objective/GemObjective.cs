using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemObjective : MonoBehaviour
{
    public float portalHealth;

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
        //updates the UI for the portal health
        CanvasController.Instance.UpdateGemHealth(portalHealth);

        if (portalHealth <= 0f)
        {
            WaveManager.Instance.gameState = WaveManager.GameState.Lost;
        }
    }
}
