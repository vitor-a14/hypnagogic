using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text npcName;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TMP_Text[] choicesText;

    private Story currentStory;
    public bool dialogueIsPlaying;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        PlayerController.Instance.controls.Player.Dialogue.performed += ctx => ContinueDialogue();

        choicesText = new TMP_Text[choices.Length];
        int index = 0;
        foreach(GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TMP_Text>();
            index++;
        }
    }

    public void StartDialogue(TextAsset inkJson)
    {
        PlayerController.Instance.UseMouse(true);
        currentStory = new Story(inkJson.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true); 

        if(currentStory.canContinue) //Check if there is more dialogue to play
        {
            dialogueText.text = currentStory.currentText;
            DisplayChoices();
        }
        else
            EndDialogue();
    }

    public void ContinueDialogue() 
    {
        if(!dialogueIsPlaying || currentStory.currentChoices.Count > 0) return;
        
        if(currentStory.canContinue) //Check if there is more dialogue to play
        {
            dialogueText.text = currentStory.Continue();
            StartCoroutine(DisplayChoicesCoroutine());
        }
        else
            EndDialogue();
    }

    private IEnumerator DisplayChoicesCoroutine()
    {
        yield return new WaitForEndOfFrame();
        DisplayChoices();
    }

    public void EndDialogue()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        PlayerController.Instance.UseMouse(false);
        dialogueText.text = "";
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueDialogue();
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        // Defensive check
        if(currentChoices.Count > choices.Length)
            Debug.LogError("More choices were given than the UI can support. Increase the number of UI choice buttons.");

        int index = 0;

        // Enable and initialize the choices UI up to the amount of the choices in the dialogue
        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        // Go to the remaining choices and disable
        for(int i = index; i < choices.Length; i++)
            choices[i].gameObject.SetActive(false);
    }
}
