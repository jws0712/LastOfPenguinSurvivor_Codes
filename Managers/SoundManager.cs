using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;

    public Sound[] musicSounds, sfxSounds;
    public RandomSoundList[] musicSoundsRandom;
    public AudioSource musicSource, sfxSource;

    //½Ì±ÛÅæ
    private void Awake()
    {
        #region ½Ì±ÛÅæ
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion
    }

    public void UpdateMusicVolume(float value)
    {
        musicSource.volume = value;
    }

    public void UpdateSFXVolume(float value)
    {
        sfxSource.volume = value;
    }

    //À½¾Ç ½ÇÇà
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Music Not Found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    //À½¾Ç Àç»ý Á¾·á
    public void StopMusic()
    {
        musicSource.Stop();
    }


    //È¿°úÀ½ ½ÇÇà
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sount Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }
}