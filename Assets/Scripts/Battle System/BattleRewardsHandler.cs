using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleRewardsHandler : MonoBehaviour
{

    public static BattleRewardsHandler instance;

    [SerializeField] TextMeshProUGUI xpText, lootText;
    [SerializeField] GameObject rewardPanel;

    [SerializeField] ItemsManager[] rewardItems;
    [SerializeField] int xpReward;

    public bool markQuestComplete;
    public string questToComplete;

    private void Start()
    {
        instance = this;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            OpenRewardScreen(676767, rewardItems);
        }
    }

    public void OpenRewardScreen(int xpEarned, ItemsManager[] itemsEarned)
    {
        StartCoroutine(OpenRewardScreenCoroutine(xpEarned, itemsEarned));
    }

    private IEnumerator OpenRewardScreenCoroutine(int xpEarned, ItemsManager[] itemsEarned)
    {
        if (SceneTransition.instance != null)
        {
            // Fade completely to black
            yield return StartCoroutine(SceneTransition.instance.FadeToBlack());
        }

        xpReward = xpEarned;
        rewardItems = itemsEarned;

        xpText.text = xpEarned + " xp";

        lootText.text = "";

        foreach (ItemsManager rewardItemText in rewardItems)
        {
            lootText.text += rewardItemText.itemName + " ";
        }

        // Open the panel while the screen is still black
        rewardPanel.SetActive(true);

        // Allow the panel to initialize
        yield return null;
        yield return new WaitForEndOfFrame();

        if (SceneTransition.instance != null)
        {
            // Reveal the reward panel
            yield return StartCoroutine(SceneTransition.instance.FadeFromBlack());
        }
    }

    public void CloseRewardScreen()
    {
        foreach (PlayerStats activePlayer in GameManager.instance.GetPlayerStats())
        {
            if (activePlayer.gameObject.activeInHierarchy)
            {
                activePlayer.AddXP(xpReward);
            }
        }

        foreach (ItemsManager itemRewarded in rewardItems)
        {
            Inventory.instance.AddItems(itemRewarded);
        }

        rewardPanel.SetActive(false);
        GameManager.instance.battleIsActive = false;

        //rewardPanel.SetActive(false);
        if (markQuestComplete)
        {
            QuestManager.instance.MarkQuestComplete(questToComplete);
        }
    }
}

