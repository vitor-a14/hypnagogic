using UnityEngine;

public class InteractableDoor : Interactable
{
    public Item keyItem;
    public float angularVelocity;
    public bool locked;

    private bool openDoor = false;
    private Quaternion openDoorRotation, closedDoorRotation;
    private BoxCollider doorCollider;

    private void Start() {
        doorCollider = GetComponent<BoxCollider>();
        openDoorRotation = transform.rotation * Quaternion.Euler(0f, -90f, 0f);
        closedDoorRotation = transform.rotation;
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
    }

    private void UpdateDoor()
    {
        if(openDoor)
            transform.rotation = Quaternion.Lerp(transform.rotation, openDoorRotation, Time.deltaTime * angularVelocity);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, closedDoorRotation, Time.deltaTime * angularVelocity);
    }
}
