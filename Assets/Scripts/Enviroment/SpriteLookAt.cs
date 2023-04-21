using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLookAt : MonoBehaviour
{
    [SerializeField] private Transform camTransform;

    void LateUpdate()
    {
        transform.LookAt(camTransform);
    }
}
