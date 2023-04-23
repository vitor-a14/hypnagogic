using System.Collections;
using UnityEngine;

public class InteractableDoor : Interactable, IDataPersistance
{
    [SerializeField] private string id;
    [ContextMenu("Generate ID")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public Item keyItem;
    public float angularVelocity;
    public bool locked;

    private bool openDoor = false;
    private Quaternion openDoorRotation, closedDoorRotation;
    private BoxCollider doorCollider;

    //audio
    [SerializeField] private AudioClip doorOpenAudio, doorCloseAudio, doorLockedAudio;
    [SerializeField] private AudioSource audioSource;

    private void Start() 
    {
        if(id == null || id == "")
            Debug.LogWarning("Door ID is empty! Generate a ID");

        doorCollider = GetComponent<BoxCollider>();
        openDoorRotation = transform.rotation * Quaternion.Euler(0f, -90f, 0f);
        closedDoorRotation = transform.rotation;
    }

    public void Save(ref Data gameData)
    {
        if(gameData.unlockedDoors.ContainsKey(id))
            gameData.unlockedDoors.Remove(id);

        gameData.unlockedDoors.Add(id, true);
    }

    public void Load(Data gameData)
    {
        bool found = false;
        gameData.collectedItems.TryGetValue(id, out found);
        if(found) 
        {
            locked = false;
            ChangeDoorState();
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
