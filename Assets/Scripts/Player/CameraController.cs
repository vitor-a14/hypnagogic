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
    private Vector2 input;
    private Vector3 camRotation;
    private Transform entityOnReach; // get the interactable entity that the player is aiming

    private void Start()
    {
        PlayerController.Instance.controls.Player.Camera.performed += ctx => input = ctx.ReadValue<Vector2>();
        PlayerController.Instance.controls.Player.Camera.canceled += ctx => input = Vector2.zero;
        PlayerController.Instance.controls.Player.Interact.performed += ctx => InteractWithEnviroment();
    }

    private void Update()
    {
        if(DialogueManager.Instance.dialogueIsPlaying || InventoryManager.Instance.onInventory) 
        {
            HUDManager.Instance.ChangeInteractionIcon(false, "");
            return;
        }

        camRotation.y += input.x * sensivity * Time.fixedDeltaTime;
        camRotation.x -= input.y * sensivity * Time.fixedDeltaTime;
        camRotation.x = Mathf.Clamp(camRotation.x, -maxAngle, maxAngle); 

        entityOnReach = GetEntityOnReach();
    }

    private Transform GetEntityOnReach() 
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, interactableLayer)) 
        {
            string text = "Interact";

            if (hit.transform.tag == "NPC")
                text = "Talk";
            else if (hit.transform.tag == "Item")
                text = "Pick Item";
            else if (hit.transform.tag == "Interact")
                text = "Interact";

            HUDManager.Instance.ChangeInteractionIcon(true, text);
            return hit.transform;
        }

        HUDManager.Instance.ChangeInteractionIcon(false, "");
        return null;
    }

    private void InteractWithEnviroment()
    {   
        if(entityOnReach != null && entityOnReach.GetComponent<Interactable>() && !DialogueManager.Instance.dialogueIsPlaying && !InventoryManager.Instance.onInventory)
            entityOnReach.GetComponent<Interactable>().Interact();
    }

    private void LateUpdate() 
    {
        if(DialogueManager.Instance.dialogueIsPlaying || InventoryManager.Instance.onInventory)
        {
            camRotation = transform.rotation.eulerAngles;
        }

        transform.rotation = Quaternion.Euler(camRotation);
    }
}
