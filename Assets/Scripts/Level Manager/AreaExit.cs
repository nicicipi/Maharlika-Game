using UnityEngine;

using System.Collections;
using UnityEngine.SceneManagement;
using Utilities;

public class AreaExit : MonoBehaviour
{
    [SerializeField] SceneEnum sceneToLoad;
    [SerializeField] int positionIndex = 0;

    [Header("Main Quest Requirement")]
    [SerializeField] string mainQuestToUnlock;

    [Header("Additional Quest Requirement")]
    [SerializeField] string requiredQuestToUnlock;

    [Header("Enemy Blocking Exit")]
    [SerializeField] string[] blockingEnemyIds;

    [Header("Blocked Exit Dialogue")]
    [SerializeField] Dialogue blockedExitDialogue;

    [Header("Scene Transition")]
    [SerializeField] float fadeToBlackDelay = 0.35f;

    private bool isLoading;
    private bool hasTriggered;
    private bool blockedDialogueShown;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (DialogueManager.Instance.isDialogueActive)
            return;
        TryStartTransition(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (DialogueManager.Instance.isDialogueActive)
            return;
        TryStartTransition(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (DialogueManager.Instance.isDialogueActive)
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

        if (!CanEnterArea())
        {
            ShowBlockedExitDialogue();
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

    private bool CanEnterArea()
    {
        if (string.IsNullOrWhiteSpace(mainQuestToUnlock))
        {
            return true;
        }

        if (QuestManager.instance == null)
        {
            Debug.LogError("AreaExit: QuestManager Singleton is missing.");
            return false;
        }

        if (QuestManager.instance.ArePreviousMainQuestsComplete(mainQuestToUnlock))
        {
            return true;
        }

        Debug.Log("AreaExit: Complete the previous main quests first.", this);
        return false;
    }

    private bool CanLeaveArea()
    {
        if (HasBlockingEnemiesRemaining())
        {
            Debug.Log("AreaExit: Defeat all blocking enemies before leaving.", this);
            return false;
        }

        if (blockingEnemyIds != null && blockingEnemyIds.Length > 0)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(requiredQuestToUnlock))
        {
            return true;
        }

        if (QuestManager.instance == null)
        {
            Debug.LogError("AreaExit: QuestManager Singleton is missing.");
            return false;
        }

        if (QuestManager.instance.CheckIfComplete(requiredQuestToUnlock))
        {
            return true;
        }

        Debug.Log(
            "AreaExit: Complete the required quest before leaving.",
            this
        );

        return false;
    }

    private bool HasBlockingEnemiesRemaining()
    {
        if (blockingEnemyIds == null || blockingEnemyIds.Length == 0)
        {
            return false;
        }

        foreach (string enemyId in blockingEnemyIds)
        {
            if (string.IsNullOrWhiteSpace(enemyId))
            {
                continue;
            }

            if (PlayerPrefs.GetInt("EnemyDefeated_" + enemyId, 0) == 1)
            {
                continue;
            }

            BattleInstantiator[] enemies =
                FindObjectsOfType<BattleInstantiator>();

            foreach (BattleInstantiator enemy in enemies)
            {
                if (enemy != null && enemy.EnemyId == enemyId)
                {
                    return true;
                }
            }
        }

        return false;
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

        if (DialogueManager.Instance.isDialogueActive)
        {
            return;
        }

        blockedDialogueShown = true;
        DialogueManager.Instance.StartDialogue(blockedExitDialogue);
    }
}
