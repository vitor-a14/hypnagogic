using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    public bool isActive;

    [SerializeField, Range(0, 0.1f)] private float amplitude = 0.015f;
    [SerializeField, Range(0, 30)] private float frequency = 10.0f;

    private float toggleSpeed = 3f;
    private Vector3 startPos;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform cameraHolder;

    [SerializeField] private float landingTriggerHeight;
    private bool canLand = true;

    void Start()
    {
        startPos = cam.localPosition;
    }

    void Update()
    {
        if(!isActive) return;

        CheckMotion();
        ResetPosition();
        cam.LookAt(FocusTarget());

        if(!Physics.Raycast(transform.position, -transform.up, landingTriggerHeight))
        {
            canLand = true;
        }

        if(PlayerController.Instance.onGround && canLand)
        {
            cam.localPosition = Vector3.up * -0.3f;
            canLand = false;
        }
    }

    private void CheckMotion()
    {
        float speed = PlayerController.Instance.rigid.velocity.magnitude;
        if(speed < toggleSpeed || !PlayerController.Instance.onGround) return;

        PlayMotion(FootStepMotion());
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        return pos;
    }

    private void ResetPosition()
    {
        if(cam.localPosition == startPos) return;
        cam.localPosition = Vector3.Lerp(cam.localPosition, startPos, 2 * Time.deltaTime);
    }

    private void PlayMotion(Vector3 motion)
    {
        cam.localPosition += motion;
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraHolder.localPosition.y, transform.position.z);
        pos += cameraHolder.forward * 15f;
        return pos;
    }
}
