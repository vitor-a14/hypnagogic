using UnityEngine;
using UnityEngine.EventSystems;

public class UIComponentManager : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverAudio, selectAudio;

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlayOneShot2D(selectAudio, AudioManager.Instance.gameObject, AudioManager.AudioType.SFX, 0.2f);
        Debug.Log("clicked");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayOneShot2D(hoverAudio, AudioManager.Instance.gameObject, AudioManager.AudioType.SFX, 0.2f);
    }
}
