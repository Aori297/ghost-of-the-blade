using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGameData : MonoBehaviour
{
    public static PlayerGameData Instance { get; private set; }


    [SerializeField] private PlayerController player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        FindPlayer();

    }

    public void SyncDataToPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not found. Cannot sync data.");
            return;
        }
    }

    public void SyncDataFromPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not found. Cannot sync data.");
            return;
        }

        Debug.Log("Data Updated from Player");
    }

    public void FindPlayer()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        player = FindObjectOfType<PlayerController>(true);

        if (player != null)
        {
            Debug.Log($"Player found in scene: {currentScene.name}");
        }
        else
        {
            Debug.LogWarning($"Player not found in scene: {currentScene.name}");
        }
        SyncDataToPlayer();
    }
}