using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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

        FadeInAndOut(popUp, popUpDuration, 15);
        Destroy(popUp, popUpDuration * 2);
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
