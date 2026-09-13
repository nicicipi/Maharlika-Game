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
        xpReward = xpEarned;
        rewardItems = itemsEarned;

        xpText.text = xpEarned + " xp";

        foreach (ItemsManager rewardItemText in rewardItems)
        {
           lootText.text = rewardItemText.itemName + " ";
        }

        rewardPanel.SetActive(true);
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
    }
}

