using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CheckpointScript : MonoBehaviour
{
    [SerializeField] private BoxCollider2D checkpoint;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject savePanel;
    [SerializeField] private Transform playerTransform;

    private void Start()
    {
        gameManager = GameManager.Instance;

        if (playerTransform == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform != null && checkpoint.bounds.Contains(playerTransform.position))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                savePanel.SetActive(true);
            }
        }
    }

    public void SaveGameData()
    {
        gameManager.SaveData();
        savePanel.SetActive(false);
    }
}
