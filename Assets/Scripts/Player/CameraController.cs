using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Camera movement config's
    public float sensivity;
    public float maxAngle;

    //Movement setup
    Vector2 input;
    Vector3 camRotation;
    Inputs controls;

    void Start()
    {
        controls = new Inputs();
        controls.Enable();

        controls.Player.Camera.performed += ctx => input = ctx.ReadValue<Vector2>();
        controls.Player.Camera.canceled += ctx => input = Vector2.zero;
    }

    void Update()
    {
        camRotation.y += input.x * sensivity * Time.deltaTime;
        camRotation.x -= input.y * sensivity * Time.deltaTime;
        camRotation.x = Mathf.Clamp(camRotation.x, -maxAngle, maxAngle); 
    }

    private void FixedUpdate() 
    {
        transform.rotation = Quaternion.Euler(camRotation);
    }
}
