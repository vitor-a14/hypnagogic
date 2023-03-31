using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueManager : Interactable
{
    [SerializeField] private TextAsset inkJson;
    public override void Interact()
    {
        base.Interact();

        if(!InventoryManager.Instance.onInventory && !DialogueManager.Instance.dialogueIsPlaying)
            DialogueManager.Instance.StartDialogue(inkJson);
    }
}
