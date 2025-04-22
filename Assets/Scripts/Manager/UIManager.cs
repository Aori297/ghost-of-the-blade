using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    private musicManager mM;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private bool isPaused;

    [SerializeField] private GameObject LoadScreen;
    [SerializeField] private Animator Backgroundanim;
    [SerializeField] private Animator Loaderanim;


    [SerializeField] private Slider musicSldier;
    [SerializeField] private Slider sfxSldier;
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume"; 

    private GameInput gameInput;

    private bool isMusicMuted = false; 
    private bool isSFXMuted = false; 

    private float savedMusicVolume;
    private float savedSFXVolume;

    private void Start()
    {
        Invoke("LoadingScreen", 5);
        Time.timeScale = 1f;
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

        if (gameInput == null)
        {
            gameInput = GameInput.Instance;
            if (gameInput == null)
            {
                Debug.LogError("GameInput script not found in the scene!");
                return;
            }
        }


        gameInput.inputActions.PlayerInput.Pause.performed += _ => Pause();
    }

    public void LoadingScreen()
    {
        Backgroundanim.SetTrigger("Loaded");
        Loaderanim.SetTrigger("Loaded");
        Invoke("LoadingScreenOff", 2f);
    }
    public void LoadingScreenOff()
    {
        LoadScreen.SetActive(false);
    }
    public void Pause()
    {
        //mM.PlayOnceClip("buttonClick");
        Time.timeScale = 0f;

        isPaused = true;

        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        //mM.PlayOnceClip("buttonClick");
        Time.timeScale = 1f;

        isPaused = false;

        pausePanel.SetActive(false);
    }

    public void OpenSettings()
    {
        //mM.PlayOnceClip("buttonClick");
        Time.timeScale = 0f;
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        //mM.PlayOnceClip("buttonClick");
        SceneManager.LoadScene("MainMenu");
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
}
