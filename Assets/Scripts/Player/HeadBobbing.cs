using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    public bool isActive; //Optional for the player

    [SerializeField, Range(0, 0.05f)] private float amplitude = 0.015f; 
    [SerializeField, Range(0, 30)] private float frequency = 10.0f;

    private Vector3 startPos;
    [SerializeField] private Transform cam; //Make the headbobbing movement
    [SerializeField] private Transform cameraHolder; //Make the actual camera look straight

    [SerializeField] private float landingTriggerHeight; //Min height to trigger fall animation
    private bool canLand = true;

    private void Start()
    {
        startPos = cam.localPosition;
    }

    private void FixedUpdate()
    {
        if(!isActive) return;

        CheckMotion();
        ResetPosition();
        cam.LookAt(FocusTarget());

        if(!Physics.Raycast(transform.position, -transform.up, landingTriggerHeight))
            canLand = true;

        //Apply landing animation
        if(PlayerController.Instance.onGround && canLand)
        {
            cam.localPosition = Vector3.up * -0.3f;
            canLand = false;
        }
    }

    //Check if the head bobbing need to be applied based on player velocity
    private void CheckMotion()
    {
        if(!PlayerController.Instance.aboveToggleSpeed || !PlayerController.Instance.onGround) return;

        PlayMotion(FootStepMotion());
    }

    //Animation math
    private Vector3 FootStepMotion()
    {
        float actualFrequency = frequency;
        if(PlayerController.Instance.isRunning) 
            actualFrequency *= PlayerController.Instance.runMultiplier;
        else if(PlayerController.Instance.isCrounching) 
            actualFrequency *= PlayerController.Instance.crouchingMultiplier;

        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * actualFrequency) * amplitude;
        pos.x += Mathf.Cos(Time.time * actualFrequency / 2) * amplitude * 2;
        return pos;
    }

    private void ResetPosition()
    {
        if(cam.localPosition == startPos) return;
        cam.localPosition = Vector3.Lerp(cam.localPosition, startPos, 2 * Time.fixedDeltaTime);
    }

    private void PlayMotion(Vector3 motion)
    {
        cam.localPosition += motion;
    }

    //Make the camera look in a straight line 
    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraHolder.localPosition.y, transform.position.z);
        pos += cameraHolder.forward * 15f;
        return pos;
    }
}
