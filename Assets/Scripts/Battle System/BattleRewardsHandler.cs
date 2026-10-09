using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleRewardsHandler : MonoBehaviour
{
    public static BattleRewardsHandler instance;

    [Header("Reward UI")]
    [SerializeField]
    private GameObject rewardPanel;

    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [Header("Typing")]
    [SerializeField]
    private float typingSpeed = 0.03f;

    [Header("Reward Audio")]
    [SerializeField]
    private int rewardSFXIndex;

    public bool markQuestComplete;
    public string questToComplete;

    private readonly List<string> messages = new List<string>();

    private int currentMessageIndex;
    private Coroutine typingCoroutine;
    private bool isTyping;

    private int xpReward;
    private ItemsManager[] rewardItems;
    private int scrapsReward;

    private void Awake()
    {
        instance = this;
    }

    public void PrepareRewardScreen(
        int xpEarned,
        ItemsManager[] itemsEarned,
        int scrapsEarned
    )
    {
        xpReward = xpEarned;
        rewardItems = itemsEarned;
        scrapsReward = scrapsEarned;

        ApplyRewards();
        BuildRewardMessages();

        currentMessageIndex = 0;

        rewardPanel.SetActive(true);
        DisplayCurrentMessage();

        PlayRewardSFX();
    }

    public void DisplayNextMessage()
    {
        if (!rewardPanel.activeInHierarchy)
        {
            return;
        }

        if (isTyping)
        {
            FinishTyping();
            return;
        }

        currentMessageIndex++;

        if (currentMessageIndex >= messages.Count)
        {
            CloseRewardScreen();
            return;
        }

        DisplayCurrentMessage();
    }

    private void DisplayCurrentMessage()
    {
        if (currentMessageIndex < 0 ||
            currentMessageIndex >= messages.Count)
        {
            return;
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(
            TypeMessage(messages[currentMessageIndex])
        );
    }

    private IEnumerator TypeMessage(string message)
    {
        isTyping = true;
        dialogueText.text = string.Empty;

        foreach (char character in message)
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    private void FinishTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = messages[currentMessageIndex];
        isTyping = false;
    }

    private void ApplyRewards()
    {
        PlayerStats[] playerStats =
            GameManager.instance.GetPlayerStats();

        foreach (PlayerStats player in playerStats)
        {
            if (player.gameObject.activeInHierarchy)
            {
                player.AddXP(xpReward);
            }
        }

        if (rewardItems != null && Inventory.instance != null)
        {
            foreach (ItemsManager item in rewardItems)
            {
                if (item != null)
                {
                    Inventory.instance.AddItems(item);
                }
            }
        }

        GameManager.instance.currentMoney += scrapsReward;
    }

    private void BuildRewardMessages()
    {
        messages.Clear();

        PlayerStats[] playerStats =
            GameManager.instance.GetPlayerStats();

        foreach (PlayerStats player in playerStats)
        {
            if (!player.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (player.playerLevel > 1)
            {
                AddMessage(
                    player.playerName +
                    " has gained " +
                    xpReward +
                    " XP and reached Level " +
                    player.playerLevel +
                    "!"
                );
            }
            else
            {
                AddMessage(
                    player.playerName +
                    " has gained " +
                    xpReward +
                    " XP."
                );
            }
        }

        AddMessage(BuildLootMessage());

        AddMessage(
            "You picked up " +
            scrapsReward +
            " scraps."
        );
    }

    private string BuildLootMessage()
    {
        if (rewardItems == null || rewardItems.Length == 0)
        {
            return "You did not receive any items.";
        }

        Dictionary<string, int> itemCounts =
            new Dictionary<string, int>();

        foreach (ItemsManager item in rewardItems)
        {
            if (item == null)
            {
                continue;
            }

            if (!itemCounts.ContainsKey(item.itemName))
            {
                itemCounts[item.itemName] = 0;
            }

            itemCounts[item.itemName]++;
        }

        List<string> itemMessages = new List<string>();

        foreach (KeyValuePair<string, int> item in itemCounts)
        {
            itemMessages.Add(
                "You got " +
                item.Value +
                " " +
                item.Key +
                "."
            );
        }

        return string.Join("\n", itemMessages);
    }

    private void AddMessage(string message)
    {
        messages.Add(message);
    }

    public void CloseRewardScreen()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        rewardPanel.SetActive(false);

        GameManager.instance.battleIsActive = false;

        if (markQuestComplete)
        {
            QuestManager.instance.MarkQuestComplete(
                questToComplete
            );
        }

        if (Player.instance != null)
        {
            Player.instance.ResumeMovement();
        }
    }

    public void PlayRewardSFX()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(rewardSFXIndex);
        }
    }
}

