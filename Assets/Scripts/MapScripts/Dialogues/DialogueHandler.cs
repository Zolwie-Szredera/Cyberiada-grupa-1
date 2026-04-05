using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class DialogueHandler : MonoBehaviour
{
    //TODO: I think choices should be handled in a separate script/scriptable object
    [Header("Required References")]
    public TextMeshProUGUI textDisplay;
    public GameObject dialogueCanvas; //the one with text and choices
    public GameObject mainCanvas; //so it can be disabled when dialogue is active. It's the one with HP bar
    public Animator playerAnimator;
    [Header("Choice UI Reference")]
    public GameObject choiceContainer;
    public GameObject buttonPrefab;
    [Header("Settings")]
    public float typingSpeed = 0.05f;
    [Range(0, 1)] public float volume = 0.5f;
    [Header("Settings for choices")]
    public int marginLeft = 20;
    public int marginRight = 20;
    public int marginTop = 10;
    public int marginBottom = 10;
    public float buttonSpacing = 15f;
    public Color outlineHighlightColor = Color.red;
    //-----------------------------
    [HideInInspector] public bool isOpen = false;
    private bool pauseDialogue = false;
    private AudioSource audioSource;
    private PauseMenu pause;
    private Rigidbody2D playerRb;
    private DialogueData currentData;
    private string[] sentences;
    private int index;
    private bool isTyping;
    private bool noNewDialogue = false;
    private static WaitForSeconds _waitForSeconds0_2 = new(0.2f);
    public void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogWarning("Player not found in the scene. That's ok if your in the main menu.");
            return;
        }
        audioSource = GetComponent<AudioSource>();
        pause = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PauseMenu>();
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }
    public void ContinueDialogue()
    {
        if(pauseDialogue) return; //used to prevent advancing dialogue while paused
        if (!isOpen) //start dialogue. This shouldn't happen here, special button type starts dialogue directly, but just in case
        {
            StartDialogue(currentData);
        }
        else if (!isTyping) //go to next sentence
        {
            NextSentence();
        }
        else //finish sentence
        {
            StopAllCoroutines();
            textDisplay.text = sentences[index];
            isTyping = false;
        }
    }
    public void OnContinueDialogue(InputAction.CallbackContext context) //so that attacking also advances dialogue, can be changed to something else if needed
    {
        if (context.started && isOpen)
        {
            ContinueDialogue();
        }
    }
    public void StartDialogue(DialogueData data)
    {
        if (noNewDialogue) //I did that so E doesn't start a new dialogue immediately after closing one
        {
            return;
        }
        if (data == null || data.sentences == null || data.sentences.Length == 0)
        {
            Debug.LogWarning("Incomplete data in dialogue");
            return;
        } //assume all data is correct, no null checks for sentences and choices later on
        if(data.choices == null || data.choices.Length == 0)
        {
            Debug.Log("No choices in dialogue.");
        }
        dialogueCanvas.SetActive(true);
        mainCanvas.SetActive(false);
        //prevent the player from doing anything while dialogue is active
        pause.ParalyzePlayer();
        //stop player movement immediately when dialogue starts, so they don't slide around while being paralyzed
        playerRb.linearVelocity = Vector2.zero;
        //reset player animation to idle
        playerAnimator.SetBool("isWalking", false);

        StopAllCoroutines();

        isOpen = true;
        isTyping = false;
        currentData = data;
        sentences = data.sentences;
        index = 0;

        if (sentences != null && sentences.Length > 0) //start dialogue normally
        {
            Debug.Log("Starting dialogue: " + currentData.characterName);
            StartCoroutine(Type());
        }
        else if (currentData != null && currentData.choices != null && currentData.choices.Length > 0) //start dialogue with choices, no sentences.
        {
            ShowChoices();
        } else
        {
            Debug.LogWarning("Dialogue has no sentences or choices.");
            CloseDialogue();
        }
    }
    void CloseDialogue()
    {
        StopAllCoroutines();
        mainCanvas.SetActive(true);
        dialogueCanvas.SetActive(false);

        index = 0;
        isOpen = false;
        pause.UnparalyzePlayer();
        Debug.Log("Closing dialogue: " + currentData.characterName);
        StartCoroutine(NoNewDialogueDelay());
    }
    IEnumerator NoNewDialogueDelay()
    {
        noNewDialogue = true;
        yield return _waitForSeconds0_2;
        noNewDialogue = false;
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

                // --- AUDIO LOGIC START ---
                if (audioSource != null)
                {
                    // PlayOneShot allows sounds to overlap if the typing speed is very fast
                    audioSource.PlayOneShot(audioSource.clip, volume);
                }
                // --- AUDIO LOGIC END ---

                yield return new WaitForSeconds(typingSpeed);
            }
        }
        isTyping = false;
    }
    void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            StartCoroutine(Type());
            Debug.Log("Moving to next sentence: " + currentData.characterName);
        }
        else
        {
            if (currentData != null && currentData.choices != null && currentData.choices.Length > 0)
            {
                ShowChoices();
            }
            else
            {
                CloseDialogue();
            }
        }
    }
    
     //used in PauseMenu.cs when pausing the game while in dialogue, so then player can't mess with the dialogue while paused.
    public void PauseDialogue()
    {
        pauseDialogue = true;
        dialogueCanvas.SetActive(false);
    }
    public void EndPauseDialogue()
    {
        pauseDialogue = false;
        dialogueCanvas.SetActive(true);
    }
    //----------------------------------------------------
    //This is where the code becomes extremely complicated
     //No way I'm touching this --Michix
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

        if (!choiceContainer.TryGetComponent<VerticalLayoutGroup>(out var vg)) vg = choiceContainer.AddComponent<VerticalLayoutGroup>();

        vg.padding = new RectOffset(0, 0, 0, 0);
        vg.spacing = 0;

        // ZMIANA: W��czamy wymuszanie wysoko�ci rz�d�w, aby kontener je �ciska�
        vg.childControlHeight = true;
        vg.childForceExpandHeight = true;
        vg.childControlWidth = true;
        vg.childForceExpandWidth = true;

        int totalChoices = currentData.choices.Length;
        int columns = Mathf.CeilToInt(Mathf.Sqrt(totalChoices));

        GameObject currentRow = null;
        List<GameObject> allButtons = new();

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
                PointerEventData data = new(EventSystem.current);
                ExecuteEvents.Execute(btnObj, data, ExecuteEvents.pointerEnterHandler);
                break;
            }
        }
    }


    GameObject CreateNewRow()
    {
        GameObject rowObj = new("Row", typeof(RectTransform));
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

        // ZMIANA: Zamiast sztywnego preferredHeight, u�ywamy flexibleHeight.
        // Dzi�ki temu przy ma�ej ilo�ci przycisk�w b�d� du�e, a przy du�ej - same si� �cisn�.
        LayoutElement le = rowObj.AddComponent<LayoutElement>();
        le.flexibleHeight = 1;

        return rowObj;
    }

    GameObject CreateButton(Choice choice, Transform parent)
    {
        GameObject btnObj = Instantiate(buttonPrefab, parent);
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

        UnityEngine.UI.Button b = btnObj.GetComponent<UnityEngine.UI.Button>();
        b.onClick.AddListener(() =>
        {
            choice.onChoiceSelected?.Invoke();
            SelectChoice(choice.nextDialogue);
        });

        b.transition = Selectable.Transition.None;

        Navigation nav = new()
        {
            mode = Navigation.Mode.None
        };
        b.navigation = nav;

        Outline outline = btnObj.GetComponent<Outline>();
        Color basicColor = (outline != null) ? outline.effectColor : Color.black;

        EventTrigger trigger = btnObj.GetComponent<EventTrigger>();
        if (trigger == null) trigger = btnObj.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new()
        {
            eventID = EventTriggerType.PointerEnter
        };
        entryEnter.callback.AddListener((data) => { SetOutlineColor(btnObj, outlineHighlightColor); });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new()
        {
            eventID = EventTriggerType.PointerExit
        };
        entryExit.callback.AddListener((data) => { SetOutlineColor(btnObj, basicColor); });
        trigger.triggers.Add(entryExit);

        return btnObj;
    }

    void SetOutlineColor(GameObject obj, Color color)
    {
        if (obj == null) return;
        if (obj.TryGetComponent<Outline>(out var outline)) outline.effectColor = color;
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

}