using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ItemsAssets : MonoBehaviour
{

    public static ItemsAssets Singleton;
    [SerializeField] ItemData[] itemsAvailable;
    private Dictionary<string, ItemData> _itemMap = new();
    private void Awake()
    {
        if(Singleton != null && Singleton != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Singleton = this;
        }
    }
    private void Start()
    {
        foreach(var item in itemsAvailable)
        {
            _itemMap[item.itemName] = item; 
        }
    }

    public ItemData GetItemData(string itemID)
    {
        if (string.IsNullOrWhiteSpace(itemID))
            return null;

        string requestedItemName = itemID.Trim();

        if(_itemMap.TryGetValue(requestedItemName, out ItemData value))
        {
            return value;
        }
        return null;
    }
}
