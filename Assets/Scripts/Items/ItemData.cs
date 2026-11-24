using SaveSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable Data")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Consumable, Weapon, Armor }
    public ItemType itemType;

    public string itemName, itemDescription;
    public int sellValue;
    public int buyValue;
    public Sprite itemsImage;

    public enum AffectType { HP, SP }
    public int amountOfAffect;
    public AffectType affectType;

    public int weaponPower;
    public int armorDefence;

    public bool isStackable;
}
