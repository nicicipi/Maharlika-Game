using SaveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public List<ItemDataWrapper> itemsList = new();
    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
    }
    public void AddItem(ItemData item, int count = 1)
    {
        if (item == null || count <= 0) return;

        ItemDataWrapper existingItem = itemsList.Find(x => x.itemData == item);

        if (existingItem != null)
        {
            if(existingItem.itemData.isStackable)
                existingItem.amount += count;
        }
        else
        {
            itemsList.Add(new ItemDataWrapper { itemData = item, amount = count });
        }
    }
    public void AddItems(List<ItemData> items)
    {
        foreach (var item in items)
            AddItem(item, 1);
    }

    public void RemoveItem(ItemData item, int count = 1)
    {
        if (item == null) 
            return;
        
        ItemDataWrapper existingItem = itemsList.Find(x => x.itemData == item);
        
        if (existingItem == null) 
            return;
        
        existingItem.amount -= count;
        
        if (existingItem.amount <= 0) 
            itemsList.Remove(existingItem);
    }
    public bool HasItem(ItemData item)
    {
        if (item == null ) 
            return false;

        ItemDataWrapper existingItem = itemsList.Find(x => x.itemData == item);

        if (existingItem == null)
            return false;

        return true;


    }
    public List<ItemDataWrapper> GetItemsList()
    {
        return itemsList;
    }
    public void SaveData(PlayerData data)
    {
        data.items.Clear();
        foreach (var item in itemsList)
            if (item.itemData != null && item.amount > 0)
                data.items.Add(new Item { itemName = item.itemData.itemName, itemCount = item.amount });
    }
    public void LoadData(PlayerData data)
    {
        itemsList.Clear();
        foreach (var item in data.items)
        {
            ItemData itemData = ItemsAssets.Singleton.GetItemData(item.itemName);
            if (itemData != null && item.itemCount > 0)
                itemsList.Add(new ItemDataWrapper { itemData = itemData, amount = item.itemCount });
        }
    }

    [Serializable]
    public class ItemDataWrapper
    {
        public ItemData itemData;
        public int amount;
    }
}
