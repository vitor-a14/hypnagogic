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

    public void PlayOneShot(AudioClip audio, Vector3 position, AudioType type, float multiplier)
    {
        GameObject instance = Instantiate(audioInstance, position, Quaternion.identity);
        AudioSource audioSource = instance.GetComponent<AudioSource>();

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
        Destroy(instance, audio.length);
    }
}
