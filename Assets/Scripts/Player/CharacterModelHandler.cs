using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModelHandler : MonoBehaviour
{
    private static readonly int idle = Animator.StringToHash("Idle");
    private static readonly int walk = Animator.StringToHash("Walk");
    private static readonly int run = Animator.StringToHash("Run");
    private static readonly int crouch = Animator.StringToHash("Crouch");
    private static readonly int fall = Animator.StringToHash("Fall");

    private Animator anim;
    private int currentState, state;
    [SerializeField] private Transform cam;

    [SerializeField] private float standingPosY;
    [SerializeField] private float crouchingPosY;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        var rot = Quaternion.LookRotation(cam.forward);
        rot.x = 0;
        rot.z = 0;
        transform.rotation = rot;

        state = GetState();

        if(state == crouch)
            transform.localPosition = new Vector3(transform.localPosition.x, crouchingPosY, transform.localPosition.z);
        else
            transform.localPosition = new Vector3(transform.localPosition.x, standingPosY, transform.localPosition.z);

        if(state == currentState) return;
        anim.CrossFade(state, 0, 0);
        currentState = state;
    }

    private int GetState()
    {
        anim.speed = 1;
        
        if(!PlayerController.Instance.onGround) return fall;

        if(PlayerController.Instance.isCrounching)
        {
            if(!PlayerController.Instance.aboveToggleSpeed) anim.speed = 0;
            return crouch;
        }

        if(!PlayerController.Instance.aboveToggleSpeed) return idle;

        if(PlayerController.Instance.isRunning) return run;
        else return walk;
    }
}
