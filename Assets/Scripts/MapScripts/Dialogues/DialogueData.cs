using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

[System.Serializable]
public class Choice
{
    public string choiceText;
    public DialogueData nextDialogue;

    [Tooltip("Akcja do wykonania gdy gracz wybierze tę opcję")]
    public DialogueAction rewardAction;

    public UnityEvent onChoiceSelected; // TO DODAJEMY: okno na Twoje skrypty w Inspektorze
}

[CreateAssetMenu(fileName = "NowyDialog", menuName = "Dialogi/Paczki dialogowe")]
public class DialogueData : ScriptableObject
{
    public string characterName;
    public Sprite characterPortrait;
    public LocalizedString[] sentences;

    [Tooltip("Lista akcji do wykonania po zamknięciu dialogu")]
    public List<DialogueAction> endActions;

    public Choice[] choices;
}