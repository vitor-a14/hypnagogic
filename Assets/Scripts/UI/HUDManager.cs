using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;
    
    [Header("Pop Up")]
    [SerializeField] private float popUpDuration;
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject itemPopUpInstance;
    [SerializeField] private GameObject textPopUpInstance;

    [Header("Interaction")]
    public GameObject interactionIcon; //A HUD element that output for the player if a interactable is in reach
    private TMP_Text interactionText;

    [Header("Player HUD")]
    [SerializeField] private CanvasGroup playerHUD;
    public Slider healthBarSlider; //the max value is changed in the PlayerStatus class everytime the max life changes

    [SerializeField] private CanvasGroup playerHUDcurrentWeapon;
    [SerializeField] private Image playerHUDcurrentWeaponIcon;
    [SerializeField] private TMP_Text playerHUDcurrentWeaponName;

    void Start()
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.Log("Instance is already set, something is wrong");

        interactionText = interactionIcon.GetComponent<TMP_Text>();
        healthBarSlider.maxValue = PlayerStatus.Instance.maxLife; //This updates in the PlayerStatus class

        FadeIn(playerHUD.gameObject, 5);
    }

    //Player UI Logic
    private void LateUpdate() 
    {
        healthBarSlider.value = PlayerStatus.Instance.currentLife;
    }

    public void CollectedItemPopUp(Item item)
    {
        GameObject popUp = Instantiate(itemPopUpInstance, canvas);
        popUp.GetComponentInChildren<TMP_Text>().text = item.itemName;
        popUp.transform.GetChild(1).GetComponent<Image>().sprite = item.icon;

        FadeInAndOut(popUp, popUpDuration, 15);
        Destroy(popUp, popUpDuration * 2);
    }

    public void SetPlayerHUDCurrentWeapon(Item item)
    {
        if(item == null)
        {
            playerHUDcurrentWeapon.alpha = 0;
            return;
        } 
        else playerHUDcurrentWeapon.alpha = 1;

        playerHUDcurrentWeaponIcon.sprite = item.icon;
        playerHUDcurrentWeaponName.text = item.name;
    }

    public void TextPopUp(string text)
    {
        GameObject popUp = Instantiate(textPopUpInstance, canvas);
        popUp.GetComponentInChildren<TMP_Text>().text = text;

        Destroy(popUp, popUpDuration);
    }

    public void ChangeInteractionIcon(bool state, string text)
    {
        interactionIcon.SetActive(state);
        interactionText.text = text;
    }

    public void FadeInAndOut(GameObject element, float duration, float fadeRate)
    {
        StartCoroutine(FadeInAndOutCoroutine(element, duration, fadeRate));
    }

    public void FadeIn(GameObject element, float fadeRate)
    {
        StartCoroutine(FadeInCoroutine(element, fadeRate));
    }

    public void FadeOut(GameObject element, float fadeRate)
    {
        StartCoroutine(FadeOutCoroutine(element, fadeRate));
    }

    private IEnumerator FadeInCoroutine(GameObject element, float fadeRate) 
    {
        CanvasGroup canvas = element.GetComponent<CanvasGroup>();
        float targetAlpha = 1.0f;
        float alpha = 0f;

        while(Mathf.Abs(targetAlpha - alpha) > 0.0001f) 
        {
            alpha = Mathf.Lerp(alpha, targetAlpha, fadeRate * Time.deltaTime);
            canvas.alpha = alpha;
            yield return null;
        }
    }

    private IEnumerator FadeOutCoroutine(GameObject element, float fadeRate) 
    {
        CanvasGroup canvas = element.GetComponent<CanvasGroup>();
        float targetAlpha = 0f;
        float alpha = 1f;

        while(Mathf.Abs(targetAlpha - alpha) > 0.0001f) 
        {
            alpha = Mathf.Lerp(alpha, targetAlpha, fadeRate * Time.deltaTime);
            canvas.alpha = alpha;
            yield return null;
        }
    }

    private IEnumerator FadeInAndOutCoroutine(GameObject element, float duration, float fadeRate)
    {
        CanvasGroup canvas = element.GetComponent<CanvasGroup>();

        FadeIn(element, fadeRate);
        yield return new WaitForSeconds(duration);
        FadeOut(element, fadeRate);
    }
}
