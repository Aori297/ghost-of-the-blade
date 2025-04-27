using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject deathScreen;
    public static GameManager Instance { get; private set; }
    private PlayerGameData gameData;
    private PlayerController player;
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        savePath = Application.persistentDataPath + "/savegame.json";

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

    private void Start()
    {
        FindPlayer();
        gameData = PlayerGameData.Instance;

        StartCoroutine(CheckForLoadRequest());
    }

    private IEnumerator CheckForLoadRequest()
    {
        yield return new WaitForSeconds(0.2f);

        if (SceneLoader.Instance != null && SceneLoader.Instance.shouldLoadSavedGame)
        {
            int attempts = 0;
            while (player == null && attempts < 10)
            {
                FindPlayer();
                if (player == null)
                {
                    yield return new WaitForSeconds(0.1f);
                    attempts++;
                }
            }

            LoadData();
        }
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
        player = FindAnyObjectByType<PlayerController>();
        if (player == null)
        {
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
        FindPlayer(); 
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
    }

    public void DieRespawn()
    {
        deathScreen.SetActive(false);
        LoadData();
    }

    public void LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            FindPlayer();
            if (player != null)
            {
                player.transform.position = new Vector3(data.Xpos, data.Ypos);
                player.dashEnabled = data.dashEnabled;
                player.doubleJumpEnabled = data.doubleJumpEnabled;
                if (gameData != null)
                {
                    gameData.SyncDataFromPlayer();
                }
                Debug.Log("Game Loaded!");
            }
        }
        else
        {
            Debug.LogWarning("No save file found!");
        }
    }
}