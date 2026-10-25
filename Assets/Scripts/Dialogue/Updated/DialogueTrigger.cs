using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;

    [TextArea(3, 10)]
    public string line;

    public UnityEvent onLineStarted;

    [Header("Text Effect")]
    public DialogueTextEffect textEffect;

    [Min(0f)]
    public float effectStrength = 1f;

    [Min(0f)]
    public float effectSpeed = 5f;

    [Header("Typing Speed")]
    public bool useCustomTypingSpeed;

    [Min(0f)]
    public float typingSpeed = 0.03f;

    [Header("Item Reward")]
    public string itemRewardName;

    [Min(1)]
    public int itemRewardAmount = 1;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();

    public UnityEvent onDialogueStarted;
    public UnityEvent onDialogueEnded;
}

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] bool triggerOnEnter = true;

    [Header("Dialogue Variants")]
    public Dialogue dialogue;

    [SerializeField]
    private Dialogue followUpDialogue;

    [SerializeField]
    private string followUpQuestToCheck;

    public void TriggerDialogue()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueTrigger: DialogueManager is missing.", this);
            return;
        }

        if (dialogue == null)
        {
            Debug.LogError("DialogueTrigger: Dialogue is not assigned.", this);
            return;
        }

        if (GameManager.Singleton.CurrentGameState == GameManager.GameState.Dialogue)
            return;

        Dialogue dialogueToStart = dialogue;

        if (followUpDialogue != null &&
            !string.IsNullOrWhiteSpace(followUpQuestToCheck) &&
            QuestManager.instance != null &&
            QuestManager.instance.CheckIfComplete(followUpQuestToCheck))
        {
            dialogueToStart = followUpDialogue;
        }

        DialogueManager.Instance.StartDialogue(dialogueToStart);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggerOnEnter)
            return;

        if (collision.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }
}