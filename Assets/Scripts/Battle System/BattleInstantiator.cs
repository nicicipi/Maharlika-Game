using System.Collections;
using UnityEngine;

public class BattleInstantiator : MonoBehaviour
{
    [SerializeField] BattleTypeManager[] availableBattles;
    [SerializeField] bool activateOnEnter;
    private bool inArea;

    [SerializeField] float timeBetweenBattles;
    private float battleCounter;

    [SerializeField] bool deactiveAfterStarting;
    [SerializeField] bool canRunAway;

    [SerializeField] bool shouldCompleteQuest;
    public string questToComplete;

    private bool battleStarting;

    private void Start()
    {
        battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f);
    }

    private void Update()
    {
        if (battleStarting)
            return;

        if (inArea && !Player.instance.deactivateMovement)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 ||
                Input.GetAxisRaw("Vertical") != 0)
            {
                battleCounter -= Time.deltaTime;
            }
        }

        if (battleCounter <= 0f)
        {
            battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f);
            StartCoroutine(StartBattleCoroutine());
        }
    }

    private IEnumerator StartBattleCoroutine()
    {
        battleStarting = true;

        if (Player.instance != null)
        {
            Player.instance.StopMovement();
        }

        if (SceneTransition.instance != null)
        {
            yield return StartCoroutine(SceneTransition.instance.FadeToBlack());
        }

        int selectBattle = Random.Range(0, availableBattles.Length);

        BattleManager.instance.itemsReward = availableBattles[selectBattle].rewardItems;
        BattleManager.instance.xpRewardAmount = availableBattles[selectBattle].rewardXP;

        GameManager.instance.battleIsActive = true;

        // This activates the battle scene.
        BattleManager.instance.StartBattle(
            availableBattles[selectBattle].enemies,
            canRunAway
        );

        // Allow battleScene objects to initialize.
        yield return null;

        if (BattleRewardsHandler.instance != null)
        {
            BattleRewardsHandler.instance.markQuestComplete = shouldCompleteQuest;
            BattleRewardsHandler.instance.questToComplete = questToComplete;
        }
        else
        {
            Debug.LogWarning("BattleRewardsHandler instance was not found.");
        }

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (SceneTransition.instance != null)
        {
            yield return StartCoroutine(SceneTransition.instance.FadeFromBlack());
        }

        if (deactiveAfterStarting)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || battleStarting)
            return;

        if (activateOnEnter)
        {
            StartCoroutine(StartBattleCoroutine());
        }
        else
        {
            inArea = true;
        }
    }
}
