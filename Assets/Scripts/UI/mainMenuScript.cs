using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mainMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject mainButtons;
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject quitPanel;

    [SerializeField] private Image backgroundImage;
    private Color originalBackgroundColor;
    [SerializeField] private float brightnessMin = 0.8f;
    [SerializeField] private float brightnessMax = 1.2f;
    [SerializeField] private float brightnessChangeSpeed = 0.5f;
    private bool isBrightnessIncreasing = true;

    private void Start()
    {

        if (SceneLoader.Instance == null)
        {
            GameObject sceneLoaderObj = new GameObject("SceneLoader");
            sceneLoaderObj.AddComponent<SceneLoader>();
        }

        originalBackgroundColor = backgroundImage.color;
        StartCoroutine(AnimateBackgroundBrightness());
    }

    private IEnumerator AnimateBackgroundBrightness()
    {
        while (true)
        {
            float currentBrightness = backgroundImage.color.grayscale;
            if (isBrightnessIncreasing)
            {
                currentBrightness += Time.deltaTime * brightnessChangeSpeed;
                if (currentBrightness >= brightnessMax)
                    isBrightnessIncreasing = false;
            }
            else
            {
                currentBrightness -= Time.deltaTime * brightnessChangeSpeed;
                if (currentBrightness <= brightnessMin)
                    isBrightnessIncreasing = true;
            }
            backgroundImage.color = new Color(
                originalBackgroundColor.r * currentBrightness,
                originalBackgroundColor.g * currentBrightness,
                originalBackgroundColor.b * currentBrightness,
                originalBackgroundColor.a
            );
            yield return null;
        }
    }

    public void Continue()
    {
        string savePath = Application.persistentDataPath + "/savegame.json";
        if (File.Exists(savePath))
        {
            SceneLoader.Instance.LoadGameScene(true);
        }
        else
        {
            mainButtons.SetActive(false);
        }
    }

    public void NewGame()
    {
        mainButtons.SetActive(false);
        newGamePanel.SetActive(true);
    }

    public void YesNewGame()
    {
        SceneLoader.Instance.LoadGameScene(false);
    }

    public void NoNewGame()
    {
        newGamePanel.SetActive(false);
        mainButtons.SetActive(true);
    }

    public void OpenSettings()
    {
        mainButtons.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainButtons.SetActive(true);
    }

    public void QuitGame()
    {
        mainButtons.SetActive(false);
        quitPanel.SetActive(true);
    }

    public void ConfirmQuit()
    {
        Application.Quit();
    }

    public void CancelQuit()
    {
        quitPanel.SetActive(false);
        mainButtons.SetActive(true);
    }
}