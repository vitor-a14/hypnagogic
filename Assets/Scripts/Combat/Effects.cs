using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public static Effects Instance;
    public float duration;
    private bool waiting;
    [SerializeField] private Transform cam;

    private void Awake() 
    {
        //Set a global static instance for weapons to acess
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");
    }

    private IEnumerator Freeze()
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;
    }

    private IEnumerator Shake()
    {
        Vector3 startPos = cam.position;
        float elapsedTime = 0.0f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            cam.position = startPos + Random.insideUnitSphere * 0.05f;
            yield return null;
        }

        cam.position = startPos;
    }

    public void FreezeFrame()
    {
        if(waiting) return;

        Time.timeScale = 0.0f;
        StartCoroutine(Freeze());
    }

    public void ScreenShake()
    {
        StartCoroutine(Shake());
    }
}
