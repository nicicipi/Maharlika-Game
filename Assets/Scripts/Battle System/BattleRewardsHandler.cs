using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SaveSystem;
using BattleUtils;

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
    private List<BattleRewardData> rewards;
    private Dictionary<string, int> _newItemMaps = new Dictionary<string, int>();

    private void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            DisplayNextMessage();
        }
    }

    public void PrepareRewardScreen(List<BattleRewardData> rewards)
    {
        this.rewards = rewards;

        ApplyRewards();

        currentMessageIndex = 0;

    }
    public void DisplayMessags()
    {
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

        typingCoroutine = StartCoroutine(TypeMessage(messages[currentMessageIndex]));
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
        int totalXP = 0;
        int totalScraps = 0;
        List<ItemData> items = new();
        foreach(BattleRewardData rewardPool in rewards)
        {
            items.AddRange(rewardPool.rewardItems);
            totalXP += rewardPool.rewardXP;
            totalScraps += rewardPool.scrapsReward;
        }

        Inventory.Singleton.AddItems(items);
        PlayerDataHandler.Singleton.AddXPToPresentCharacters(totalXP);
        PlayerDataHandler.Singleton.ModifyMoney(totalScraps);

        BuildRewardMessages(totalXP, totalScraps, items);
    }
    private void BuildRewardMessages(int xp, int scraps, List<ItemData> items)
    {
        messages.Clear();
        foreach( var character in PlayerDataHandler.Singleton.currentPlayerData.characters)
        {
            if (xp != 0)
            AddMessage(character.characterName + " has gained " + xp + " XP.");
        }
        AddMessage(BuildLootMessage(items));
        if (scraps != 0)
            AddMessage("You picked up " + scraps + " scraps.");
    }

    private string BuildLootMessage(List<ItemData> items )
    {
        if (items == null || items.Count == 0)
            return "You did not receive any items.";

        _newItemMaps.Clear();

        foreach (ItemData item in items)
        {
            if (!_newItemMaps.ContainsKey(item.itemName))
            {
                _newItemMaps[item.itemName] = 0;
            }
            _newItemMaps[item.itemName]++;
        }

        List<string> itemMessages = new List<string>();

        foreach (KeyValuePair<string, int> item in _newItemMaps)
        {
            itemMessages.Add("You got " + item.Value + " " + item.Key + "." );
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

        if (markQuestComplete)
        {
            QuestManager.instance.MarkQuestComplete(
                questToComplete
            );
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

