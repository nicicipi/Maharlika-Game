using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public enum ItemType { Item, Weapon, Armor }
    public ItemType itemType;

    public string itemName, itemDescription;
    public int valueInCoins;
    public Sprite itemsImage;

    public enum AffectType { HP, SP }
    public int amountOfAffect;
    public AffectType affectType;

    public int weaponDexterity;
    public int armorDefence;

    public bool isStackable;
    public int amount;

    public void UseItem()
    {
        if(itemType == ItemType.Item)
        {
            if(affectType == AffectType.HP)
            {
                PlayerStats.instance.AddHP(amountOfAffect);
            }
            else if (affectType == AffectType.SP)
            {
                PlayerStats.instance.AddSP(amountOfAffect);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // print("Triggered " + itemName);

            Inventory.instance.AddItems(this);
            SelfDestroy();
        }
    }

    public void SelfDestroy()
    {
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

}
