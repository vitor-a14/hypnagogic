using System.Collections;
using UnityEngine;

[System.Serializable]
public struct FootstepSet
{
    public string tag;
    public AudioClip[] footstepAudios;
}

public class PlayerAudioManager : MonoBehaviour
{
    public static PlayerAudioManager Instance { get; private set; }
    
    [Header("Footsteps")]
    public float footstepVolumeMultiplier = 0.02f;
    public float footstepFrequency;
    public FootstepSet[] footsteps;
    private int lastFootstepSound;

    float toggleSpeed = 3f;
    AudioSource audioSorce;

    [Header("Jump")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landingSound;

    private void Awake() 
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");    
    }

    private void Start()
    {
        audioSorce = GetComponent<AudioSource>();
        StartCoroutine(FootStepLogic());
    }

    private IEnumerator FootStepLogic()
    {   
        while(true)
        {
            float actualFootstepFrequency = footstepFrequency;

            if(PlayerController.Instance.isRunning) 
                actualFootstepFrequency /= PlayerController.Instance.runMultiplier;
            else if(PlayerController.Instance.isCrounching || CombatHandler.Instance.attacking || CombatHandler.Instance.defending)
                actualFootstepFrequency /= PlayerController.Instance.crouchingMultiplier;

            if((PlayerController.Instance.input.x != 0 || PlayerController.Instance.input.y != 0) && PlayerController.Instance.onGround && PlayerController.Instance.aboveToggleSpeed)
            {
                PlayFootStep();
                yield return new WaitForSeconds(actualFootstepFrequency);     
            }
            else 
            {
                audioSorce.Stop();
                yield return null;
            }
        }
    }

    public void JumpSound()
    {
        AudioManager.Instance.PlayOneShot3D(jumpSound, gameObject, AudioManager.AudioType.SFX, footstepVolumeMultiplier);
    }

    public void LandingSound()
    {
        AudioClip footstepsSound = GetFootStepAudio();

        if(footstepsSound != null)
            AudioManager.Instance.PlayOneShot3D(footstepsSound, gameObject, AudioManager.AudioType.SFX, footstepVolumeMultiplier);
    }

    public void PlayFootStep()
    {
        AudioClip footstepsSound = GetFootStepAudio();

        if(footstepsSound != null)
            AudioManager.Instance.PlayOnAudioSorce(footstepsSound, audioSorce, AudioManager.AudioType.SFX, footstepVolumeMultiplier);
    }

    private AudioClip GetFootStepAudio()
    {
        AudioClip[] footstepsSound = null;

        foreach(FootstepSet footstep in footsteps)
        {
            if(footstep.tag == PlayerController.Instance.floorType)
                footstepsSound = footstep.footstepAudios;
        }

        int footStepRandomIndex = Random.Range(0, footstepsSound.Length - 1);
        if(footStepRandomIndex == lastFootstepSound)
        {
            if(footStepRandomIndex < footstepsSound.Length)
                footStepRandomIndex += 1;
            else
                footStepRandomIndex = 0;

            lastFootstepSound = footStepRandomIndex;
        }

        return footstepsSound[footStepRandomIndex];
    }
}
