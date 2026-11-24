using BattleSystem;
using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Inventory;

public class InventoryPanel : MonoBehaviour
{
    public Transform itemSlotParent;
    public ItemButton itemButtonPrefab;
    private ItemData _selectedItem;
    [SerializeField] private CharacterTargetPanel _characterTargetPanel;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _description;
    public void SetupInventory(List<ItemDataWrapper> items)
    {
        _characterTargetPanel.gameObject.SetActive(false);
        foreach (Transform child in itemSlotParent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < items.Count; i++)
        {
            ItemButton itemButton = Instantiate(itemButtonPrefab, itemSlotParent);
            itemButton.SetupItem(items[i].itemData, items[i].amount, DisplayItem);
        }
        gameObject.SetActive(true);
    }

    public void DisplayItem(ItemData item)
    {
        _selectedItem = item;
        _itemNameText.text = item.itemName;
        _description.text = item.itemDescription;
    }

    public void UseItemButton()
    {
        if(_selectedItem == null)
        {
            Debug.Log($"No item selected");
            return;
        }

        _characterTargetPanel.SetupTargets(BattleManager.instance.GetAllAllies(), UseItemOnCharacter);
    }

    public void UseItemOnCharacter(BattleCharacters character)
    {
        if (_selectedItem == null)
        {
            Debug.Log($"No item selected");
            return;
        }
        _selectedItem = null;
        _characterTargetPanel.gameObject.SetActive(false);
        character.CharacterData.stats.UseItemOnCharacter(_selectedItem);
        if(_selectedItem.itemType == ItemData.ItemType.Consumable)
        {
            Inventory.Singleton.RemoveItem(_selectedItem);
            BattleManager.instance.TriggerPlayerInputEnd();
        }
        else
        {
            BattleManager.instance.battleNotice.SetText("Equipped " + _selectedItem.itemName + " on " + character.CharacterData.characterName);
            BattleManager.instance.battleNotice.Activate();
            BattleManager.instance.TriggerPlayerInputEnd();
        }
    }
}
