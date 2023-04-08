using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;
    
    [Header("Pop Up")]
    [SerializeField] private float popUpDuration;
    [SerializeField] private Transform popUpWindow;
    [SerializeField] private GameObject popUpInstance;

    [Header("Interaction")]
    public GameObject interactionIcon; //A HUD element that output for the player if a interactable is in reach

    void Start()
    {
        Instance = this;
    }

    public void TriggerPopUp(string message)
    {
        GameObject popUp = Instantiate(popUpInstance, popUpWindow);
        popUp.GetComponentInChildren<TMP_Text>().text = message;

        Destroy(popUp, popUpDuration);
    }

    public void ChangeInteractionIcon(bool state)
    {
        interactionIcon.SetActive(state);
    }
}
