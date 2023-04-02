using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public float footstepVolumeMultiplier = 0.02f;
    public float footstepFrequency;
    public AudioClip[] footsteps;

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
        AudioManager.Instance.PlayOnAudioSorce(footsteps[Random.Range(0, footsteps.Length)], audioSorce, AudioManager.AudioType.SFX, footstepVolumeMultiplier);
    }
}
