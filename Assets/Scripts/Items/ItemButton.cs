using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public ItemData itemOnButton;


    public void Press()
    {
        if (GameMenuManager.instance.menu.activeInHierarchy)
        {
            //this is not great, bc its better to send the name and desc to the menu manager script
            GameMenuManager.instance.itemName.text = itemOnButton.itemName;
            GameMenuManager.instance.itemDescription.text = itemOnButton.itemDescription;

            GameMenuManager.instance.activeItem = itemOnButton;
            GameMenuManager.instance.OpenCharacterChoicePanel();
        }

        if (ShopManager.instance.shopMenu.activeInHierarchy)
        {
            if (ShopManager.instance.buyPanel.activeInHierarchy)
            {
                ShopManager.instance.SelectedBuyItem(itemOnButton);
            }
            else if (ShopManager.instance.sellPanel.activeInHierarchy)
            {
                ShopManager.instance.SelectedSellItem(itemOnButton);
            }
        }

        if (BattleManager.instance.itemsToUseMenu.activeInHierarchy)
        {
            BattleManager.instance.SelectItem(itemOnButton);
        }


    }
}
