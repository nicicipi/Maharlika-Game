using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    //[SerializeField] Image imageToFade;

    public GameObject menu;

    [SerializeField] GameObject[] statsButtons;

    public static GameMenuManager instance;

    private PlayerStats[] playerStats;

    [SerializeField] TextMeshProUGUI[] nameText, hpText, spText, lvlText, xpText;
    [SerializeField] Slider[] hpSlider, spSlider, xpSlider;
    [SerializeField] Image[] characterImage;
    [SerializeField] GameObject[] characterPanel;
    [SerializeField] GameObject characterInfoPanel, itemsPanel, statPanel, savePanel, gameSavedPanel, configPanel, quitPanel;

    [SerializeField] TextMeshProUGUI statLevel, statName, statHP, statSP, statDex, statDef, statAttack, statEquippedWeapon, statEquippedArmor;
    [SerializeField] TextMeshProUGUI statWeaponPower, statArmorDefence;
    [SerializeField] Image characterStatImage;

    [SerializeField] GameObject itemSlotContainer;
    [SerializeField] Transform itemSlotContainerParent;

    public TextMeshProUGUI itemName, itemDescription;

    public ItemData activeItem;

    [SerializeField] GameObject characterChoicePanel;
    [SerializeField] TextMeshProUGUI[] itemsCharacterChoiceNames;

    private readonly Dictionary<Button, UnityEngine.Events.UnityAction>
        characterChoiceHandlers =
            new Dictionary<Button, UnityEngine.Events.UnityAction>();

    [SerializeField] TextMeshProUGUI currentMoneyText;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (GameManager.Singleton.CurrentPlayingState == GameManager.PlayState.MainMenu || GameManager.Singleton.CurrentPlayingState == GameManager.PlayState.GameOver)
            return;

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (GameManager.Singleton.CurrentPlayingState == (GameManager.PlayState.Menus))
            {
                GameManager.Singleton.SetPlayState(GameManager.PlayState.Gameplay);
                UpdateStats();
                CloseMenu();
            }
            else
            {
                GameManager.Singleton.SetPlayState(GameManager.PlayState.Menus);
                currentMoneyText.text = PlayerDataHandler.Singleton.currentPlayerData.money.ToString();
                UpdateStats();

                menu.SetActive(true);
                AudioManager.instance.PlaySFX(19);
            }
        }
    }

    public void UpdateStats()
    {
        //playerStats = GameManager.Singleton.GetPlayerStats();

        //for(int i = 0; i < playerStats.Length; i++)
        //{
        //    characterPanel[i].SetActive(true);
        //    nameText[i].text = playerStats[i].playerName;
        //    hpText[i].text = playerStats[i].currentHP + "/" + playerStats[i].maxHP;
        //    spText[i].text = playerStats[i].currentStamina + "/" + playerStats[i].maxStamina;
        //    lvlText[i].text = playerStats[i].playerLevel + "" ;

        //    characterImage[i].sprite = playerStats[i].characterImage;

        //    hpSlider[i].maxValue = playerStats[i].maxHP;
        //    hpSlider[i].value = playerStats[i].currentHP;

        //    spSlider[i].maxValue = playerStats[i].maxStamina;
        //    spSlider[i].value = playerStats[i].currentStamina;

        //    xpText[i].text = playerStats[i].currentXP.ToString() + "/" + playerStats[i].xpForNextLevel[playerStats[i].playerLevel];
        //    xpSlider[i].maxValue = playerStats[i].xpForNextLevel[playerStats[i].playerLevel];
        //    xpSlider[i].value = playerStats[i].currentXP;
        //}
    }

    public void StatsMenu()
    {
        for(int i = 0; i < playerStats.Length; i++)
        {
            statsButtons[i].SetActive(true);

            statsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = playerStats[i].playerName;
        }

        StatsMenuUpdate(0);
    }

    public void StatsMenuUpdate(int playerSelectedNumber)
    {
        if(playerStats.Length == 0)
        {
            Debug.LogWarning("NO player stats to update");
            return;
        }
        PlayerStats playerSelected = playerStats[playerSelectedNumber];

        statName.text = playerSelected.playerName;
        statLevel.text = playerSelected.playerLevel + "";

        statHP.text = playerSelected.currentHP.ToString() + "/" + playerSelected.maxHP;
        statSP.text = playerSelected.currentStamina.ToString() + "/" + playerSelected.maxStamina;

        statAttack.text = playerSelected.attack.ToString();
        statDef.text = playerSelected.defence.ToString();
        statDex.text = playerSelected.dexterity.ToString();
        
        characterStatImage.sprite = playerSelected.characterImage;

        statEquippedWeapon.text = playerSelected.equippedWeaponName;
        statEquippedArmor.text = playerSelected.equippedArmorName;

        statWeaponPower.text = playerSelected.weaponPower.ToString();
        statArmorDefence.text = playerSelected.armorDefence.ToString();
    }

    public void UpdateItemsInventory()
    {
        itemSlotContainerParent = ResolveItemContentParent(
            itemSlotContainerParent
        );

        if (itemSlotContainerParent == null)
        {
            Debug.LogError(
                "GameMenuManager: Item slot content parent is not assigned."
            );

            return;
        }

        foreach (Transform itemSlot in itemSlotContainerParent)
        {
            if (itemSlot.GetComponent<ItemButton>() != null)
            {
                Destroy(itemSlot.gameObject);
            }
        }

        if (Inventory.Singleton == null)
        {
            Debug.LogError("GameMenuManager: Inventory Singleton is missing.");
            return;
        }

        foreach (ItemData item in Inventory.Singleton.GetItemsList())
        {
            if (item == null)
            {
                continue;
            }

            RectTransform itemSlot = Instantiate(itemSlotContainer, itemSlotContainerParent).GetComponent<RectTransform>();

            Image itemImage = itemSlot.Find("Item Image").GetComponent<Image>(); //item image remember this when checking for the sprite of your items in inventory, if you change it, it wont work here
            itemImage.sprite = item.itemsImage;

            TextMeshProUGUI itemsAmountText = itemSlot.Find("Amount Text").GetComponent<TextMeshProUGUI>();
            if (item.amount > 1)
                itemsAmountText.text = item.amount.ToString();
            else
                itemsAmountText.text = "";

            itemSlot.GetComponent<ItemButton>().itemOnButton = item;
        }
    }

    private static Transform ResolveItemContentParent(Transform configuredParent)
    {
        if (configuredParent == null)
        {
            return null;
        }

        Transform[] descendants =
            configuredParent.GetComponentsInChildren<Transform>(true);

        foreach (Transform descendant in descendants)
        {
            if (descendant != configuredParent &&
                descendant.name.IndexOf(
                    "Content",
                    System.StringComparison.OrdinalIgnoreCase
                ) >= 0)
            {
                return descendant;
            }
        }

        return configuredParent;
    }

    public void DiscardItem()
    {
        Inventory.Singleton.RemoveItem(activeItem);
        UpdateItemsInventory();
        AudioManager.instance.PlaySFX(15);
    }
    public void UseItem(int selectedCharacter)
    {
        Debug.Log("Use Item");
        //PlayerStats[] party = GameManager.Singleton.GetPlayerStats();

        //if (activeItem == null)
        //{
        //    return;
        //}

        //if (party == null ||
        //    selectedCharacter < 0 ||
        //    selectedCharacter >= party.Length ||
        //    party[selectedCharacter] == null ||
        //    !party[selectedCharacter].gameObject.activeInHierarchy)
        //{
        //    Debug.LogWarning(
        //        "GameMenuManager: Cannot use the selected item on that character."
        //    );

        //    return;
        //}

        //activeItem.UseItem(selectedCharacter);

        //Inventory.Singleton.RemoveItem(activeItem);
        //UpdateItemsInventory();
        //CloseCharacterChoicePanel();
        //activeItem = null;

        //if (AudioManager.Singleton != null)
        //{
        //    AudioManager.Singleton.PlaySFX(16);
        //}
    }

    private void UseItemOnPlayer(string selectedPlayer)
    {
        if (activeItem == null)
        {
            return;
        }

        //if (GameManager.Singleton == null || selectedPlayer == null)
        //{
        //    Debug.LogWarning(
        //        "GameMenuManager: The selected player is no longer available."
        //    );

        //    return;
        //}

        //PlayerStats[] party = GameManager.Singleton.GetPlayerStats();
        //int playerIndex = System.Array.IndexOf(party, selectedPlayer);

        //if (playerIndex < 0 || !selectedPlayer.gameObject.activeInHierarchy)
        //{
        //    Debug.LogWarning(
        //        "GameMenuManager: The selected player is not in the active party."
        //    );

        //    return;
        //}

        //UseItem(playerIndex);
    }

    public void OpenCharacterChoicePanel()
    {
        if (activeItem == null)
        {
            Debug.LogWarning(
                "GameMenuManager: Select an item before choosing a character."
            );

            return;
        }

        if (GameManager.Singleton == null || characterChoicePanel == null)
        {
            Debug.LogError(
                "GameMenuManager: GameManager or character choice panel is missing."
            );

            return;
        }

        //GameManager.Singleton.RefreshPlayerStats();
        //playerStats = GameManager.Singleton.GetPlayerStats();

        if (playerStats == null || playerStats.Length == 0)
        {
            Debug.LogError(
                "GameMenuManager: Cannot show character choices because the party roster is empty."
            );

            return;
        }

        List<int> availablePlayerIndices = new List<int>();

        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i] != null &&
                playerStats[i].gameObject.activeInHierarchy)
            {
                availablePlayerIndices.Add(i);
            }
        }

        if (availablePlayerIndices.Count == 0)
        {
            Debug.LogError(
                "GameMenuManager: No active PlayerStats objects are registered. " +
                "Check the GameManager roster and active party members."
            );

            return;
        }

        foreach (KeyValuePair<Button, UnityEngine.Events.UnityAction> handler
            in characterChoiceHandlers)
        {
            if (handler.Key != null)
            {
                handler.Key.onClick.RemoveListener(handler.Value);
            }
        }

        characterChoiceHandlers.Clear();

        List<Button> optionButtons = new List<Button>();
        Button[] panelButtons =
            characterChoicePanel.GetComponentsInChildren<Button>(true);

        foreach (Button button in panelButtons)
        {
            if (button == null ||
                button.transform == characterChoicePanel.transform ||
                button.GetComponentInChildren<TextMeshProUGUI>(true) == null)
            {
                continue;
            }

            optionButtons.Add(button);
        }

        if (optionButtons.Count == 0)
        {
            Debug.LogError(
                "GameMenuManager: No character option Buttons with text were found " +
                "inside Character Choice Panel."
            );

            return;
        }

        for (int i = 0; i < optionButtons.Count; i++)
        {
            Button optionButton = optionButtons[i];
            Transform optionRoot = GetCharacterChoiceOptionRoot(
                optionButton.GetComponentInChildren<TextMeshProUGUI>(true)
            );

            if (optionRoot == null)
            {
                continue;
            }

            if (i >= availablePlayerIndices.Count)
            {
                optionRoot.gameObject.SetActive(false);
                continue;
            }

            int playerIndex = availablePlayerIndices[i];
            PlayerStats selectedPlayer = playerStats[playerIndex];
            TextMeshProUGUI characterName =
                optionButton.GetComponentInChildren<TextMeshProUGUI>(true);

            characterName.text = selectedPlayer.playerName;
            optionRoot.gameObject.SetActive(true);
            optionButton.interactable = true;

            UnityEngine.Events.UnityAction action =
                () => UseItemOnPlayer("selectedPlayer");

            if (!HasPersistentUseItemListener(optionButton))
            {
                optionButton.onClick.AddListener(action);
                characterChoiceHandlers.Add(optionButton, action);
            }
        }

        characterChoicePanel.SetActive(true);
    }

    private bool HasPersistentUseItemListener(Button button)
    {
        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
        {
            if (button.onClick.GetPersistentTarget(i) == this &&
                button.onClick.GetPersistentMethodName(i) == nameof(UseItem))
            {
                return true;
            }
        }

        return false;
    }

    private Transform GetCharacterChoiceOptionRoot(
        TextMeshProUGUI characterName
    )
    {
        if (characterName == null || characterChoicePanel == null)
        {
            return null;
        }

        Transform optionRoot = characterName.transform;
        Transform panelTransform = characterChoicePanel.transform;

        while (optionRoot.parent != null &&
               optionRoot.parent != panelTransform)
        {
            optionRoot = optionRoot.parent;
        }

        return optionRoot.parent == panelTransform
            ? optionRoot
            : characterName.transform;
    }

    public void CloseCharacterChoicePanel()
    {
        characterChoicePanel.SetActive(false);
    }

    public void QuitGame()
    {
        //Application.Quit();
        SceneManager.LoadScene("MainMenu");
        CloseMenu();
        Debug.Log("Quit Game");
    }
    public void CloseMenu()
    {
        characterInfoPanel.SetActive(true);
        CloseEverythingInMenu();
        menu.SetActive(false);

        AudioManager.instance.PlaySFX(18);
    }

    public void CloseEverythingInMenu()
    {
        itemsPanel.SetActive(false);
        statPanel.SetActive(false);
        savePanel.SetActive(false);
        gameSavedPanel.SetActive(false);
        configPanel.SetActive(false);
        quitPanel.SetActive(false);
    }

    public void OpenSavePanel()
    {
        savePanel.SetActive(true);
        gameSavedPanel.SetActive(false);
    }

    public void ConfirmSave()
    {
        GameManager.Singleton.DebugSave();

        savePanel.SetActive(false);
        gameSavedPanel.SetActive(true);
    }

    public void CloseSavePanel()
    {
        savePanel.SetActive(false);
    }

    public void CloseGameSavedPanel()
    {
        gameSavedPanel.SetActive(false);
    }

    public void OpenInventory()
    {
        itemsPanel.SetActive(true);
        UpdateItemsInventory();
    }
}
