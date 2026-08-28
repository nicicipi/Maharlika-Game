using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public GameObject shopMenu, buyPanel, sellPanel, talkPanel, quitPanel, shopkeeper;

    [SerializeField] TextMeshProUGUI currentMoneyText;

    public List<ItemsManager> itemsForSale;

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
    }
    public void OpenSellPanel()
    {
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);
        quitPanel.SetActive(false);
        talkPanel.SetActive(false);
        GameManager.instance.shopOpened = true;
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

}
