using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class chat_box : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject dialogVisuals;
    public TextMeshProUGUI textDisplay;
    public Image portraitDisplay;
    public GameObject choiceContainer;
    public GameObject buttonPrefab;

    [Header("Input Settings")]
    public Key primaryKey = Key.Z;
    public Key secondaryKey = Key.Enter;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public bool isOpen;
    public Color outlineHighlightColor = Color.red;

    [Header("Margins & Spacing")]
    public int marginLeft = 20;
    public int marginRight = 20;
    public int marginTop = 10;
    public int marginBottom = 10;
    public float buttonSpacing = 15f;

    private DialogueData currentData;
    private string[] sentences;
    private int index;
    private bool isTyping;

    void Awake()
    {
        isOpen = false;
        if (choiceContainer != null) choiceContainer.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        isOpen = false;
        isTyping = false;
        StopAllCoroutines();
    }

    public void StartDialogue(DialogueData data)
    {
        if (data == null ||
           ((data.sentences == null || data.sentences.Length == 0) &&
            (data.choices == null || data.choices.Length == 0)))
        {
            return;
        }

        gameObject.SetActive(true);
        isOpen = true;

        if (dialogVisuals != null) dialogVisuals.SetActive(true);

        StopAllCoroutines();
        isTyping = false;
        if (choiceContainer != null) choiceContainer.SetActive(false);

        currentData = data;
        sentences = data.sentences;
        index = 0;

        if (portraitDisplay != null)
        {
            portraitDisplay.sprite = data.characterPortrait;
            portraitDisplay.gameObject.SetActive(data.characterPortrait != null);
        }

        if (sentences != null && sentences.Length > 0)
        {
            StartCoroutine(Type());
        }
        else
        {
            ShowChoices();
        }
    }

    IEnumerator Type()
    {
        isTyping = true;
        textDisplay.text = "";
        if (index < sentences.Length)
        {
            foreach (char letter in sentences[index].ToCharArray())
            {
                textDisplay.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        isTyping = false;
    }

    void Update()
    {
        if (!isOpen) return;
        if (choiceContainer.activeSelf) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard[primaryKey].wasPressedThisFrame || keyboard[secondaryKey].wasPressedThisFrame)
        {
            if (isTyping)
            {
                StopAllCoroutines();
                textDisplay.text = sentences[index];
                isTyping = false;
            }
            else
            {
                NextSentence();
            }
        }
    }

    void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            StartCoroutine(Type());
        }
        else
        {
            if (currentData != null && currentData.choices != null && currentData.choices.Length > 0)
                ShowChoices();
            else
                CloseDialogue();
        }
    }

    void ShowChoices()
    {
        textDisplay.text = "";

        if (currentData.choices == null || currentData.choices.Length == 0)
        {
            CloseDialogue();
            return;
        }

        choiceContainer.SetActive(true);
        choiceContainer.transform.SetAsLastSibling();

        foreach (Transform child in choiceContainer.transform) Destroy(child.gameObject);

        VerticalLayoutGroup vg = choiceContainer.GetComponent<VerticalLayoutGroup>();
        if (vg == null) vg = choiceContainer.AddComponent<VerticalLayoutGroup>();

        vg.padding = new RectOffset(0, 0, 0, 0);
        vg.spacing = 0;

        // ZMIANA: W³¹czamy wymuszanie wysokoœci rzêdów, aby kontener je œciska³
        vg.childControlHeight = true;
        vg.childForceExpandHeight = true;
        vg.childControlWidth = true;
        vg.childForceExpandWidth = true;

        int totalChoices = currentData.choices.Length;
        int columns = Mathf.CeilToInt(Mathf.Sqrt(totalChoices));

        GameObject currentRow = null;
        List<GameObject> allButtons = new List<GameObject>();

        for (int i = 0; i < totalChoices; i++)
        {
            if (i % columns == 0) currentRow = CreateNewRow();
            allButtons.Add(CreateButton(currentData.choices[i], currentRow.transform));
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        StartCoroutine(CheckMouseHover(allButtons));
    }

    IEnumerator CheckMouseHover(List<GameObject> buttons)
    {
        yield return new WaitForEndOfFrame();
        if (Mouse.current == null) yield break;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        foreach (GameObject btnObj in buttons)
        {
            if (btnObj == null) continue;
            if (RectTransformUtility.RectangleContainsScreenPoint(btnObj.GetComponent<RectTransform>(), mousePos, null))
            {
                PointerEventData data = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(btnObj, data, ExecuteEvents.pointerEnterHandler);
                break;
            }
        }
    }

    GameObject CreateNewRow()
    {
        GameObject rowObj = new GameObject("Row", typeof(RectTransform));
        rowObj.transform.SetParent(choiceContainer.transform, false);

        RectTransform rt = rowObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(0.5f, 1f);

        HorizontalLayoutGroup hg = rowObj.AddComponent<HorizontalLayoutGroup>();
        hg.padding = new RectOffset(marginLeft, marginRight, marginTop, marginBottom);
        hg.spacing = buttonSpacing;
        hg.childControlWidth = true;
        hg.childControlHeight = true;
        hg.childForceExpandWidth = true;
        hg.childForceExpandHeight = true;

        // ZMIANA: Zamiast sztywnego preferredHeight, u¿ywamy flexibleHeight.
        // Dziêki temu przy ma³ej iloœci przycisków bêd¹ du¿e, a przy du¿ej - same siê œcisn¹.
        LayoutElement le = rowObj.AddComponent<LayoutElement>();
        le.flexibleHeight = 1;

        return rowObj;
    }

    GameObject CreateButton(Choice choice, Transform parent)
    {
        GameObject btnObj = Instantiate(buttonPrefab, parent);
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

        UnityEngine.UI.Button b = btnObj.GetComponent<UnityEngine.UI.Button>();
        b.onClick.AddListener(() => {
            if (choice.onChoiceSelected != null) choice.onChoiceSelected.Invoke();
            SelectChoice(choice.nextDialogue);
        });

        b.transition = Selectable.Transition.None;

        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.None;
        b.navigation = nav;

        Outline outline = btnObj.GetComponent<Outline>();
        Color basicColor = (outline != null) ? outline.effectColor : Color.black;

        EventTrigger trigger = btnObj.GetComponent<EventTrigger>();
        if (trigger == null) trigger = btnObj.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { SetOutlineColor(btnObj, outlineHighlightColor); });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { SetOutlineColor(btnObj, basicColor); });
        trigger.triggers.Add(entryExit);

        return btnObj;
    }

    void SetOutlineColor(GameObject obj, Color color)
    {
        if (obj == null) return;
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null) outline.effectColor = color;
    }

    public void SelectChoice(DialogueData nextData)
    {
        choiceContainer.SetActive(false);
        if (nextData != null)
        {
            StartDialogue(nextData);
        }
        else
        {
            CloseDialogue();
        }
    }

    void CloseDialogue()
    {
        StopAllCoroutines();
        index = 0;
        isOpen = false;
        gameObject.SetActive(false);
    }
}