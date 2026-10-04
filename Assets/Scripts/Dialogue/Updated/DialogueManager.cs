using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;
    private DialogueLine currentLine;
    private Coroutine typingCoroutine;
    private bool isTyping;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.2f;
    public Animator animator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        lines = new Queue<DialogueLine>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null || dialogue.dialogueLines == null)
        {
            Debug.LogError("DialogueManager: Dialogue data is missing.");
            return;
        }

        isDialogueActive = true;

        if (GameManager.instance != null)
        {
            GameManager.instance.dialogueBoxOpened = true;
        }

        if (Player.instance != null)
        {
            Player.instance.StopMovement();
        }

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
            EndDialogue();
            return;
        }

        currentLine = lines.Dequeue();

        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        typingCoroutine = StartCoroutine(TypeSentence(currentLine));
    }

    private IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        isTyping = true;
        dialogueArea.text = "";

        foreach (char letter in dialogueLine.line)
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    private void EndDialogue()
    {
        isDialogueActive = false;

        if (GameManager.instance != null)
        {
            GameManager.instance.dialogueBoxOpened = false;
        }

        if (Player.instance != null)
        {
            Player.instance.ResumeMovement();
        }

        if (animator != null)
        {
            animator.Play("Hide");
        }
    }
}
