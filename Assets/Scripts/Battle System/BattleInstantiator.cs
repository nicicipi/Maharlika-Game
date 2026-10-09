using System.Collections;
using UnityEngine;

public class BattleInstantiator : MonoBehaviour
{

    [Header("Tutorial Enemy Persistence")]
    [SerializeField]
    private string enemyId = "RobotTutorialEnemy";

    [SerializeField]
    private ItemsManager tutorialRewardItem;

    private string EnemyDefeatedKey =>
        "EnemyDefeated_" + enemyId;

    private string TutorialRewardKey =>
        "TutorialRewardGiven_" + enemyId;

    [Header("Battle Tutorial")]
    [SerializeField]
    private BattleTutorialManager battleTutorialManager;

    [SerializeField]
    private bool startTutorialOnFirstBattle = true;

    private bool tutorialHasStarted;

    [SerializeField]
    private BattleTypeManager[] availableBattles;

    [SerializeField]
    private bool canRunAway;

    [SerializeField]
    private float runAwayDistance = 2f;

    [SerializeField]
    private float battleTriggerCooldown = 2f;

    private Collider2D battleTrigger;

    [SerializeField]
    private bool shouldCompleteQuest;

    [SerializeField]
    private bool deactivateAfterStarting;

    [SerializeField]
    private int battleMusicIndex;

    public string questToComplete;

    private bool battleStarting;

    private bool tutorialRewardGivenThisSession;

    private void Start()
    {
        if (PlayerPrefs.GetInt(EnemyDefeatedKey, 0) == 1)
        {
            Destroy(transform.root.gameObject);
        }
    }

    private void Awake()
    {
        battleTrigger = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (battleStarting ||
            !collision.CompareTag("Player") ||
            GameManager.instance.battleIsActive)
        {
            return;
        }

        StartCoroutine(StartBattleCoroutine());
    }

    private IEnumerator StartBattleCoroutine()
    {
        battleStarting = true;

        if (Player.instance != null)
        {
            Player.instance.StopMovement();
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.StartBattleMusic(battleMusicIndex);
        }

        if (SceneTransition.instance != null)
        {
            yield return StartCoroutine(
                SceneTransition.instance.FadeToBlack()
            );
        }

        int selectedBattle =
            Random.Range(0, availableBattles.Length);

        BattleManager.instance.itemsReward =
            availableBattles[selectedBattle].rewardItems;

        BattleManager.instance.xpRewardAmount =
            availableBattles[selectedBattle].rewardXP;

        BattleManager.instance.scrapsReward =
            availableBattles[selectedBattle].scrapsReward;

        GameManager.instance.battleIsActive = true;

        GiveTutorialReward();

        BattleManager.instance.SetBattleTrigger(this);

        BattleManager.instance.StartBattle(
            availableBattles[selectedBattle].enemies,
            canRunAway
        );

        if (startTutorialOnFirstBattle &&
            !tutorialHasStarted &&
            battleTutorialManager != null)
        {
            tutorialHasStarted = true;
            battleTutorialManager.StartTutorial();
        }
        

        yield return null;

        if (BattleRewardsHandler.instance != null)
        {
            BattleRewardsHandler.instance.markQuestComplete =
                shouldCompleteQuest;

            BattleRewardsHandler.instance.questToComplete =
                questToComplete;
        }

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (SceneTransition.instance != null)
        {
            yield return StartCoroutine(
                SceneTransition.instance.FadeFromBlack()
            );
        }

        if (deactivateAfterStarting)
        {
            Destroy(transform.root.gameObject);
        }
    }


    public void MoveEnemyAwayFromPlayer()
    {
        if (Player.instance == null)
        {
            return;
        }

        Transform enemy = transform.root;

        Vector2 directionAwayFromPlayer =
            (enemy.position - Player.instance.transform.position).normalized;

        if (directionAwayFromPlayer == Vector2.zero)
        {
            directionAwayFromPlayer = Vector2.right;
        }

        Vector2 newPosition =
            (Vector2)Player.instance.transform.position +
            directionAwayFromPlayer * runAwayDistance;

        Rigidbody2D enemyRigidbody =
            enemy.GetComponent<Rigidbody2D>();

        if (enemyRigidbody != null)
        {
            enemyRigidbody.position = newPosition;
            enemyRigidbody.velocity = Vector2.zero;
        }
        else
        {
            enemy.position = newPosition;
        }

        // Allow this enemy to start another battle later.
        battleStarting = false;

        if (battleTrigger != null)
        {
            battleTrigger.enabled = false;

            Invoke(
                nameof(EnableBattleTrigger),
                battleTriggerCooldown
            );
        }
    }

    private void EnableBattleTrigger()
    {
        if (battleTrigger != null)
        {
            battleTrigger.enabled = true;
        }
    }

    public void GiveTutorialReward()
    {
        if (tutorialRewardGivenThisSession)
        {
            return;
        }

        if (tutorialRewardItem == null)
        {
            Debug.LogError(
                "BattleInstantiator: Tutorial Reward Item is not assigned.",
                this
            );

            return;
        }

        if (Inventory.instance == null)
        {
            Debug.LogError(
                "BattleInstantiator: Inventory instance was not found.",
                this
            );

            return;
        }

        if (tutorialRewardItem.isStackable &&
            tutorialRewardItem.amount <= 0)
        {
            tutorialRewardItem.amount = 1;
        }

        Inventory.instance.AddItems(tutorialRewardItem);
        tutorialRewardGivenThisSession = true;

        Debug.Log(
            "Tutorial reward added: " +
            tutorialRewardItem.itemName
        );
    }

    public void MarkEnemyDefeated()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterDefeatedEnemy(enemyId);
        }

        Destroy(transform.root.gameObject);
    }

    [ContextMenu("Reset Tutorial Progress")]
    private void ResetTutorialProgress()
    {
        PlayerPrefs.DeleteKey(TutorialRewardKey);
        PlayerPrefs.DeleteKey(EnemyDefeatedKey);
        PlayerPrefs.Save();

        Debug.Log(
            "Reset tutorial progress for enemy ID: " +
            enemyId
        );
    }

}