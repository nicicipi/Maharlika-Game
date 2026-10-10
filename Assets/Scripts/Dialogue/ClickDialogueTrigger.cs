using UnityEngine;

public class ClickDialogueTrigger : MonoBehaviour
{
    [SerializeField]
    private Dialogue dialogue;

    [SerializeField]
    private bool oneShot;

    private Collider2D clickableCollider;
    private bool dialogueStarted;

    private void Awake()
    {
        clickableCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogError(
                "ClickDialogueTrigger: Camera.main was not found.",
                this
            );

            return;
        }

        Vector2 mousePosition =
            Camera.main.ScreenToWorldPoint(
                Input.mousePosition
            );

        Collider2D[] clickedColliders =
            Physics2D.OverlapPointAll(mousePosition);

        foreach (Collider2D clickedCollider in clickedColliders)
        {
            if (clickedCollider != clickableCollider)
            {
                continue;
            }

            StartDialogue();
            return;
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
}

public enum DialogueTextEffect
{
    None,
    Shake,
    Wobble
}