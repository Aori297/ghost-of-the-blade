using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.UI;

public class musicManager : MonoBehaviour
{
    public float sfxVolume;
    public float musicVolume;
    public bool isSfxMuted;
    public bool isMusicMuted;
    public AudioSource a_Source;
    public AudioSource bgmusic_source;
    public AudioClip bgMusic;

    public AudioClip buttonClick;

    public static musicManager Instance;

    private Dictionary<string, AudioClip> audioDic;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);


        bgmusic_source.loop = true;
    }
    void Start()
    {
        audioDic = new Dictionary<string, AudioClip>
    {
        { "bgMusic", bgMusic },
        { "buttonClick", buttonClick },
    };
    }

    public void PlayBG()
    {
        bgmusic_source.clip = bgMusic;
        bgmusic_source.volume = musicVolume;
        bgmusic_source.Play();
    }

    public void PlayOnceClip(string clipName)
    {
        if (isSfxMuted)
        {
            a_Source.volume = 0f;

        }
        else
        {
            a_Source.volume = sfxVolume;

        }
        if (audioDic.TryGetValue(clipName, out AudioClip audioClip))
        {
            a_Source.PlayOneShot(audioClip);
        }
    }
}