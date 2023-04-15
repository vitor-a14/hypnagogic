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
    private TMP_Text interactionText;

    void Start()
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.Log("Instance is already set, something is wrong");

        interactionText = interactionIcon.GetComponent<TMP_Text>();
    }

    public void CollectedItemPopUp(Item item)
    {
        GameObject popUp = Instantiate(itemPopUpInstance, popUpWindow);
        popUp.GetComponentInChildren<TMP_Text>().text = item.itemName;
        popUp.transform.GetChild(1).GetComponent<Image>().sprite = item.icon;

        Destroy(popUp, popUpDuration);
    }

    public void TextPopUp(string text)
    {
        GameObject popUp = Instantiate(textPopUpInstance, popUpWindow);
        popUp.GetComponentInChildren<TMP_Text>().text = text;

        Destroy(popUp, popUpDuration);
    }

    public void ChangeInteractionIcon(bool state, string text)
    {
        interactionIcon.SetActive(state);
        interactionText.text = text;
    }
}
