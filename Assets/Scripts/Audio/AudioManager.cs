using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public enum AudioType
    {
        SFX,
        Ambience,
        Music
    }

    [Header("Settings")]
    [Range(0, 5)] public float masterVolume;
    [Range(0, 1)] public float SFXVolume;
    [Range(0, 1)] public float ambienceVolume;
    [Range(0, 1)] public float musicVolume;

    [Header("Ambience Audio")]
    public AudioSource mainAudioThread;
    public AudioSource secundaryAudioThread;
    public float transitionTime = 1.25f;
    private AmbienceAudioAsset currentAmbienceSound;

    [Header("Others")]
    [SerializeField] private GameObject audioInstance;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");
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
        audioSource.volume *= masterVolume;
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
        audioSource.volume *= masterVolume;
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
        audioSource.volume *= masterVolume;
        audioSource.clip = audio;
        audioSource.Play();
    }

    //The audio manager has two audio sources. One main thread and a secundary thread
    //The function below will interpolate between the two to make a smooth audio transition
    public void ChangeAmbienceSound(AmbienceAudioAsset audio)
    {
        if(currentAmbienceSound == audio) return; //Avoid a transition between identical audios

        AudioSource nowPlaying = mainAudioThread;
        AudioSource target = secundaryAudioThread;

        if(!nowPlaying.isPlaying)
        {
            nowPlaying = secundaryAudioThread;
            target = mainAudioThread;
        }

        target.clip = audio.clip;
        currentAmbienceSound = audio;
        StopAllCoroutines(); //This avoid bugs in the coroutines 
        StartCoroutine(MixAudiosCoroutine(nowPlaying, target));
    }

    //User a coroutine to interpolate the ambience audios
    private IEnumerator MixAudiosCoroutine(AudioSource nowPlaying, AudioSource target)
    {
        float percentage = 0;
        float maxVolume = ambienceVolume * 0.2f * masterVolume;

        target.UnPause();

        if(target.isPlaying == false)
            target.Play();

        while(nowPlaying.volume > 0)
        {
            nowPlaying.volume = Mathf.Lerp(maxVolume, 0, percentage);
            target.volume = Mathf.Lerp(0, maxVolume, percentage);
            percentage += Time.deltaTime / transitionTime;
            yield return null;
        }

        nowPlaying.Pause();

        percentage = 0; 
    }
}
