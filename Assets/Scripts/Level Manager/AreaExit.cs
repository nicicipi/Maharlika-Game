using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Utilities;

public class AreaExit : MonoBehaviour
{
    [SerializeField] SceneEnum sceneToLoad;
    [SerializeField] int positionIndex = 0;

    [Header("Enemy Blocking Exit")]
    [SerializeField] GameObject[] blockingEnemy;
     
    [Header("Blocked Exit Dialogue")]
    [SerializeField] Dialogue blockedExitDialogue;

    [Header("Scene Transition")]
    [SerializeField] float fadeToBlackDelay = 0.35f;

    private bool isLoading;
    private bool hasTriggered;
    private bool blockedDialogueShown;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Singleton.CurrentGameState != GameManager.GameState.Roaming)
            return;
        TryStartTransition(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (GameManager.Singleton.CurrentGameState != GameManager.GameState.Roaming)
            return;
        TryStartTransition(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GameManager.Singleton.CurrentGameState != GameManager.GameState.Roaming)
            return;
        if (IsPlayerCollider(collision))
        {
            blockedDialogueShown = false;
        }
    }

    private void TryStartTransition(Collider2D collision)
    {
        if (isLoading || hasTriggered || !IsPlayerCollider(collision))
            return;

        if (SceneTransition.instance == null)
        {
            Debug.LogError("AreaExit: SceneTransition Singleton is missing.");
            return;
        }

        if (!CanLeaveArea())
        {
            ShowBlockedExitDialogue();
            return;
        }

        isLoading = true;
        hasTriggered = true;
        SceneTransition.instance.TransitionToScene(sceneToLoad.GetReference(), positionIndex);
    }

    private bool IsPlayerCollider(Collider2D collision)
    {
        return collision.CompareTag("Player") || collision.transform.root.CompareTag("Player");
    }

    private bool CanLeaveArea()
    {
        foreach (var item in blockingEnemy)
            if (item.activeSelf)
                return false;
        return true;
    }
    private void ShowBlockedExitDialogue()
    {
        if (blockedDialogueShown || blockedExitDialogue == null)
        {
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("AreaExit: DialogueManager Singleton is missing.");
            return;
        }

        if (GameManager.Singleton.CurrentGameState != GameManager.GameState.Roaming)
        {
            return;
        }

        blockedDialogueShown = true;
        DialogueManager.Instance.StartDialogue(blockedExitDialogue);
    }
    private BoxCollider2D _collider;
    public void OnDrawGizmos()
    {

        if (_collider == null)
        {
            _collider = GetComponent<BoxCollider2D>();
        }
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(_collider.offset, _collider.size);
    }
}
