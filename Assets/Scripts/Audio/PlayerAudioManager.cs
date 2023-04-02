using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public float footstepVolumeMultiplier = 0.02f;
    public float footstepFrequency;
    public AudioClip[] footsteps;

    void Start()
    {
        StartCoroutine(FootStepLogic());
    }

    IEnumerator FootStepLogic()
    {   
        while(true)
        {
            if(PlayerController.Instance.input.x != 0 || PlayerController.Instance.input.y != 0 && PlayerController.Instance.onGround)
            {
                AudioManager.Instance.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)], transform.position, AudioManager.AudioType.SFX, footstepVolumeMultiplier);
                yield return new WaitForSeconds(footstepFrequency);     
            }
            else yield return null;
        }
    }
}
