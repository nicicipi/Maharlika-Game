using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static GameManager;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;
    private DialogueLine currentLine;
    private Dialogue currentDialogue;
    private Coroutine typingCoroutine;
    private bool isTyping;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.2f;
    public Animator animator;

    [SerializeField]
    private DialogueTextAnimator textAnimator;

    private UnityAction _onDialogueEnd;
    private GameState _onEndGameState;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        lines = new Queue<DialogueLine>();

        if (textAnimator == null &&
            dialogueArea != null)
        {
            textAnimator =
                dialogueArea.GetComponent<DialogueTextAnimator>();

            if (textAnimator == null)
            {
                textAnimator =
                    dialogueArea.gameObject.AddComponent<DialogueTextAnimator>();
            }
        }
    }

    public void StartDialogue(Dialogue dialogue, UnityAction OnDialogueEnd = null, GameManager.GameState onEndState = GameManager.GameState.Roaming)
    {
        if (dialogue == null || dialogue.dialogueLines == null)
        {
            Debug.LogError("DialogueManager: Dialogue data is missing.");
            return;
        }
        _onDialogueEnd = OnDialogueEnd;
        _onEndGameState = onEndState;
        GameManager.instance.SetGameState(GameState.Dialogue);

        isDialogueActive = true;
        currentDialogue = dialogue;

        currentDialogue.onDialogueStarted?.Invoke();

        if (animator != null)
        {
            animator.gameObject.SetActive(true);
            animator.Play("Show");
        }

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            if (dialogueLine != null && dialogueLine.character != null)
            {
                lines.Enqueue(dialogueLine);
            }
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        // First click while text is typing: show the complete sentence.
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            dialogueArea.text = currentLine.line;
            isTyping = false;
            typingCoroutine = null;
            return;
        }

        // Second click: move to the next sentence.
        if (lines.Count == 0)
        {
            Debug.Log("EndingDialogue");
            EndDialogue();
            return;
        }

        currentLine = lines.Dequeue();

        GiveItemReward(currentLine);

        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        if (textAnimator != null)
        {
            textAnimator.SetEffect(
                currentLine.textEffect,
                currentLine.effectStrength,
                currentLine.effectSpeed
            );
        }

        currentLine.onLineStarted?.Invoke();

        typingCoroutine = StartCoroutine(TypeSentence(currentLine));
    }

    private void GiveItemReward(DialogueLine dialogueLine)
    {
        if (dialogueLine == null ||
            string.IsNullOrWhiteSpace(dialogueLine.itemRewardName))
        {
            return;
        }

        if (ItemsAssets.instance == null)
        {
            Debug.LogError(
                "DialogueManager: ItemsAssets instance is missing; cannot give item reward."
            );
            return;
        }

        if (Inventory.instance == null)
        {
            Debug.LogError(
                "DialogueManager: Inventory instance is missing; cannot give item reward."
            );
            return;
        }

        ItemsManager item =
            ItemsAssets.instance.GetItemAsset(dialogueLine.itemRewardName);

        if (item == null)
        {
            Debug.LogError(
                "DialogueManager: Item reward was not found: " +
                dialogueLine.itemRewardName
            );
            return;
        }

        for (int i = 0; i < dialogueLine.itemRewardAmount; i++)
        {
            Inventory.instance.AddItems(item);
        }

        Debug.Log(
            "DialogueManager: Added " +
            dialogueLine.itemRewardAmount +
            " x " +
            item.itemName +
            " to the inventory."
        );
    }

    private IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        isTyping = true;
        dialogueArea.text = string.Empty;

        float currentTypingSpeed = typingSpeed;

        if (dialogueLine.useCustomTypingSpeed)
        {
            currentTypingSpeed = dialogueLine.typingSpeed;
        }

        if (currentTypingSpeed <= 0f)
        {
            dialogueArea.text = dialogueLine.line;
            isTyping = false;
            typingCoroutine = null;
            yield break;
        }

        foreach (char letter in dialogueLine.line)
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(
                currentTypingSpeed
            );
        }

        isTyping = false;
        typingCoroutine = null;
    }

    private void EndDialogue()
    {
        isDialogueActive = false;

        if (currentDialogue != null)
        {
            currentDialogue.onDialogueEnded?.Invoke();

            if (!string.IsNullOrWhiteSpace(currentDialogue.questToCompleteOnEnd) &&
                QuestManager.instance != null)
            {
                QuestManager.instance.MarkQuestComplete(currentDialogue.questToCompleteOnEnd);
                Debug.Log("DialogueManager: Quest completed: " + currentDialogue.questToCompleteOnEnd);
            }

            currentDialogue = null;
        }

        if (animator != null)
        {
            animator.Play("Hide");
        }

        if (textAnimator != null)
        {
            textAnimator.ClearEffect();
        }

        _onDialogueEnd?.Invoke();
        GameManager.instance.SetGameState(_onEndGameState);
    }
}
