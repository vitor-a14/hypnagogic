using System.Collections;
using UnityEngine;

public class InteractableItem : Interactable
{
    public Item item;
    private float duration = 120;

    private void OnEnable() 
    {
        StartCoroutine(LoadInfo());
    }

    private IEnumerator LoadInfo()
    {
        yield return new WaitForEndOfFrame();
        string[] loadedPickedItems = SaveSystem.Instance.gameData.pickedItems;
        foreach(string pickedItemName in loadedPickedItems)
        {
            if(pickedItemName == transform.name)
                Destroy(gameObject);
        }
    }

    public override void Interact()
    {
        base.Interact();
        
        InventoryManager.Instance.Add(item);
        SaveSystem.Instance.gameData.AddPickedItemToInfo(transform);
        Destroy(gameObject);
    }

    public void SetItem(Item item, bool destroyAfterDuration)
    {
        this.item = item;
        Destroy(Instantiate(this.item.model, transform), duration);
    }
}
