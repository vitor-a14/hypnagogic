using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; //Acess from other scripts
    public List<Item> items = new List<Item>();
    public GameObject inventoryItem; //Item HUD instance
    public Transform contentGrid; //Grid where the items will be displayed
    public GameObject inventoryComponent; //All the inventory HUD to show or hide
    
    void Start()
    {
        Instance = this;
    }

    public void Add(Item item) 
    {
        items.Add(item);
    }

    public void Remove(Item item) 
    {
        items.Remove(item);
    }

    //Open or close the inventory and update the HUD with the itens
    //Also update if the player can move or not
    public void ListItems() 
    {
        if(!PlayerController.Instance.onGround) return;

        inventoryComponent.SetActive(inventoryComponent.activeSelf ? false : true);

        bool inventoryIsActive = inventoryComponent.activeSelf;
        PlayerController.Instance.canMove = !inventoryIsActive;
        PlayerController.Instance.UseMouse(inventoryIsActive);

        if(!inventoryIsActive) return;

        foreach (Transform item in contentGrid)
            Destroy(item.gameObject);

        foreach (var item in items)
        {
            GameObject obj = Instantiate(inventoryItem, contentGrid);
            var itemName = obj.transform.Find("Name").GetComponent<TMP_Text>();
            var itemIcon = obj.transform.Find("Icon").GetComponent<Image>();

            itemName.text = item.name;
            itemIcon.sprite = item.icon;
        }
    }
}
