using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image imageToFade;
    [SerializeField] GameObject menu;

    [SerializeField] GameObject[] statsButtons;

    public static MenuManager instance;

    private PlayerStats[] playerStats;
    [SerializeField] TextMeshProUGUI[] nameText, hpText, spText, lvlText, xpText;
    [SerializeField] Slider[] hpSlider, spSlider, xpSlider;
    [SerializeField] Image[] characterImage;
    [SerializeField] GameObject[] characterPanel;

    [SerializeField] TextMeshProUGUI statName, statHP, statSP, statDex, statDef, statAttack;
    [SerializeField] Image characterStatImage;

    [SerializeField] GameObject itemSlotContainer;
    [SerializeField] Transform itemSlotContainerParent;

    public TextMeshProUGUI itemName, itemDescription;

    public ItemsManager activeItem;

    [SerializeField] GameObject characterChoicePanel;
    [SerializeField] TextMeshProUGUI[] itemsCharacterChoiceNames;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            if(menu.activeInHierarchy)
            {
                UpdateStats();
                menu.SetActive(false);
                GameManager.instance.gameMenuOpened = false;

            } 
            else 
            {
                menu.SetActive(true);
                GameManager.instance.gameMenuOpened = true;
            }
        }
       
    }

    public void UpdateStats()
    {
        playerStats = GameManager.instance.GetPlayerStats();

        for(int i = 0; i < playerStats.Length; i++)
        {
            characterPanel[i].SetActive(true);
            nameText[i].text = playerStats[i].playerName;
            hpText[i].text = playerStats[i].currentHP + "/" + playerStats[i].maxHP;
            spText[i].text = playerStats[i].currentStamina + "/" + playerStats[i].maxStamina;
            lvlText[i].text = playerStats[i].playerLevel + "" ;

            characterImage[i].sprite = playerStats[i].characterImage;

            hpSlider[i].maxValue = playerStats[i].maxHP;
            hpSlider[i].value = playerStats[i].currentHP;

            spSlider[i].maxValue = playerStats[i].maxStamina;
            spSlider[i].value = playerStats[i].currentStamina;

            xpText[i].text = playerStats[i].currentXP.ToString() + "/" + playerStats[i].xpForNextLevel[playerStats[i].playerLevel];
            xpSlider[i].maxValue = playerStats[i].xpForNextLevel[playerStats[i].playerLevel];
            xpSlider[i].value = playerStats[i].currentXP;
        }
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
        PlayerStats playerSelected = playerStats[playerSelectedNumber];

        statName.text = playerSelected.playerName;

        statHP.text = playerSelected.currentHP.ToString() + "/" + playerSelected.maxHP;
        statSP.text = playerSelected.currentStamina.ToString() + "/" + playerSelected.maxStamina;

        statAttack.text = playerSelected.attack.ToString();
        statDef.text = playerSelected.defence.ToString();
        statDex.text = playerSelected.dexterity.ToString();
        
        characterStatImage.sprite = playerSelected.characterImage;
    }

    public void UpdateItemsInventory()
    {
        foreach (Transform itemSlot in itemSlotContainerParent)
        {
            Destroy(itemSlot.gameObject);
        }

        foreach (ItemsManager item in Inventory.instance.GetItemsList())
        {
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

    public void DiscardItem()
    {
        Inventory.instance.RemoveItem(activeItem);
        UpdateItemsInventory();
    }

    public void OpenCharacterChoicePanel()
    {
        characterChoicePanel.SetActive(true);

        for(int i = 0; i < playerStats.Length; i++)
        {
            PlayerStats activePlayer = GameManager.instance.GetPlayerStats()[i];
            itemsCharacterChoiceNames[i].text = activePlayer.playerName;

            bool activePlayerAvailable = activePlayer.gameObject.activeInHierarchy;
            itemsCharacterChoiceNames[i].transform.parent.gameObject.SetActive(activePlayerAvailable);
        }
    }

    public void CloseCharacterChoicePanel()
    {
        characterChoicePanel.SetActive(false);
    }

    public void UseItem()
    {
        activeItem.UseItem();
        OpenCharacterChoicePanel();
        DiscardItem(); // MOVE THIS AFTER
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void FadeImage()
    {
        imageToFade.GetComponent<Animator>().SetTrigger("Start Fading");
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        GameManager.instance.gameMenuOpened = false;
    }
}
