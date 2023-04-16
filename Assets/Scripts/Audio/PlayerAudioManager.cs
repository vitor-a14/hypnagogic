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
    [Header("Footsteps")]
    public float footstepVolumeMultiplier = 0.02f;
    public float footstepFrequency;
    public FootstepSet[] footsteps;

    float toggleSpeed = 3f;
    AudioSource audioSorce;

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

    public void PlayFootStep()
    {
        AudioClip[] footstepsSound = null;
        foreach(FootstepSet footstep in footsteps)
        {
            if(footstep.tag == PlayerController.Instance.floorType)
                footstepsSound = footstep.footstepAudios;
        }

        AudioManager.Instance.PlayOnAudioSorce(footstepsSound[Random.Range(0, footstepsSound.Length)], audioSorce, AudioManager.AudioType.SFX, footstepVolumeMultiplier);
    }
}
