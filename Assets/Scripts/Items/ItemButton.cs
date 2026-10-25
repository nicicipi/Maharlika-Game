using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public ItemData itemOnButton;
    public Image image;
    public TextMeshProUGUI amountText;

    public void SetupItem(ItemData item, int amount = 1)
    {
        itemOnButton = item;
        if(item.itemsImage != null)
        image.sprite = item.itemsImage;
        if (amount <= 1)
        {
            amountText.text = "";
            return;
        }
        amountText.text = "" + amount;
    }
    public void Press()
    {
        GameMenuManager.instance.itemName.text = itemOnButton.itemName;
        GameMenuManager.instance.itemDescription.text = itemOnButton.itemDescription;
        GameMenuManager.instance.activeItem = itemOnButton;

        ShopManager.instance.SelectItem(itemOnButton);
        
        BattleManager.instance.SelectItem(itemOnButton);
    }
}
