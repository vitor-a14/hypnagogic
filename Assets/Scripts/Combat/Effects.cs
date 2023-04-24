using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public static Effects Instance;
    private bool waiting;
    [SerializeField] private Transform cam;

    private void Awake() 
    {
        //Set a global static instance for weapons to acess
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");

        Time.timeScale = 1;
    }

    private IEnumerator Freeze(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;
    }

    private IEnumerator Shake(float duration)
    {
        Vector3 startPos = cam.position;
        float elapsedTime = 0.0f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            cam.position = startPos + Random.insideUnitSphere * 0.05f;
            yield return null;
        }

        cam.localPosition = Vector3.zero;
    }

    public void FreezeFrame(float duration)
    {
        if(waiting) return;

        Time.timeScale = 0.1f;
        StartCoroutine(Freeze(duration));
    }

    public void ScreenShake(float duration)
    {
        StartCoroutine(Shake(duration));
    }
}
