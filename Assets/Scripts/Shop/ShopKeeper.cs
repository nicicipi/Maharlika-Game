using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopKeeper : MonoBehaviour
{
    private bool canOpenShop;

    [SerializeField] List<ItemData> shopKeepersItemsForSale;
    [SerializeField] GameObject shopMenu, talkPanel;

    void Start()
    {
       
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            ShopManager.instance.itemsForSale = shopKeepersItemsForSale;
            ShopManager.instance.OpenShopMenu();

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //canOpenShop = false;
        }
    }

}
