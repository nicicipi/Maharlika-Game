using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopKeeper : MonoBehaviour
{
    private bool canOpenShop;

    [SerializeField] List<ItemsManager> shopKeepersItemsForSale;
    [SerializeField] GameObject shopMenu, talkPanel;
    //[SerializeField] private string sceneToLoad;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (canOpenShop && !Player.instance.deactivateMovement 
    //        && !ShopManager.instance.shopMenu.activeInHierarchy)
    //    {
    //        ShopManager.instance.itemsForSale = shopKeepersItemsForSale;
    //        ShopManager.instance.OpenShopMenu();
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //canOpenShop = true;
            ShopManager.instance.itemsForSale = shopKeepersItemsForSale;
            //SceneManager.LoadScene(sceneToLoad);

            //talkPanel.SetActive(true);
            //shopMenu.SetActive(true);
            ShopManager.instance.OpenShopMenu();
            Player.instance.deactivateMovement = true;

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
