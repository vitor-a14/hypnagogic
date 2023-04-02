using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public enum AudioType
    {
        SFX,
        Ambience,
        Music
    }

    [Header("Settings")]
    [Range(0, 1)] public float masterVolume;
    [Range(0, 1)] public float SFXVolume;
    [Range(0, 1)] public float ambienceVolume;
    [Range(0, 1)] public float musicVolume;

    [Header("Others")]
    [SerializeField] private GameObject audioInstance;

    void Awake()
    {
        Instance = this;
    }

    //Create a 3D audio instance in a gameobject
    public void PlayOneShot3D(AudioClip audio, GameObject entity, AudioType type, float multiplier)
    {
        AudioSource audioSource = entity.AddComponent<AudioSource>();

        switch(type)
        {
        case AudioType.SFX:
            audioSource.volume = SFXVolume;
            break;
        case AudioType.Ambience:
            audioSource.volume = ambienceVolume;
            break;
        case AudioType.Music:
            audioSource.volume = musicVolume;
            break;
        }

        audioSource.spatialBlend = 1;
        audioSource.volume *= multiplier;
        audioSource.PlayOneShot(audio);
        
        Destroy(audioSource, audio.length);
    }

    //Create a 2D audio instance in a gameobject
    public void PlayOneShot2D(AudioClip audio, GameObject entity, AudioType type, float multiplier)
    {
        AudioSource audioSource = entity.AddComponent<AudioSource>();

        switch(type)
        {
        case AudioType.SFX:
            audioSource.volume = SFXVolume;
            break;
        case AudioType.Ambience:
            audioSource.volume = ambienceVolume;
            break;
        case AudioType.Music:
            audioSource.volume = musicVolume;
            break;
        }

        audioSource.spatialBlend = 0;
        audioSource.volume *= multiplier;
        audioSource.PlayOneShot(audio);
        
        Destroy(audioSource, audio.length);
    }

    //Use a already existent audio source to play a audio
    public void PlayOnAudioSorce(AudioClip audio, AudioSource audioSource, AudioType type, float multiplier)
    {
        switch(type)
        {
        case AudioType.SFX:
            audioSource.volume = SFXVolume;
            break;
        case AudioType.Ambience:
            audioSource.volume = ambienceVolume;
            break;
        case AudioType.Music:
            audioSource.volume = musicVolume;
            break;
        }

        audioSource.volume *= multiplier;
        audioSource.PlayOneShot(audio);
    }
}
