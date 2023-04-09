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
    public TMP_Text itemName, itemDescription;
    public GameObject inventoryComponent; //All the inventory HUD to show or hide
    public bool onInventory = false;

    public Transform equippablePivot;
    private GameObject currentEquipedItem;

    //Is this boolean is true, it means that the fade animation of the inventory is running. The player only can move when this is finished
    private bool inventoryTransition = false; 

    [SerializeField] private AudioClip addItemAudio, removeItemAudio;
    
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");

        PlayerController.Instance.controls.Player.Inventory.performed += ctx => ListItems();
    }

    private void LateUpdate() 
    {
        if(currentEquipedItem != null)
        {
            currentEquipedItem.transform.position = Vector3.Slerp(currentEquipedItem.transform.position, equippablePivot.position, 25 * Time.deltaTime);
            var rot = Quaternion.LookRotation(equippablePivot.forward);
            rot.x = 0;
            rot.z = 0;
            currentEquipedItem.transform.rotation = rot;
        }
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
        if(!PlayerController.Instance.onGround || DialogueManager.Instance.dialogueIsPlaying || inventoryTransition) return;
        //Inventory transition: the player can only open or close the inventory if he is not in the fade animation

        onInventory = !onInventory;

        itemName.text = "";
        itemDescription.text = "";

        StartCoroutine(InventoryFadeDurationCoroutine(HUDEffects.fadeDuration)); //Fade duration delay
        HUDEffects.Instance.FadeUI(inventoryComponent, onInventory); //Apply fade in/out animation in the inventyory HUD

        PlayerController.Instance.UseMouse(onInventory);

        if(!onInventory) return;

        foreach (Transform item in contentGrid)
            Destroy(item.gameObject);

        foreach (var item in items)
        {
            GameObject obj = Instantiate(inventoryItem, contentGrid);
            var itemIcon = obj.transform.Find("Icon").GetComponent<Image>();
            obj.GetComponent<HUDItemHandler>().item = item;
            
            itemIcon.sprite = item.icon;
        }
    }

    //Open a window in the inventory to show the item description
    public void ShowItem(Item item)
    {
        if(!onInventory) return; //Only show item if inventory is open

        itemName.text = item.itemName;
        itemDescription.text = item.description;

        EquipItem(item);
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

    private void EquipItem(Item item)
    {
        if(!item.isEquippable) return;

        if(currentEquipedItem != null)
            Destroy(currentEquipedItem);

        currentEquipedItem = Instantiate(item.equippableInstance, equippablePivot);
        currentEquipedItem.transform.SetParent(null);
    }

    private IEnumerator InventoryFadeDurationCoroutine(float duration)
    {
        inventoryTransition = true;
        yield return new WaitForSeconds(duration);
        inventoryTransition = false;
    }
}
