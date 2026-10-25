using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SaveSystem;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public GameObject shopMenu, buyPanel, sellPanel, talkPanel, quitPanel, shopkeeper;

    [SerializeField] TextMeshProUGUI currentMoneyText;

    public List<ItemData> itemsForSale;

    [SerializeField] GameObject itemSlotContainer;
    [SerializeField] Transform itemSlotBuyContainerParent;
    [SerializeField] Transform itemSlotSellContainerParent;

    [SerializeField] ItemData selectedItem;
    [SerializeField] TextMeshProUGUI buyItemName, buyItemDescription, buyItemValue;
    [SerializeField] TextMeshProUGUI sellItemName, sellItemDescription, sellItemValue;

    public enum ShopStates
    {
        Buy,
        Sell,
        Talk,
        Quit
    }
    [SerializeField] private ShopStates _currentState;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetShopState(ShopStates state)
    {
        buyPanel.SetActive(false);
        sellPanel.SetActive(false);
        quitPanel.SetActive(false);
        talkPanel.SetActive(false);

        switch (state)
        {
            case ShopStates.Talk:
                talkPanel.SetActive(true);
                break;
            case ShopStates.Buy:
                buyPanel.SetActive(true);
                UpdateItemsInShop(itemSlotBuyContainerParent, itemsForSale);
                break;
            case ShopStates.Sell:
                sellPanel.SetActive(true);
                UpdateItemsInShop(itemSlotSellContainerParent, Inventory.Singleton.GetItemsList());
                break;
            case ShopStates.Quit:
                quitPanel.SetActive(true);
                break;
        }
    }
    public void OpenShopMenu()
    {
        GameManager.Singleton.SetGameState(GameManager.GameState.Shop);
        currentMoneyText.text = "" + PlayerDataHandler.Singleton.currentPlayerData.money;
        
        shopMenu.SetActive(true);
        SetShopState(ShopStates.Talk);
    }
    public void CloseShopMenu()
    {
        GameManager.Singleton.SetGameState(GameManager.GameState.Roaming);
    }

    private void UpdateItemsInShop(Transform itemSlotContainerParent, List<ItemData> itemsToLookThrough)
    {
        foreach (Transform itemSlot in itemSlotContainerParent)
        {
            Destroy(itemSlot.gameObject);
        }

        foreach (ItemData item in itemsToLookThrough)
        {
            RectTransform itemSlot = Instantiate(itemSlotContainer, itemSlotContainerParent).GetComponent<RectTransform>();

            Image itemImage = itemSlot.Find("Item Image").GetComponent<Image>(); //item image remember this when checking for the sprite of your items in inventory, if you change it, it wont work here
            itemImage.sprite = item.itemsImage;

            TextMeshProUGUI itemsAmountText = itemSlot.Find("Amount Text").GetComponent<TextMeshProUGUI>();
            //if (item.amount > 1)
            //    itemsAmountText.text = ""; //item.amount.ToString(); -- makes it infinite amount of shopkeeper selling an item
            //else
            //    itemsAmountText.text = "";

            if (itemSlotContainerParent == itemSlotSellContainerParent && item.amount > 1)
                itemsAmountText.text = item.amount.ToString();
            else
                itemsAmountText.text = ""; // Infinite/hidden count for merchant stock

            itemSlot.GetComponent<ItemButton>().itemOnButton = item;
        }
    }

    public void SelectedBuyItem(ItemData itemToBuy)
    {
        selectedItem = itemToBuy;
        buyItemName.text = selectedItem.itemName;
        buyItemDescription.text = selectedItem.itemDescription;
        buyItemValue.text = "" + selectedItem.valueInCoins;
    }
    public void SelectedSellItem(ItemData itemToSell)
    {
        selectedItem = itemToSell;
        sellItemName.text = selectedItem.itemName;
        sellItemDescription.text = selectedItem.itemDescription;
        sellItemValue.text = "" + (int)(selectedItem.valueInCoins*0.75f);
    }


    public void BuyItem()
    {
        if(PlayerDataHandler.Singleton.currentPlayerData.money >= selectedItem.valueInCoins)
        {
            PlayerDataHandler.Singleton.currentPlayerData.money -= selectedItem.valueInCoins;
            Inventory.Singleton.AddItem(selectedItem);

            currentMoneyText.text = "" + PlayerDataHandler.Singleton.currentPlayerData.money;
        }
    }

    public void SellItem()
    {
        if (selectedItem != null)
        {
            PlayerDataHandler.Singleton.currentPlayerData.money += (int)(selectedItem.valueInCoins * 0.75f);
            Inventory.Singleton.RemoveItem(selectedItem);

            currentMoneyText.text = "" + PlayerDataHandler.Singleton.currentPlayerData.money;

            // Check if the item is still present in the player's inventory
            if (Inventory.Singleton.GetItemsList().Contains(selectedItem))
            {
                // Refresh the item slots while keeping this item highlighted
                UpdateItemsInShop(itemSlotSellContainerParent, Inventory.Singleton.GetItemsList());
                SelectedSellItem(selectedItem);
            }
            else
            {
                // No more copies left in inventory: clear selection and reset details
                selectedItem = null;
                //sellItemName.text = "";
                //sellItemDescription.text = "";
                //sellItemValue.text = "";
            }
        }
    }


}
