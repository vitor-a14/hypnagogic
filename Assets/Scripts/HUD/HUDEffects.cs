using System.Collections;
using UnityEngine;

public class HUDEffects : MonoBehaviour
{
    public static HUDEffects Instance { get; private set; }
    public static float fadeDuration = 0.18f;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else 
            Debug.Log("Instance is already set, something is wrong.");
    }

    public void FadeUI(GameObject uiElement, bool fadeIn)
    {
        StartCoroutine(UIFadeCoroutine(uiElement, fadeIn));
    }

    public void FadeInAndOut(GameObject uiElement, float duration, bool destroyAfter)
    {
        StartCoroutine(UIFadeInAndOutCoroutine(uiElement, duration, destroyAfter));
    }

    private IEnumerator UIFadeInAndOutCoroutine(GameObject uiElement, float duration, bool destroyAfter)
    {
        StartCoroutine(UIFadeCoroutine(uiElement, true));
        yield return new WaitForSeconds(duration);
        StartCoroutine(UIFadeCoroutine(uiElement, false));
        yield return new WaitForSeconds(fadeDuration);
        if(destroyAfter)
            Destroy(uiElement);
    }

    private IEnumerator UIFadeCoroutine(GameObject uiElement, bool fadeIn)
    {
        CanvasGroup canvas = uiElement.GetComponent<CanvasGroup>();
        if(canvas == null)
        {
            Debug.LogError("The object does not have a canvas group element!");
            yield return null;
        }

        float t = 0;

        if(fadeIn)
        {
            uiElement.SetActive(true);
            canvas.alpha = 0;
        }

        while(t < fadeDuration)
        {
            if(fadeIn)
                canvas.alpha = Mathf.Lerp(canvas.alpha, 1, t);
            else
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0, t);

            t += Time.fixedDeltaTime;
            yield return null;
        }

        if(fadeIn)
            canvas.alpha = 1;
        else
        {
            canvas.alpha = 0;
            uiElement.SetActive(false);
        }

        yield return null;
    }
}
