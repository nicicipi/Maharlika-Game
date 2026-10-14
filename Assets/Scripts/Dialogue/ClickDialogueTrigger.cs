using UnityEngine;
using UnityEngine;

public class ClickDialogueTrigger : MonoBehaviour
{
    [SerializeField]
    private Dialogue dialogue;

    [Header("Quest Completion")]
    [SerializeField]
    private string questToCompleteOnDialogueEnd;

    [SerializeField]
    private NpcWalkAway npcWalkAway;

    [SerializeField]
    private Dialogue[] followUpDialogues;

    [Header("Quest-Based Dialogue")]
    [SerializeField]
    private string followUpQuest;

    [SerializeField]
    private bool oneShot;

    [SerializeField]
    private Animator talkIndicatorAnimator;

    [SerializeField]
    private string playerInRangeParameter = "PlayerInRange";

    private bool dialogueStarted;
    private bool playerInTrigger;
    private int conversationCount;

    private void Awake()
    {
        SetTalkIndicatorVisible(false);
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (playerInTrigger)
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        if (oneShot && dialogueStarted)
        {
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogError(
                "ClickDialogueTrigger: Updated DialogueManager was not found.",
                this
            );

            return;
        }

        if (DialogueManager.Instance.isDialogueActive)
        {
            return;
        }

        Dialogue dialogueToStart = GetDialogueForConversation();

        if (npcWalkAway != null)
        {
            npcWalkAway.StopWalking();
        }

        if (dialogueToStart == null)
        {
            Debug.LogError(
                "ClickDialogueTrigger: Dialogue is not assigned.",
                this
            );

            return;
        }

        dialogueStarted = true;
        conversationCount++;

        DialogueManager.Instance.StartDialogue(dialogueToStart);
    }

    public void CompleteQuestOnDialogueEnd()
    {
        if (string.IsNullOrWhiteSpace(questToCompleteOnDialogueEnd))
        {
            return;
        }

        if (QuestManager.instance == null)
        {
            Debug.LogError(
                "ClickDialogueTrigger: QuestManager instance is missing.",
                this
            );

            return;
        }

        QuestManager.instance.MarkQuestComplete(questToCompleteOnDialogueEnd);

        Debug.Log(
            "ClickDialogueTrigger: Quest completed: " +
            questToCompleteOnDialogueEnd,
            this
        );
    }

    private Dialogue GetDialogueForConversation()
    {
        bool followUpQuestCompleted =
            !string.IsNullOrWhiteSpace(followUpQuest) &&
            QuestManager.instance != null &&
            QuestManager.instance.CheckIfComplete(followUpQuest);

        if (!followUpQuestCompleted ||
            followUpDialogues == null ||
            followUpDialogues.Length == 0)
        {
            return dialogue;
        }

        int followUpIndex = Mathf.Min(
            conversationCount,
            followUpDialogues.Length - 1
        );

        return followUpDialogues[followUpIndex];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        playerInTrigger = true;
        SetTalkIndicatorVisible(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        playerInTrigger = false;
        SetTalkIndicatorVisible(false);
    }

    private void SetTalkIndicatorVisible(bool visible)
    {
        if (talkIndicatorAnimator == null)
        {
            return;
        }

        talkIndicatorAnimator.SetBool(playerInRangeParameter, visible);
    }
}

public enum DialogueTextEffect
{
    None,
    Shake,
    Wobble
}