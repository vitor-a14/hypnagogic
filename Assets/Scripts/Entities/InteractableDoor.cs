using System.Collections;
using UnityEngine;

public class InteractableDoor : Interactable
{
    public Item keyItem;
    public float angularVelocity;
    public bool locked;

    private bool openDoor = false;
    private Quaternion openDoorRotation, closedDoorRotation;
    private BoxCollider doorCollider;

    //audio
    [SerializeField] private AudioClip doorOpenAudio, doorCloseAudio, doorLockedAudio;
    [SerializeField] private AudioSource audioSource;

    private void OnEnable() {
        StartCoroutine(LoadInfo());

        doorCollider = GetComponent<BoxCollider>();
        openDoorRotation = transform.rotation * Quaternion.Euler(0f, -90f, 0f);
        closedDoorRotation = transform.rotation;
    }

    private IEnumerator LoadInfo()
    {
        yield return new WaitForEndOfFrame();
        string[] loadedUnlockedDoors = SaveSystem.Instance.gameData.unlockedDoors;
        foreach(string unlockedDoorName in loadedUnlockedDoors)
        {
            if(unlockedDoorName == transform.name)
            {
                locked = false;
                ChangeDoorState();
                break;
            }
        }
    }

    public override void Interact()
    {
        base.Interact();

        if(locked)
        {
            if(InventoryManager.Instance.SearchForItem(keyItem))
            {
                InventoryManager.Instance.Remove(keyItem);
                locked = false;
                ChangeDoorState();
            }
            else
                AudioManager.Instance.PlayOnAudioSorce(doorLockedAudio, audioSource, AudioManager.AudioType.SFX, 1);
        }
        else
            ChangeDoorState();
    }

    private void Update() 
    {
        UpdateDoor();
    }

    private void ChangeDoorState()
    {   
        openDoor = !openDoor;
        doorCollider.isTrigger = openDoor;   

        if(openDoor)
            AudioManager.Instance.PlayOnAudioSorce(doorOpenAudio, audioSource, AudioManager.AudioType.SFX, 1);
        else
            AudioManager.Instance.PlayOnAudioSorce(doorCloseAudio, audioSource, AudioManager.AudioType.SFX, 1);
    }

    private void UpdateDoor()
    {
        if(openDoor)
            transform.rotation = Quaternion.Lerp(transform.rotation, openDoorRotation, Time.deltaTime * angularVelocity);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, closedDoorRotation, Time.deltaTime * angularVelocity);
    }
}
