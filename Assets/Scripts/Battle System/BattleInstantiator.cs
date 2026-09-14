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
        MenuManager.instance.FadeImage();
        GameManager.instance.battleIsActive = true;

        int selectBattle = Random.Range(0, availableBattles.Length);

        BattleManager.instance.itemsReward = availableBattles[selectBattle].rewardItems;
        BattleManager.instance.xpRewardAmount = availableBattles[selectBattle].rewardXP;

        BattleRewardsHandler.instance.markQuestComplete = shouldCompleteQuest;

        yield return new WaitForSeconds(1.5f);

        MenuManager.instance.FadeOut();

        BattleManager.instance.StartBattle(availableBattles[selectBattle].enemies, canRunAway);

        if(deactiveAfterStarting)
        {
            Destroy(gameObject); //this destroys the battle instantiator after starting the battle so that it doesn't start another battle after this one
            // perfect for bosses but not for random encounters
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
