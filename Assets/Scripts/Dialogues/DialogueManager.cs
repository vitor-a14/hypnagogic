using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.UI;

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
    private Button[] choicesButton;

    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJson;

    private Story currentStory;
    public bool dialogueIsPlaying;

    private DialogueVariables dialogueVariables;
    private CanvasGroup dialogueCanvas;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");

        dialogueVariables = new DialogueVariables(loadGlobalsJson);
        dialogueCanvas = dialoguePanel.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        PlayerController.Instance.controls.Player.Dialogue.performed += ctx => ContinueDialogue();
 
        choicesText = new TMP_Text[choices.Length];
        choicesButton = new Button[choices.Length];
        int index = 0;
        foreach(GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TMP_Text>();
            choicesButton[index] = choice.GetComponent<Button>();
            index++;
        }
    }

    public void StartDialogue(TextAsset inkJson)
    {
        PlayerController.Instance.UseMouse(true);
        currentStory = new Story(inkJson.text);
        dialogueIsPlaying = true;
        HUDEffects.Instance.FadeUI(dialoguePanel, true);

        dialogueVariables.StartListening(currentStory);

        dialogueText.text = currentStory.currentText;
        StartCoroutine(DisplayChoicesCoroutine());
        HandleTags(currentStory.currentTags);
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

    private void HandleTags(List<string> currentTags)
    {
        foreach(string tag in currentTags)
        {
            string[] splitTag = tag.Split(":");
            if(splitTag.Length != 2)
                Debug.LogError("Tag could not be appropriately parsed: " + tag);

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            //if more than one tag exists, put in a switch loop below, instead of a if
            if(tagKey == "speaker")
            {
                npcName.text = tagValue;
            }
            else
                Debug.Log("Tag came in but is not currently being handled: " + tag);
        }
    }
    
    private IEnumerator DisplayChoicesCoroutine()
    {
        yield return new WaitForEndOfFrame();
        DisplayChoices();
        HandleTags(currentStory.currentTags);
    }

    public void EndDialogue()
    {
        dialogueVariables.StopListening(currentStory);
        dialogueIsPlaying = false;
        HUDEffects.Instance.FadeUI(dialoguePanel, false);
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
            choicesButton[index].interactable = true;
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        // Go to the remaining choices and disable
        for(int i = index; i < choices.Length; i++)
        {
            choicesButton[i].interactable = false;
            choices[i].gameObject.SetActive(false);
        }
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if(variableValue == null)
            Debug.LogWarning("Ink variable was found to be null: " + variableName);

        return variableValue;
    }
}
