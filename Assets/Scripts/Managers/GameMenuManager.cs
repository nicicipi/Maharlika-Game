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
    public GameObject DemoObject;
    public void EndDemo()
    {
        StartCoroutine(DemoEndRoutine());
    }
    private IEnumerator DemoEndRoutine()
    {
        GameManager.Singleton.SetGameState(GameManager.GameState.Cutscene);
        yield return SceneTransition.instance.FadeToBlack();
        DemoObject.SetActive(true);
        yield return SceneTransition.instance.FadeFromBlack();

        yield return new WaitForSeconds(10f);

        yield return SceneTransition.instance.FadeToBlack();
        DemoObject.SetActive(false);

        QuitGame();
        yield return SceneTransition.instance.FadeFromBlack();
        GameManager.Singleton.SetGameState(GameManager.GameState.Roaming);
    }
    [Header("CharacterData")]
    public CharacterInfo[] characterInfos;
    private List<Character> characters;
    [SerializeField] GameObject[] statsButtons;

    public static GameMenuManager instance;

    [SerializeField] TextMeshProUGUI[] nameText, hpText, spText, lvlText, xpText;
    [SerializeField] Slider[] hpSlider, spSlider, xpSlider;
    [SerializeField] Image[] characterImage;
    [SerializeField] GameObject[] characterPanel;
    [SerializeField] GameObject characterInfoPanel, itemsPanel, statPanel, configPanel, quitPanel;

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


        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Singleton.CurrentPlayingState == (GameManager.PlayState.Menus))
            {
                GameManager.Singleton.SetPlayState(GameManager.PlayState.Gameplay);
                UpdateStats();
                CloseMenu();
            }
            else
            {

                if (GameManager.Singleton.CurrentGameState != GameManager.GameState.Roaming)
                    return;

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
        characters = PlayerDataHandler.Singleton.currentPlayerData.characters;
        
        foreach(var info in characterInfos)
        {
            info.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < characters.Count; i++)
        {
            characterInfos[i].gameObject.SetActive(true);
            characterInfos[i].UpdateInfo(characters[i]);
        }
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

        foreach (var item in Inventory.Singleton.GetItemsList())
        {
            if (item == null)
            {
                continue;
            }

            RectTransform itemSlot = Instantiate(itemSlotContainer, itemSlotContainerParent).GetComponent<RectTransform>();

            Image itemImage = itemSlot.Find("Item Image").GetComponent<Image>(); //item image remember this when checking for the sprite of your items in inventory, if you change it, it wont work here
            itemImage.sprite = item.itemData.itemsImage;

            TextMeshProUGUI itemsAmountText = itemSlot.Find("Amount Text").GetComponent<TextMeshProUGUI>();
            //if (item.amount > 1)
            //    itemsAmountText.text = item.amount.ToString();
            //else
                itemsAmountText.text = "";

            itemSlot.GetComponent<ItemButton>().SetupItem(item.itemData, item.amount);
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
        if(activeItem.itemType == ItemData.ItemType.Item)
            Inventory.Singleton.RemoveItem(activeItem);
        switch (selectedCharacter)
        {
            case 0:
                PlayerDataHandler.Singleton.currentPlayerData.GetCharacter("Lyka").stats.UseItemOnCharacter(activeItem);
                break;
            case 1:
                PlayerDataHandler.Singleton.currentPlayerData.GetCharacter("Briar").stats.UseItemOnCharacter(activeItem);
                break;
            case 2:
                PlayerDataHandler.Singleton.currentPlayerData.GetCharacter("Adam").stats.UseItemOnCharacter(activeItem);
                break;
        }
        UpdateItemsInventory();
        UpdateStats();
    }
    public Button LykaButton, BriarButton, AdamButton;
    public void OpenCharacterChoicePanel()
    {
        if (activeItem == null)
            return;
        characterChoicePanel.SetActive(true);
        LykaButton.gameObject.SetActive(false);
        BriarButton.gameObject.SetActive(false);
        AdamButton.gameObject.SetActive(false);

        LykaButton.gameObject.SetActive(PlayerDataHandler.Singleton.currentPlayerData.GetCharacter("Lyka") != null);
        BriarButton.gameObject.SetActive(PlayerDataHandler.Singleton.currentPlayerData.GetCharacter("Briar") != null);
        AdamButton.gameObject.SetActive(PlayerDataHandler.Singleton.currentPlayerData.GetCharacter("Adam") != null);
    }
    //    if (activeItem == null)
    //    {
    //        Debug.LogWarning(
    //            "GameMenuManager: Select an item before choosing a character."
    //        );

        //        return;
        //    }

        //    if (GameManager.Singleton == null || characterChoicePanel == null)
        //    {
        //        Debug.LogError(
        //            "GameMenuManager: GameManager or character choice panel is missing."
        //        );

        //        return;
        //    }

        //    //GameManager.Singleton.RefreshPlayerStats();
        //    //playerStats = GameManager.Singleton.GetPlayerStats();

        //    //if (playerStats == null || playerStats.Length == 0)
        //    //{
        //    //    Debug.LogError(
        //    //        "GameMenuManager: Cannot show character choices because the party roster is empty."
        //    //    );

        //    //    return;
        //    //}

        //    //List<int> availablePlayerIndices = new List<int>();

        //    //for (int i = 0; i < playerStats.Length; i++)
        //    //{
        //    //    if (playerStats[i] != null &&
        //    //        playerStats[i].gameObject.activeInHierarchy)
        //    //    {
        //    //        availablePlayerIndices.Add(i);
        //    //    }
        //    //}

        //    //if (availablePlayerIndices.Count == 0)
        //    //{
        //    //    Debug.LogError(
        //    //        "GameMenuManager: No active PlayerStats objects are registered. " +
        //    //        "Check the GameManager roster and active party members."
        //    //    );

        //    //    return;
        //    //}

        //    foreach (KeyValuePair<Button, UnityEngine.Events.UnityAction> handler
        //        in characterChoiceHandlers)
        //    {
        //        if (handler.Key != null)
        //        {
        //            handler.Key.onClick.RemoveListener(handler.Value);
        //        }
        //    }

        //    characterChoiceHandlers.Clear();

        //    List<Button> optionButtons = new List<Button>();
        //    Button[] panelButtons =
        //        characterChoicePanel.GetComponentsInChildren<Button>(true);

        //    foreach (Button button in panelButtons)
        //    {
        //        if (button == null ||
        //            button.transform == characterChoicePanel.transform ||
        //            button.GetComponentInChildren<TextMeshProUGUI>(true) == null)
        //        {
        //            continue;
        //        }

        //        optionButtons.Add(button);
        //    }

        //    if (optionButtons.Count == 0)
        //    {
        //        Debug.LogError(
        //            "GameMenuManager: No character option Buttons with text were found " +
        //            "inside Character Choice Panel."
        //        );

        //        return;
        //    }

        //    for (int i = 0; i < optionButtons.Count; i++)
        //    {
        //        Button optionButton = optionButtons[i];
        //        Transform optionRoot = GetCharacterChoiceOptionRoot(
        //            optionButton.GetComponentInChildren<TextMeshProUGUI>(true)
        //        );

        //        if (optionRoot == null)
        //        {
        //            continue;
        //        }

        //        if (i >= availablePlayerIndices.Count)
        //        {
        //            optionRoot.gameObject.SetActive(false);
        //            continue;
        //        }

        //        int playerIndex = availablePlayerIndices[i];
        //        PlayerStats selectedPlayer = playerStats[playerIndex];
        //        TextMeshProUGUI characterName =
        //            optionButton.GetComponentInChildren<TextMeshProUGUI>(true);

        //        characterName.text = selectedPlayer.playerName;
        //        optionRoot.gameObject.SetActive(true);
        //        optionButton.interactable = true;

        //        UnityEngine.Events.UnityAction action =
        //            () => UseItemOnPlayer("selectedPlayer");

        //        if (!HasPersistentUseItemListener(optionButton))
        //        {
        //            optionButton.onClick.AddListener(action);
        //            characterChoiceHandlers.Add(optionButton, action);
        //        }
        //    }

        //    characterChoicePanel.SetActive(true);
        //}

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

        GameManager.Singleton.SetGameState(GameManager.GameState.Roaming);
        GameManager.Singleton.SetPlayState(GameManager.PlayState.MainMenu);
        Debug.Log("Quit Game");
    }
    public void CloseMenu()
    {
        GameManager.Singleton.SetPlayState(GameManager.PlayState.Gameplay);
        characterInfoPanel.SetActive(true);
        CloseEverythingInMenu();
        menu.SetActive(false);

        AudioManager.instance.PlaySFX(18);
    }

    public void CloseEverythingInMenu()
    {
        itemsPanel.SetActive(false);
        statPanel.SetActive(false);
        configPanel.SetActive(false);
        quitPanel.SetActive(false);
    }

    public void OpenInventory()
    {
        itemsPanel.SetActive(true);
        UpdateItemsInventory();
    }
}
