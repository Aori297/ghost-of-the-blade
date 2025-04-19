using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private PlayerGameData gameData;
    private PlayerController player;
    private string savePath;

    private void Awake()
    {
        // Singleton pattern
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

        // Set save path
        savePath = Application.persistentDataPath + "/savegame.json";

        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find player in every scene transition
        FindPlayer();
    }

    private void Start()
    {
        FindPlayer();
        gameData = PlayerGameData.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadData();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            SaveData();
        }
    }

    private void FindPlayer()
    {
        // Multiple methods to find player
        player = FindAnyObjectByType<PlayerController>();

        if (player == null)
        {
            // Try finding inactive objects or by tag
            player = FindObjectOfType<PlayerController>(true);
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        }

        if (player != null)
        {
            Debug.Log($"Player found in scene: {SceneManager.GetActiveScene().name}");
        }
        else
        {
            Debug.LogWarning($"Player not found in scene: {SceneManager.GetActiveScene().name}");
        }
    }

    public void SaveData()
    {
        FindPlayer(); // Ensure we have the latest player reference

        if (player != null)
        {
            PlayerSaveData data = new PlayerSaveData(player);
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);

            if (gameData != null)
            {
                gameData.SyncDataFromPlayer();
            }

            Debug.Log("Game Saved!");
        }
        else
        {
            Debug.LogWarning("Cannot save: Player not found!");
        }
    }

    public void LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            // Find player before loading data
            FindPlayer();

            if (player != null)
            {
                // Apply loaded data to player
                player.transform.position = new Vector3(data.Xpos, data.Ypos);

                if (gameData != null)
                {
                    gameData.SyncDataFromPlayer();
                }

                Debug.Log("Game Loaded!");
            }
            else
            {
                Debug.LogWarning("Cannot load: Player not found!");
            }
        }
        else
        {
            Debug.LogWarning("No save file found!");
        }
    }
}