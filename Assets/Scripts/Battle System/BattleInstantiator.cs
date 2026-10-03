using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f);
    }

    private void Update()
    {
        if (inArea && !Player.instance.deactivateMovement)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                battleCounter -= Time.deltaTime;
            }
        }

        if (battleCounter <= 0)
        {
            battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f);
            StartCoroutine(StartBattleCoroutine());
        }
    }

    private IEnumerator StartBattleCoroutine()
    {
        // Fade completely to black before showing the battle scene
        if (SceneTransition.instance != null)
        {
            yield return StartCoroutine(SceneTransition.instance.FadeToBlack());
        }

        GameManager.instance.battleIsActive = true;

        int selectBattle = Random.Range(0, availableBattles.Length);

        BattleManager.instance.itemsReward = availableBattles[selectBattle].rewardItems;
        BattleManager.instance.xpRewardAmount = availableBattles[selectBattle].rewardXP;

        BattleRewardsHandler.instance.markQuestComplete = shouldCompleteQuest;

        // Start the battle while the screen is fully black
        BattleManager.instance.StartBattle(
            availableBattles[selectBattle].enemies,
            canRunAway
        );

        // Allow the battle UI and objects to initialize
        yield return null;
        yield return new WaitForEndOfFrame();

        // Reveal the battle after it is ready
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
        if (collision.CompareTag("Player"))
        {
            if (activateOnEnter)
            {
                //print("Battle Started");
                StartCoroutine(StartBattleCoroutine()); // Start the battle immediately when the player enters the area
                // add another function where it takes some time to start the battle and then starts it after that time
            }
            else
            {
                inArea = true;
            }
        }
    }

}
