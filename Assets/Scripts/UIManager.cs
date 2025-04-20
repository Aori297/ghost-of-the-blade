using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject SettingsUI;
    [SerializeField] private Button[] level_buttons;
    //private musicManager mM;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private bool isPaused;

    [SerializeField] private Slider musicSldier;
    [SerializeField] private Slider sfxSldier;
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume"; 

    [SerializeField] private Button musicToggleButton; 
    [SerializeField] private Image musicButtonImage; 
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite; 

    [SerializeField] private Button sfxToggleButton; 
    [SerializeField] private Image sfxButtonImage;
    [SerializeField] private Sprite sfxOnSprite; 
    [SerializeField] private Sprite sfxOffSprite;

    private GameInput gameInput;


    private bool isMusicMuted = false; 
    private bool isSFXMuted = false; 

    private float savedMusicVolume;
    private float savedSFXVolume;

    private void Start()
    {
        Time.timeScale = 1f;
        //mM = musicManager.Instance;

        //musicSldier.value = mM.musicVolume;
        //sfxSldier.value = mM.sfxVolume;

        //Debug.Log($"This music is{mM.isMusicMuted}");
        //Debug.Log($"This sfx is {mM.isSfxMuted}");

        //if (mM.isSfxMuted)
        //{
        //    mM.a_Source.volume = 0;
        //}
        //if (mM.isMusicMuted)
        //{
        //    mM.bgmusic_source.volume = 0;
        //    Debug.Log($"This ran and the volume is {mM.bgmusic_source.volume}");
        //}

        //isMusicMuted = mM.isMusicMuted;
        //isSFXMuted = mM.isSfxMuted;

        //musicButtonImage.sprite = isMusicMuted ? musicOffSprite : musicOnSprite;
        //sfxButtonImage.sprite = isSFXMuted ? sfxOffSprite : sfxOnSprite;

        //musicToggleButton.onClick.AddListener(ToggleMusic);
        //sfxToggleButton.onClick.AddListener(ToggleSFX);
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

    //public void sfxSlider(float vol)
    //{
    //    //if (!isSFXMuted)
    //    //{
    //    //    mM.sfxVolume = vol;
    //    //    mM.a_Source.volume = vol;
    //    //}
    //}
    //public void musicSlider(float vol)
    //{
    //    if (!isMusicMuted)
    //    {
    //        mM.musicVolume = vol;
    //        mM.bgmusic_source.volume = vol;
    //    }
    //}
    //private void ToggleMusic()
    //{
    //    isMusicMuted = !isMusicMuted; // Toggle mute state
    //    HandleMute(musicVolumeParameter, isMusicMuted, ref savedMusicVolume);

    //    musicButtonImage.sprite = isMusicMuted ? musicOffSprite : musicOnSprite;

    //}

    //private void ToggleSFX()
    //{
    //    isSFXMuted = !isSFXMuted; // Toggle mute state
    //    HandleMute(sfxVolumeParameter, isSFXMuted, ref savedSFXVolume);
    //    mM.a_Source.volume = 0f;
    //    sfxButtonImage.sprite = isSFXMuted ? sfxOffSprite : sfxOnSprite;
    //}

    //private void HandleMute(string parameter, bool isMuted, ref float savedVolume)
    //{
    //    if (isMuted)
    //    {
    //        if (parameter == musicVolumeParameter)
    //        {
    //            mM.bgmusic_source.volume = 0f;
    //            //mM.musicVolume = 0f;
    //            mM.isMusicMuted = true;
    //        }
    //        else
    //        {
    //            mM.a_Source.volume = 0f;
    //            //mM.sfxVolume = 0f;
    //            mM.isSfxMuted = true;
    //        }

    //    }
    //    else if (!isMuted)
    //    {
    //        if (parameter == musicVolumeParameter)
    //        {
    //            mM.bgmusic_source.volume = mM.musicVolume;
    //            mM.isMusicMuted = false;
    //        }
    //        else
    //        {
    //            mM.a_Source.volume = mM.sfxVolume;
    //            mM.isSfxMuted = false;
    //        }
    //    }
    //}
}
