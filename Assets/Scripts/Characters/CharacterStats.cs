using System;
using UnityEngine;
using static ItemData;

namespace SaveSystem
{
    [Serializable]
    public class CharacterStats
    {
        public int maxLevel = 20;
        public int playerLevel = 1;
        public int currentXP = 0;
        public int[] xpForNextLevel;
        public int baseLevelXP = 100;

        public int currentHP;
        public int maxHP = 100;

        public int currentStamina;
        public int maxStamina = 100;

        public int attack = 10;
        public int defence = 9;
        public int special = 3;
        public int specialDefence = 5;
        public int dexterity = 3;

        public string equippedWeaponName = "None";
        public string equippedArmorName = "None";

        public int weaponPower;
        public int armorDefence;

        public CharacterStats()
        {
            playerLevel = 1;
            currentXP = 0;

            maxHP = 100;
            currentHP = 100;

            maxStamina = 100;
            currentStamina = maxStamina;

            attack = 10;
            defence = 9;
            special = 3;
            specialDefence = 5;
            dexterity = 3;

            equippedWeaponName = "None";
            equippedArmorName = "None";

            InitializeXPTable();
        }
        public CharacterStats(CharacterStats toCopy)
        {
            if (toCopy == null) return;

            this.maxLevel = toCopy.maxLevel;
            this.playerLevel = toCopy.playerLevel;
            this.currentXP = toCopy.currentXP;
            this.baseLevelXP = toCopy.baseLevelXP;
            this.currentHP = toCopy.currentHP;
            this.maxHP = toCopy.maxHP;
            this.currentStamina = toCopy.currentStamina;
            this.maxStamina = toCopy.maxStamina;
            this.attack = toCopy.attack;
            this.defence = toCopy.defence;
            this.special = toCopy.special;
            this.specialDefence = toCopy.specialDefence;
            this.dexterity = toCopy.dexterity;
            this.equippedWeaponName = toCopy.equippedWeaponName;
            this.equippedArmorName = toCopy.equippedArmorName;
            this.weaponPower = toCopy.weaponPower;
            this.armorDefence = toCopy.armorDefence;

            if(toCopy.xpForNextLevel == null)
            {
                InitializeXPTable();
            }
            else
            {
                this.xpForNextLevel = (int[])toCopy.xpForNextLevel.Clone();
            }
        }

        public void InitializeXPTable()
        {
            xpForNextLevel = new int[maxLevel + 1];
            xpForNextLevel[1] = baseLevelXP;

            for (int level = 2; level <= maxLevel; level++)
            {
                xpForNextLevel[level] = xpForNextLevel[level - 1] + 20 + level * 20;
            }
        }

        public void AddXP(int amountOfXP)
        {
            currentXP += amountOfXP;

            while (playerLevel < maxLevel && currentXP >= xpForNextLevel[playerLevel])
            {
                currentXP -= xpForNextLevel[playerLevel];
                playerLevel++;

                if (playerLevel % 2 == 0)
                {
                    attack++;
                    special++;
                    dexterity += 2;
                }
                else
                {
                    defence += 2;
                    specialDefence++;
                }

                maxHP += 15;
                currentHP = maxHP;

                maxStamina += 4;
                currentStamina = maxStamina;
            }
        }

        public void ModifyHP(int amountHPtoAdd)
        {
            currentHP = Mathf.Clamp(currentHP + amountHPtoAdd, 0, maxHP);
        }

        public void AddSP(int amountSPtoAdd)
        {
            currentStamina = Mathf.Clamp(currentStamina + amountSPtoAdd,0, maxStamina);
        }

        public void EquipWeapon(string weaponName, int weaponPower)
        {
            equippedWeaponName = weaponName;
            this.weaponPower = weaponPower;
        }

        public void EquipArmor(string armorName, int armorDefence)
        {
            equippedArmorName = armorName;
            this.armorDefence = armorDefence;
        }
        public void UnequipWeapon()
        {
            if (equippedWeaponName == "None")
            {
                return;
            }
            Inventory.Singleton.AddItem(equippedArmorName, 1);

            equippedWeaponName = "None";
            this.weaponPower = 0;
        }
        public void UnequipArmor()
        {
            if (equippedArmorName == "None")
            {
                return;
            }
            Inventory.Singleton.AddItem(equippedArmorName,1);
            equippedArmorName = "None";
            this.armorDefence = 0;
        }

        public void UseItemOnCharacter(ItemData item)
        {
            if (item.itemType == ItemType.Consumable)
            {
                AudioManager.instance.PlaySFX(16);
                if (item.affectType == AffectType.HP)
                {
                    ModifyHP(item.amountOfAffect);
                }
                else if (item.affectType == AffectType.SP)
                {
                    AddSP(item.amountOfAffect);
                }
            }
            else if (item.itemType == ItemType.Weapon)
            {
                AudioManager.instance.PlaySFX(4);
                UnequipWeapon();
                EquipWeapon(item.itemName, item.weaponPower);
                Inventory.Singleton.RemoveItem(item,1);
            }
            else if (item.itemType == ItemType.Armor)
            {
                AudioManager.instance.PlaySFX(4);
                UnequipArmor();
                EquipArmor(item.itemName, item.armorDefence);
                Inventory.Singleton.RemoveItem(item,1);
            }
        }
    }

}