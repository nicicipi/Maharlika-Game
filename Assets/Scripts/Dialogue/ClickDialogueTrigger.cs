using UnityEngine;

public class ClickDialogueTrigger : MonoBehaviour
{
    [SerializeField]
    private Dialogue dialogue;

    [SerializeField]
    private bool oneShot;

    [SerializeField]
    private Animator talkIndicatorAnimator;

    [SerializeField]
    private string playerInRangeParameter = "PlayerInRange";

    private bool dialogueStarted;
    private bool playerInTrigger;

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

        if (dialogue == null)
        {
            Debug.LogError(
                "ClickDialogueTrigger: Dialogue is not assigned.",
                this
            );

            return;
        }

        dialogueStarted = true;

        DialogueManager.Instance.StartDialogue(dialogue);
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