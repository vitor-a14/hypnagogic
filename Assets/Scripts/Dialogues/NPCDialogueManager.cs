using System.Collections;
using UnityEngine;

public class NPCDialogueManager : Interactable
{
    public Transform camTransform;
    public Transform headBone;
    public Vector3 offset;
    private Vector3 lookPos;

    [SerializeField] private TextAsset inkJson;

    private void Start() {
        lookPos = headBone.position + offset;
    }

    public override void Interact()
    {
        base.Interact();

        if(!InventoryManager.Instance.onInventory && !DialogueManager.Instance.dialogueIsPlaying)
        {
            DialogueManager.Instance.StartDialogue(inkJson);
            StopAllCoroutines();
            StartCoroutine(CameraLookAt());
        }
    }

    private IEnumerator CameraLookAt()
    {
        float t = 0;

        while(t < 1)
        {
            Vector3 direction = lookPos - camTransform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            camTransform.rotation = Quaternion.Slerp(camTransform.rotation, targetRotation, 5 * Time.deltaTime);
            t += Time.deltaTime;

            yield return null;
        }
    }
}
