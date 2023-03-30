using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> items = new List<Item>();
    public GameObject inventoryItem;
    public Transform contentGrid;
    public GameObject inventoryComponent;
    

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

    public void ListItems() 
    {
        inventoryComponent.SetActive(inventoryComponent.activeSelf ? false : true);
        PlayerController.Instance.canMove = !inventoryComponent.activeSelf;

        if(!inventoryComponent.activeSelf) return;

        foreach (Transform item in contentGrid)
        {
            Destroy(item.gameObject);
        }

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
