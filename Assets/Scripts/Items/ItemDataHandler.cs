using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attach this script to a game object and use it to call for item functions.
/// </summary>
public class ItemDataHandler : MonoBehaviour
{
    public bool announce = true;
    public Inventory.ItemDataWrapper[] itemReference;
    public UnityEvent OnSuccessfulAdd;
    public UnityEvent OnSuccessfulRemove;
    public UnityEvent OnFailRemove;
    public void AddItem()
    {
        foreach (var item in itemReference)
        {
            if (announce)
                NotificationPanel.Singleton.ShowNotification($"Added {(item.amount > 1 ? item.amount + " " : "") + item.itemData.itemName} to inventory");

            Inventory.Singleton.AddItem(item.itemData, item.amount);
        }
        OnSuccessfulAdd?.Invoke();
    }
    public void RemoveItem()
    {
        foreach (var item in itemReference)
        {
            if (Inventory.Singleton.RemoveItem(item.itemData, item.amount))
            {
                OnSuccessfulRemove?.Invoke();
            }
            else
            {
                OnFailRemove?.Invoke();
            }
        }
    }
}
