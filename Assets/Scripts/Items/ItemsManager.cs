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

    public void UseItem(int characterToUseOn)
    {
        PlayerStats selectedChracter = GameManager.instance.GetPlayerStats()[characterToUseOn];

        if(itemType == ItemType.Item)
        {
            if(affectType == AffectType.HP)
            {
                selectedChracter.AddHP(amountOfAffect);
            }
            else if (affectType == AffectType.SP)
            {
                selectedChracter.AddSP(amountOfAffect);
            }
        }

        else if(itemType == ItemType.Weapon)
        {
            if(selectedChracter.equippedWeaponName != "None")
            {
                Inventory.instance.AddItems(selectedChracter.equippedWeapon);
            }
            selectedChracter.EquipWeapon(this);
        }
        else if(itemType == ItemType.Armor)
        {
            if (selectedChracter.equippedArmorName != "None") //double check if Player, Briar, or Adam have "" EMPTY in their player stats scripts in Unity
            {
                Inventory.instance.AddItems(selectedChracter.equippedArmor);
            }
            selectedChracter.EquipArmor(this);
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
