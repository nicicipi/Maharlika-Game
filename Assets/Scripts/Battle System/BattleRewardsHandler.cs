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

    [Header("Reward Audio")]
    [SerializeField] int rewardSFXIndex;

    public bool markQuestComplete;
    public string questToComplete;

    private void Awake()
    {
        instance = this;
    }

    public void OpenRewardScreen(int xpEarned, ItemsManager[] itemsEarned)
    {
        StartCoroutine(OpenRewardScreenCoroutine(xpEarned, itemsEarned));
    }

    private IEnumerator OpenRewardScreenCoroutine(int xpEarned, ItemsManager[] itemsEarned)
    {
        // Hide it immediately so it cannot be seen during the fade.
        rewardPanel.SetActive(false);

        // Fade completely to black.
        if (SceneTransition.instance != null)
        {
            yield return StartCoroutine(SceneTransition.instance.FadeToBlack());
        }

        xpReward = xpEarned;
        rewardItems = itemsEarned;

        xpText.text = xpEarned + " xp";
        lootText.text = "";

        foreach (ItemsManager rewardItem in rewardItems)
        {
            lootText.text += rewardItem.itemName + " ";
        }

        // Show the panel while the screen is fully black.
        rewardPanel.SetActive(true);

        // Allow the panel to initialize.
        yield return null;
        yield return new WaitForEndOfFrame();

        // Reveal the already-open reward panel.
        if (SceneTransition.instance != null)
        {
            yield return StartCoroutine(SceneTransition.instance.FadeFromBlack());
        }

        // Play the reward sound when the panel becomes visible.
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(rewardSFXIndex);
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

        if (markQuestComplete)
        {
            QuestManager.instance.MarkQuestComplete(questToComplete);
        }

        // Restore player input and Rigidbody physics after the battle.
        if (Player.instance != null)
        {
            Player.instance.ResumeMovement();
        }
    }

    public void PrepareRewardScreen(int xpEarned, ItemsManager[] itemsEarned)
    {
        xpReward = xpEarned;
        rewardItems = itemsEarned;

        xpText.text = xpEarned + " xp";
        lootText.text = "";

        foreach (ItemsManager rewardItem in rewardItems)
        {
            lootText.text += rewardItem.itemName + " ";
        }

        rewardPanel.SetActive(true);
    }

    public void PlayRewardSFX()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(rewardSFXIndex);
        }
    }
}

