using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public GameObject shopMenu;
    [SerializeField] TextMeshProUGUI currentMoneyText;

    [Header("Items")]
    public List<ItemData> itemsForSale;
    public List<ItemData> itemsForBuying;
    [SerializeField] private List<ItemButton> itemSlotPool = new();

    [SerializeField] ItemButton itemSlot;
    [SerializeField] Transform ItemContainer;
    [SerializeField] ItemData selectedItem;
    [SerializeField] TextMeshProUGUI ItemName;
    [SerializeField] TextMeshProUGUI ItemDescription;

    [Header("Shop buttons")]
    public GameObject buyPanel;
    public TextMeshProUGUI buyValueText;
    public GameObject sellPanel;
    public TextMeshProUGUI sellValueText;

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
    public void OpenShopMenu(List<ItemData> itemsForBuying)
    {
        this.itemsForBuying = itemsForBuying;
        GameManager.Singleton.SetGameState(GameManager.GameState.Shop);
        currentMoneyText.text = "" + PlayerDataHandler.Singleton.currentPlayerData.money;

        shopMenu.SetActive(true);
        SetShopState(1);
    }

    private void Refresh()
    {
        itemsForSale.Clear();
        foreach(var item in Inventory.Singleton.itemsList)
        {
            //Todo dont show stuff if the playere is wearing it.
            foreach (int i in Enumerable.Range(0, item.amount))
            {
                itemsForSale.Add(item.itemData);
            }
        }
    }
    public void SetShopState(int state)
    {
        buyPanel.SetActive(false);
        sellPanel.SetActive(false);
        Refresh();
        switch (state)
        {
            case 1:
                ItemName.text = "Shopkeeper";
                ItemDescription.text = "You looking for something?";

                buyValueText.text = "";
                sellValueText.text = "";
                ItemContainer.gameObject.SetActive(false);
                break;
            case 2:
                ItemName.text = "ShopKeeper";
                ItemDescription.text = "What'cha buying?";

                buyValueText.text = "";
                sellValueText.text = "";
                PopulateItemSlot(itemsForBuying);
                ItemContainer.gameObject.SetActive(true);
                buyPanel.SetActive(true);
                break;
            case 3:
                ItemName.text = "ShopKeeper";
                ItemDescription.text = "What'cha selling?";

                buyValueText.text = "";
                sellValueText.text = "";
                PopulateItemSlot(itemsForSale);
                ItemContainer.gameObject.SetActive(true);
                sellPanel.SetActive(true);
                break;
        }
    }
    private void PopulateItemSlot(List<ItemData> items)
    {
        while (items.Count > itemSlotPool.Count)
        {
            itemSlotPool.Add(Instantiate(itemSlot, ItemContainer));
        }
        foreach (var item in itemSlotPool)
        {
            item.gameObject.SetActive(false);
        }

        for(int i = 0; i < items.Count; i++ )
        {
            itemSlotPool[i].SetupItem(items[i]);
            itemSlotPool[i].gameObject.SetActive(true);
        }
    }
    public void CloseShopMenu()
    {
        GameManager.Singleton.SetGameState(GameManager.GameState.Roaming);
        SetShopState(1);
        shopMenu.gameObject.SetActive(false);
    }

    public void SelectItem(ItemData item)
    {
        if(item == null)
        {
            ItemName.text = "ShopKeeper";
            ItemDescription.text = "You looking for something?";

            buyValueText.text = "";
            sellValueText.text = "";

            return;
        }
        selectedItem = item;
        ItemName.text = selectedItem.itemName;
        ItemDescription.text = selectedItem.itemDescription;

        buyValueText.text = selectedItem.buyValue.ToString();
        sellValueText.text = selectedItem.sellValue.ToString();
    }
    public void BuyItem()
    {
        if (selectedItem == null)
            return;
        if(PlayerDataHandler.Singleton.currentPlayerData.money >= selectedItem.buyValue)
        {
            PlayerDataHandler.Singleton.currentPlayerData.money -= selectedItem.buyValue;
            Inventory.Singleton.AddItem(selectedItem);
            currentMoneyText.text = "" + PlayerDataHandler.Singleton.currentPlayerData.money;

            AudioManager.instance.PlaySFX(17);
            SetShopState(2);
            SelectItem(null);
        }
        else
        {
            SetShopState(1);
            ItemName.text = "ShopKeeper";
            ItemDescription.text = "Not Enough Money!";
        }
    }

    public void SellItem()
    {
        if (selectedItem == null)
            return;
        PlayerDataHandler.Singleton.currentPlayerData.money += (int)(selectedItem.sellValue * 0.75f);
        Inventory.Singleton.RemoveItem(selectedItem);
        AudioManager.instance.PlaySFX(15);

        currentMoneyText.text = "" + PlayerDataHandler.Singleton.currentPlayerData.money;
        SetShopState(3);
        SelectItem(null);
    }


}
