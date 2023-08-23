using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScreenManager : MonoBehaviour
{
    public string gameScene;
    public string creditsScene;

    public void StartGame() 
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(gameScene);
    }

    public void Credits()
    {
        SceneManager.LoadSceneAsync(creditsScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
