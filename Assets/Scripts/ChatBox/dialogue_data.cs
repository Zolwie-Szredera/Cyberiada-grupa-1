using UnityEngine;
using UnityEngine.Events; // Potrzebne do obs³ugi zdarzeñ

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

    [TextArea(3, 10)]
    public string[] sentences;

    public Choice[] choices;
}