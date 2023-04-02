using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Movement")]
    public float sensivity;
    public float maxAngle;

    [Header("Interaction System")]
    public float interactionDistance;
    public LayerMask interactableLayer;

    //Movement setup
    Vector2 input;
    Vector3 camRotation;
    Transform entityOnReach; // get the interactable entity that the player is aiming

    void Start()
    {
        PlayerController.Instance.controls.Player.Camera.performed += ctx => input = ctx.ReadValue<Vector2>();
        PlayerController.Instance.controls.Player.Camera.canceled += ctx => input = Vector2.zero;
        PlayerController.Instance.controls.Player.Interact.performed += ctx => InteractWithEnviroment();
    }

    void Update()
    {
        if(DialogueManager.Instance.dialogueIsPlaying || InventoryManager.Instance.onInventory) 
        {
            HUDManager.Instance.ChangeInteractionIcon(false);
            return;
        }

        camRotation.y += input.x * sensivity * Time.deltaTime;
        camRotation.x -= input.y * sensivity * Time.deltaTime;
        camRotation.x = Mathf.Clamp(camRotation.x, -maxAngle, maxAngle); 

        entityOnReach = GetEntityOnReach();
    }

    private Transform GetEntityOnReach() 
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, interactableLayer)) 
        {
            HUDManager.Instance.ChangeInteractionIcon(true);
            return hit.transform;
        }

        HUDManager.Instance.ChangeInteractionIcon(false);
        return null;
    }

    private void InteractWithEnviroment()
    {   
        if(entityOnReach != null && entityOnReach.GetComponent<Interactable>())
            entityOnReach.GetComponent<Interactable>().Interact();
    }

    private void FixedUpdate() 
    {
        transform.rotation = Quaternion.Euler(camRotation);
    }
}
