using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueVariables 
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }
    public Story globalVariablesStory;

    public DialogueVariables(TextAsset loadGlobalsJson)
    {
        //Create the story
        globalVariablesStory = new Story(loadGlobalsJson.text);
    }

    public string GetVariables()
    {
        if(globalVariablesStory == null) return "";

        VariableToStory(globalVariablesStory);
        return globalVariablesStory.state.ToJson();
    }

    public void LoadVariables(string variablesJson)
    {
        globalVariablesStory.state.LoadJson(variablesJson);
    }

    //Load variables into the ink system
    public void InitializeDictionary()
    {
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach(string name in globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
        }
    }

    public void StartListening(Story story)
    {
        //It's important that VariableToStory is before assigning to the listener!
        VariableToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }

    private void VariableChanged(string name, Ink.Runtime.Object value)
    {
        Debug.Log("Variable changed: " + name + " = " + value);

        //Only maitain variables that were initialized from the globals ink file
        if(variables.ContainsKey(name))
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    private void VariableToStory(Story story)
    {
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }
}
