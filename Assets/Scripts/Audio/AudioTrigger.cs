using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AmbienceAudioAsset ambienceAudio;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            AudioManager.Instance.ChangeAmbienceSound(ambienceAudio);
        }
    }
}
