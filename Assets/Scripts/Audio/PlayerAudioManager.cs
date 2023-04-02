using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public float footstepVolumeMultiplier = 0.02f;
    public float footstepFrequency;
    public AudioClip[] footsteps;

    AudioSource audioSorce;

    void Start()
    {
        audioSorce = GetComponent<AudioSource>();
        StartCoroutine(FootStepLogic());
    }

    IEnumerator FootStepLogic()
    {   
        while(true)
        {
            if(PlayerController.Instance.input.x != 0 || PlayerController.Instance.input.y != 0 && PlayerController.Instance.onGround)
            {
                PlayFootStep();
                yield return new WaitForSeconds(footstepFrequency);     
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
