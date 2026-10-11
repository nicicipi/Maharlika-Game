using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ItemsAssets : MonoBehaviour
{

    public static ItemsAssets instance;
    [SerializeField] ItemsManager[] itemsAvailable;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public ItemsManager GetItemAsset(string itemToGetName)
    {
        if (string.IsNullOrWhiteSpace(itemToGetName))
            return null;

        string requestedItemName = itemToGetName.Trim();
    
        foreach(ItemsManager item in itemsAvailable)
        {
            if (item != null &&
                !string.IsNullOrWhiteSpace(item.itemName) &&
                string.Equals(
                    item.itemName.Trim(),
                    requestedItemName,
                    System.StringComparison.OrdinalIgnoreCase
                ))
            {
                return item;
            }
        }

        return null;
    }
}
