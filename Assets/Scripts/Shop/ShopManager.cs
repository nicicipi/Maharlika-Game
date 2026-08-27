using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public GameObject shopMenu, buyPanel, sellPanel, talkPanel, shopkeeper;

    [SerializeField] TextMeshProUGUI currentMoneyText;

    // Start is called before the first frame update
    void Start()
    {
        //instance = this;
        OpenShopMenu();
        
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void OpenShopMenu()
    {
        shopMenu.SetActive(true);
        currentMoneyText.text = "" + GameManager.instance.currentMoney;
        talkPanel.SetActive(true);

    }


    public void OpenBuyPanel()
    {
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
    }

    public void OpenSellPanel()
    {
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);
    }

}
