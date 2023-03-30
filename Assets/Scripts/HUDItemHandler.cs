using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDItemHandler : MonoBehaviour
{
    [HideInInspector] public Item item;

    public void ShowDescriptionWindow()
    {
        InventoryManager.Instance.ShowItem(item);
    }
}
