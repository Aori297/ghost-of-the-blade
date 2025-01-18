using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuScript : MonoBehaviour
{
   public void PlayGame()
    {
        SceneManager.LoadSceneAsync("GameScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
