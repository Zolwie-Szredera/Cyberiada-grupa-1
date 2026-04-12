using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization; // Potrzebne do obs³ugi lokalizacji

[System.Serializable]
public class Choice
{
    public LocalizedString choiceText; // ZMIENIONO ze string
    public DialogueData nextDialogue;
    public UnityEvent onChoiceSelected;
}

[CreateAssetMenu(fileName = "NowyDialog", menuName = "Dialogi/Paczki dialogowe")]
public class DialogueData : ScriptableObject
{
    public string characterName;
    public Sprite characterPortrait;

    // ZMIENIONO ze string[] na LocalizedString[]
    public LocalizedString[] sentences;

    public Choice[] choices;
}