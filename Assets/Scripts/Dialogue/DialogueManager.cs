using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI[] choiceOptions;
    [SerializeField] private TextMeshProUGUI npcName;
    [SerializeField] private Image avatarImage;

    private bool isTyping = false;
    private bool makeChoice = false;

    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float inputCooldown = 0.1f;
    private float lastInputTime;
    private int currentLineIndex = 0;
    private int selectedChoice = 0;
    private int lastSelectedChoice = -1;
    private int currentBranchIndex;

    private GameInput gameInput;

    private readonly StringBuilder textBuilder = new StringBuilder(256);

    private string[] lines;
    private string[] choices;
    private Coroutine typingCoroutine;

    private InputAction submitAction;
    private InputAction navigateAction;

    private DialogueData currentDialogueData;
    private DialogueData.DialogueBranch currentBranch;
    private Dictionary<string, bool> dialogueOutcomes = new Dictionary<string, bool>();

    public event Action OnDialogueEnd;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (choicePanel) choicePanel.SetActive(false);
    }

    private void Start()
    {
        gameInput = GameInput.Instance;

        if (gameInput == null)
        {
            return;
        }

        submitAction = gameInput.inputActions.UI.Submit;
        navigateAction = gameInput.inputActions.UI.Navigate;
    }

    public void StartDialogue(DialogueData dialogueData)
    {
        if (dialogueData == null || dialogueData.dialogueBranches == null || dialogueData.dialogueBranches.Length == 0)
        {
            return;
        }

        if (dialoguePanel.activeSelf)
        {
            EndDialogue();
        }

        currentDialogueData = dialogueData;
        currentBranchIndex = dialogueData.startingBranchIndex;
        InitializeBranch(currentBranchIndex);
    }

    private void InitializeBranch(int branchIndex)
    {
        currentBranch = currentDialogueData.dialogueBranches[branchIndex];

        if (currentBranch.npcImage != null) avatarImage.sprite = currentBranch.npcImage;
        npcName.text = currentBranch.npcName;

        lines = currentBranch.dialogueLines;
        currentLineIndex = 0;
        selectedChoice = 0;
        lastSelectedChoice = -1;
        makeChoice = false;
        isTyping = false;

        dialoguePanel.SetActive(true);
        TogglePlayerMovement(false);

        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (isTyping)
        {
            FinishTypingImmediately();
            return;
        }

        if (currentLineIndex < lines.Length)
        {
            StartTypingLine();
            return;
        }

        if (currentBranch.choiceCheck)
        {
            ShowChoice();
            return;
        }
        OnDialogueEnd?.Invoke();
        EndDialogue();
    }

    private void FinishTypingImmediately()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = lines[currentLineIndex];
        currentLineIndex++;
        isTyping = false;
    }

    private void StartTypingLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(lines[currentLineIndex]));
    }

    private IEnumerator TypeLine(string dialogue)
    {
        isTyping = true;
        dialogueText.text = "";
        textBuilder.Clear();

        foreach (char letter in dialogue)
        {
            textBuilder.Append(letter);
            dialogueText.text = textBuilder.ToString();
            yield return new WaitForSeconds(textSpeed);
        }

        currentLineIndex++;
        isTyping = false;
        typingCoroutine = null;
    }

    private void ShowChoice()
    {
        if (currentBranch.choices == null || currentBranch.choices.Length == 0)
        {
            EndDialogue();
            return;
        }

        choicePanel.SetActive(true);
        choices = currentBranch.choices.Select(c => c.choiceText).ToArray();
        UpdateChoiceOptions();
        makeChoice = true;
        selectedChoice = 0;
        UpdateChoiceColors();
    }

    private void UpdateChoiceOptions()
    {
        int optionsCount = Mathf.Min(choiceOptions.Length, choices.Length);

        for (int i = 0; i < choiceOptions.Length; i++)
        {
            bool shouldEnable = i < optionsCount;
            choiceOptions[i].enabled = shouldEnable;

            if (shouldEnable)
            {
                choiceOptions[i].text = choices[i];
            }
        }
    }

    private void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (makeChoice)
        {
            HandleChoiceSelection();
        }
        else
        {
            HandleDialogueInput();
        }
    }

    private void HandleChoiceSelection()
    {
        if (navigateAction.triggered)
        {
            Vector2 navigation = navigateAction.ReadValue<Vector2>();

            if (navigation.y > 0.5f) 
            {
                selectedChoice = (selectedChoice > 0) ? selectedChoice - 1 : 0;
                UpdateChoiceColorsIfNeeded();
            }
            else if (navigation.y < -0.5f)
            {
                int maxChoice = Mathf.Min(choices.Length - 1, choiceOptions.Length - 1);
                selectedChoice = (selectedChoice < maxChoice) ? selectedChoice + 1 : maxChoice;
                UpdateChoiceColorsIfNeeded();
            }
        }

        if (submitAction.triggered || Input.GetKeyDown(KeyCode.Return))
        {
            DialogueData.DialogueChoice selectedDialogueChoice = currentBranch.choices[selectedChoice];

            if (selectedDialogueChoice.hasOutcome && !string.IsNullOrEmpty(selectedDialogueChoice.outcomeKey))
            {
                dialogueOutcomes[selectedDialogueChoice.outcomeKey] = true;
            }

            makeChoice = false;
            choicePanel.SetActive(false);

            if (selectedDialogueChoice.nextBranchIndex >= 0 &&
                selectedDialogueChoice.nextBranchIndex < currentDialogueData.dialogueBranches.Length)
            {
                currentDialogueData.startingBranchIndex = selectedDialogueChoice.nextBranchIndex;
                InitializeBranch(selectedDialogueChoice.nextBranchIndex);
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private void HandleDialogueInput()
    {
        bool inputTriggered = (submitAction.triggered || Input.GetMouseButtonDown(0)) &&
                             (Time.time - lastInputTime > inputCooldown);

        if (inputTriggered)
        {
            lastInputTime = Time.time;
            ShowNextLine();
        }
    }

    private void UpdateChoiceColorsIfNeeded()
    {
        if (lastSelectedChoice == selectedChoice) return;

        lastSelectedChoice = selectedChoice;
        UpdateChoiceColors();
    }

    private void UpdateChoiceColors()
    {
        for (int i = 0; i < choiceOptions.Length; i++)
        {
            if (i < choices.Length && choiceOptions[i] != null)
            {
                choiceOptions[i].color = (i == selectedChoice) ? Color.red : Color.black;
            }
        }
    }

    public void EndDialogue()
    {
        ResetDialogueState();
        TogglePlayerMovement(true);
    }

    private void ResetDialogueState()
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        lines = null;
        currentLineIndex = 0;
        isTyping = false;
        makeChoice = false;
        currentBranch = null;
        currentDialogueData = null;

        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (choicePanel) choicePanel.SetActive(false);
    }

    private void TogglePlayerMovement(bool enable)
    {
        if (gameInput == null) return;

        if (enable)
        {
            gameInput.inputActions.PlayerInput.Enable();
            gameInput.inputActions.UI.Disable();
        }
        else
        {
            gameInput.inputActions.PlayerInput.Disable();
            gameInput.inputActions.UI.Enable();
        }
    }

    public bool CheckDialogueOutcome(string outcomeKey)
    {
        return dialogueOutcomes.ContainsKey(outcomeKey) && dialogueOutcomes[outcomeKey];
    }

    private void OnDisable()
    {
        ResetDialogueState();
    }
}