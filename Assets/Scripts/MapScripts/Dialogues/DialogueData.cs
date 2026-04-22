using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

[System.Serializable]
public class Choice
{
    public string choiceText;
    public DialogueData nextDialogue;
    public UnityEvent onChoiceSelected; // TO DODAJEMY: okno na Twoje skrypty w Inspektorze
}

[CreateAssetMenu(fileName = "NowyDialog", menuName = "Dialogi/Paczki dialogowe")]
public class DialogueData : ScriptableObject
{
    public string characterName;
    public Sprite characterPortrait;
    public LocalizedString[] sentences;

    public Choice[] choices;
}