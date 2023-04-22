using System.Collections;

public class InteractableItem : Interactable
{
    public Item item;
    private float duration = 120;

    public override void Interact()
    {
        base.Interact();
        InventoryManager.Instance.Add(item);
        Destroy(gameObject);
    }

    public void SetItem(Item item, bool destroyAfterDuration)
    {
        this.item = item;
        Destroy(Instantiate(this.item.model, transform), duration);
    }
}
