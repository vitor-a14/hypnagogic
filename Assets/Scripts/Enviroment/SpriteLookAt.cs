using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLookAt : MonoBehaviour
{
    private Transform camTransform;

    private void Start() 
    {
        camTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(camTransform);
    }
}
