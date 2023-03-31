using UnityEngine;

public class InteractableItem : Interactable
{
    public Item item;

    public override void Interact()
    {
        base.Interact();
        InventoryManager.Instance.Add(item);
        Destroy(gameObject);
    }
}
