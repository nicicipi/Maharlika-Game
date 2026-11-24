using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public ItemData itemOnButton;
    public Image image;
    public TextMeshProUGUI amountText;
    private UnityAction<ItemData> _onSelectCallback;
    public void SetupItem(ItemData item, int amount = 1, UnityAction<ItemData> OnSelectCallback = null)
    {
        itemOnButton = item;
        _onSelectCallback = OnSelectCallback;
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

        _onSelectCallback?.Invoke(itemOnButton);
        //BattleManager.instance.SelectItem(itemOnButton);
    }
}
