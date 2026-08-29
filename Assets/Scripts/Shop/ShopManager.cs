using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public GameObject shopMenu, buyPanel, sellPanel, talkPanel, quitPanel, shopkeeper;

    [SerializeField] TextMeshProUGUI currentMoneyText;

    public List<ItemsManager> itemsForSale;

    [SerializeField] GameObject itemSlotContainer;
    [SerializeField] Transform itemSlotBuyContainerParent;
    [SerializeField] Transform itemSlotSellContainerParent;

    [SerializeField] ItemsManager selectedItem;
    [SerializeField] TextMeshProUGUI buyItemName, buyItemDescription, buyItemValue;
    [SerializeField] TextMeshProUGUI sellItemName, sellItemDescription, sellItemValue;

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

    public void OpenShopMenu()
    {
        shopMenu.SetActive(true);
        talkPanel.SetActive(true);
        GameManager.instance.shopOpened = true;
        currentMoneyText.text = "" + GameManager.instance.currentMoney;
    }
    public void CloseShopMenu()
    {
        shopMenu.SetActive(false);
        quitPanel.SetActive(false);
        talkPanel.SetActive(true);
        GameManager.instance.shopOpened = false;
    }

    public void OpenBuyPanel()
    {
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
        quitPanel.SetActive(false);
        talkPanel.SetActive(false);
        GameManager.instance.shopOpened = true;

        UpdateItemsInShop(itemSlotBuyContainerParent, itemsForSale);
    }
    public void OpenSellPanel()
    {
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);
        quitPanel.SetActive(false);
        talkPanel.SetActive(false);
        GameManager.instance.shopOpened = true;
         
        UpdateItemsInShop(itemSlotSellContainerParent, Inventory.instance.GetItemsList());
    }

    private void UpdateItemsInShop(Transform itemSlotContainerParent, List<ItemsManager> itemsToLookThrough)
    {
        foreach (Transform itemSlot in itemSlotContainerParent)
        {
            Destroy(itemSlot.gameObject);
        }

        foreach (ItemsManager item in itemsToLookThrough)
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

    public void SelectedBuyItem(ItemsManager itemToBuy)
    {
        selectedItem = itemToBuy;
        buyItemName.text = selectedItem.itemName;
        buyItemDescription.text = selectedItem.itemDescription;
        buyItemValue.text = "" + selectedItem.valueInCoins;
    }
    public void SelectedSellItem(ItemsManager itemToSell)
    {
        selectedItem = itemToSell;
        sellItemName.text = selectedItem.itemName;
        sellItemDescription.text = selectedItem.itemDescription;
        sellItemValue.text = "" + (int)(selectedItem.valueInCoins*0.75f);
    }

    public void OpenQuitPanel()
    {
        buyPanel.SetActive(false);
        sellPanel.SetActive(false);
        quitPanel.SetActive(true);
        talkPanel.SetActive(false);
        GameManager.instance.shopOpened = true;
    }

    public void OpenTalkPanel()
    {
        talkPanel.SetActive(true);
        buyPanel.SetActive(false);
        sellPanel.SetActive(false);
        quitPanel.SetActive(false);
        GameManager.instance.shopOpened = true;
    }

    public void BuyItem()
    {
        if(GameManager.instance.currentMoney >= selectedItem.valueInCoins)
        {
            GameManager.instance.currentMoney -= selectedItem.valueInCoins;
            Inventory.instance.AddItems(selectedItem);

            currentMoneyText.text = "" + GameManager.instance.currentMoney;
        }
    }

    //public void SellItem()
    //{
    //    if (selectedItem)
    //    {
    //        GameManager.instance.currentMoney += (int)(selectedItem.valueInCoins * 0.75f);
    //        Inventory.instance.RemoveItem(selectedItem);

    //        currentMoneyText.text = "" + GameManager.instance.currentMoney;
    //        selectedItem = null;

    //        OpenSellPanel();
    //    }
    //}

    public void SellItem()
    {
        if (selectedItem != null)
        {
            GameManager.instance.currentMoney += (int)(selectedItem.valueInCoins * 0.75f);
            Inventory.instance.RemoveItem(selectedItem);

            currentMoneyText.text = "" + GameManager.instance.currentMoney;

            // Check if the item is still present in the player's inventory
            if (Inventory.instance.GetItemsList().Contains(selectedItem))
            {
                // Refresh the item slots while keeping this item highlighted
                UpdateItemsInShop(itemSlotSellContainerParent, Inventory.instance.GetItemsList());
                SelectedSellItem(selectedItem);
            }
            else
            {
                // No more copies left in inventory: clear selection and reset details
                selectedItem = null;
                //sellItemName.text = "";
                //sellItemDescription.text = "";
                //sellItemValue.text = "";

                OpenSellPanel();
            }
        }
    }


}
