using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public bool shouldLoadSavedGame { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            shouldLoadSavedGame = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadGameScene(bool loadSavedGame)
    {
        shouldLoadSavedGame = loadSavedGame;
        SceneManager.LoadScene("GameScene");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}