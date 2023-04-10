using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;
    
    [Header("Pop Up")]
    [SerializeField] private float popUpDuration;
    [SerializeField] private Transform popUpWindow;
    [SerializeField] private GameObject itemPopUpInstance;
    [SerializeField] private GameObject textPopUpInstance;

    [Header("Interaction")]
    public GameObject interactionIcon; //A HUD element that output for the player if a interactable is in reach

    void Start()
    {
        Instance = this;
    }

    public void CollectedItemPopUp(Item item)
    {
        GameObject popUp = Instantiate(itemPopUpInstance, popUpWindow);
        popUp.GetComponentInChildren<TMP_Text>().text = item.itemName;
        popUp.transform.GetChild(1).GetComponent<Image>().sprite = item.icon;

        HUDEffects.Instance.FadeInAndOut(popUp, popUpDuration, true);
    }

    public void TextPopUp(string text)
    {
        GameObject popUp = Instantiate(textPopUpInstance, popUpWindow);
        popUp.GetComponentInChildren<TMP_Text>().text = text;

        HUDEffects.Instance.FadeInAndOut(popUp, popUpDuration, true);
    }

    public void ChangeInteractionIcon(bool state)
    {
        interactionIcon.SetActive(state);
    }
}
