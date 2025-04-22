using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mainMenuScript : MonoBehaviour
{
    private musicManager mM;

    [SerializeField] private GameObject mainButtons;
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject quitPanel;


    [SerializeField] private Slider musicSldier;
    [SerializeField] private Slider sfxSldier;
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume";

    private bool isMusicMuted = false;
    private bool isSFXMuted = false;

    private float savedMusicVolume;
    private float savedSFXVolume;

    [SerializeField] private Image backgroundImage;
    private Color originalBackgroundColor;
    [SerializeField] private float brightnessMin = 0.8f;
    [SerializeField] private float brightnessMax = 1.2f;
    [SerializeField] private float brightnessChangeSpeed = 0.5f;
    private bool isBrightnessIncreasing = true;

    private void Start()
    {
        mM = musicManager.Instance;

        musicSldier.value = mM.musicVolume;
        sfxSldier.value = mM.sfxVolume;

        Debug.Log($"This music is{mM.isMusicMuted}");
        Debug.Log($"This sfx is {mM.isSfxMuted}");

        if (mM.isSfxMuted)
        {
            mM.a_Source.volume = 0;
        }
        if (mM.isMusicMuted)
        {
            mM.bgmusic_source.volume = 0;
            Debug.Log($"This ran and the volume is {mM.bgmusic_source.volume}");
        }

        isMusicMuted = mM.isMusicMuted;
        isSFXMuted = mM.isSfxMuted;

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

    public void AudioSettings()
    {
        creditsPanel.SetActive(false);
        audioPanel.SetActive(true);
    }
    public void sfxSlider(float vol)
    {
        if (!isSFXMuted)
        {
            mM.sfxVolume = vol;
            mM.a_Source.volume = vol;
        }
    }
    public void musicSlider(float vol)
    {
        if (!isMusicMuted)
        {
            mM.musicVolume = vol;
            mM.bgmusic_source.volume = vol;
        }
    }

    public void Credits()
    {
        audioPanel.SetActive(false);
        creditsPanel.SetActive(true);
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