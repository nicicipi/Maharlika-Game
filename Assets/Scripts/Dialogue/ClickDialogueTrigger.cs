using UnityEngine;

public class ClickDialogueTrigger : MonoBehaviour
{
    [SerializeField]
    private Dialogue dialogue;

    [SerializeField]
    private NpcWalkAway npcWalkAway;

    [SerializeField]
    private Dialogue[] followUpDialogues;

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
        if (oneShot && dialogueStarted )
        {
            return;
        }
        if (GameManager.Singleton.CurrentGameState == GameManager.GameState.Dialogue)
            return;

        if (DialogueManager.Instance == null)
        {
            Debug.LogError(
                "ClickDialogueTrigger: Updated DialogueManager was not found.",
                this
            );

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

    private Dialogue GetDialogueForConversation()
    {
        if (conversationCount == 0)
            return dialogue;
        int followUpIndex = Mathf.Min(conversationCount,followUpDialogues.Length - 1);
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