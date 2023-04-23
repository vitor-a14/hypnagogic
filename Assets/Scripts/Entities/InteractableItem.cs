using System.Collections;
using UnityEngine;

public class InteractableItem : Interactable, IDataPersistance
{
    //Save and ID logic
    [SerializeField] private string id;
    [ContextMenu("Generate ID")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    //Item script logic
    public Item item;
    private float duration = 120;

    private void Start() 
    {
        if(id == null || id == "")
            Debug.LogWarning("Item ID is empty! Generate a ID");
    }

    public void Save(ref Data gameData)
    {
        if(gameData.collectedItems.ContainsKey(id))
            gameData.collectedItems.Remove(id);

        gameData.collectedItems.Add(id, true); 
    }

    public void Load(Data gameData)
    {
        gameData.collectedItems.TryGetValue(id, out bool collected);
        if(collected) 
            Destroy(gameObject);
    }

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
