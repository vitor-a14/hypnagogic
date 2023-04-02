using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public List<Item> items = new List<Item>();
    public GameObject inventoryItem; //Item HUD instance
    public Transform contentGrid; //Grid where the items will be displayed
    public GameObject itemWindow; //Item window
    public GameObject inventoryComponent; //All the inventory HUD to show or hide
    public bool onInventory = false;

    [SerializeField] private AudioClip addItemAudio, removeItemAudio;
    
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");

        PlayerController.Instance.controls.Player.Inventory.performed += ctx => ListItems();
    }

    public void Add(Item item) 
    {
        items.Add(item);
        HUDManager.Instance.TriggerPopUp(item.itemName + " added");
        AudioManager.Instance.PlayOneShot2D(addItemAudio, gameObject, AudioManager.AudioType.SFX, 1);
    }

    public void Remove(Item item) 
    {
        items.Remove(item);
        HUDManager.Instance.TriggerPopUp(item.itemName + " removed");
        AudioManager.Instance.PlayOneShot2D(removeItemAudio, gameObject, AudioManager.AudioType.SFX, 1);
    }

    //Open or close the inventory and update the HUD with the itens
    //Also update if the player can move or not
    public void ListItems() 
    {
        if(!PlayerController.Instance.onGround || DialogueManager.Instance.dialogueIsPlaying) return;

        onInventory = !onInventory;

        itemWindow.SetActive(false);
        inventoryComponent.SetActive(onInventory);
        PlayerController.Instance.UseMouse(onInventory);

        if(!onInventory) return;

        foreach (Transform item in contentGrid)
            Destroy(item.gameObject);

        foreach (var item in items)
        {
            GameObject obj = Instantiate(inventoryItem, contentGrid);
            var itemName = obj.transform.Find("Name").GetComponent<TMP_Text>();
            var itemIcon = obj.transform.Find("Icon").GetComponent<Image>();
            obj.GetComponent<HUDItemHandler>().item = item;
            
            itemName.text = item.name;
            itemIcon.sprite = item.icon;
        }
    }

    //Open a window in the inventory to show the item description
    public void ShowItem(Item item)
    {
        if(!onInventory) return; //Only show item if inventory is open

        itemWindow.SetActive(true); //Show window

        //If there is content in the windows, remove them
        Transform itemWindowContent = itemWindow.transform.GetChild(0);
        if(itemWindowContent.childCount > 0)
        {
            for(int i = 0; i < itemWindowContent.childCount; i++)
                Destroy(itemWindowContent.GetChild(i).gameObject);
        }

        //Spawn the content
        Instantiate(item.itemDescriptionWindow, itemWindowContent);
    }

    public bool SearchForItem(Item targetItem)
    {
        foreach(Item item in items)
        {
            if(item == targetItem)
                return true;
        }

        return false;
    }
}
