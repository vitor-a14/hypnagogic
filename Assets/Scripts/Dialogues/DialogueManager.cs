using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    private Story currentStory;
    public bool dialogueIsPlaying;

    private void Start()
    {
        dialogueIsPlaying = false;
        Instance = this;
        dialoguePanel.SetActive(false);

        PlayerController.Instance.controls.Player.Dialogue.performed += ctx => ContinueDialogue();
    }

    public void StartDialogue(TextAsset inkJson)
    {
        currentStory = new Story(inkJson.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true); 

        if(currentStory.canContinue) //Check if there is more dialogue to play
            dialogueText.text = currentStory.currentText;
        else
            EndDialogue();
    }

    public void ContinueDialogue() 
    {
        if(!dialogueIsPlaying) return;
        
        if(currentStory.canContinue) //Check if there is more dialogue to play
            dialogueText.text = currentStory.Continue();
        else
            EndDialogue();
    }

    public void EndDialogue()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
}
