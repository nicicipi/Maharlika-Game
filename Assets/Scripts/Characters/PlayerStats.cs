using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    public string playerName;

    public Sprite characterImage;

    [SerializeField] int maxLevel = 20;
    public int playerLevel = 1;
    public int currentXP;
    public int[] xpForNextLevel;
    [SerializeField] int baseLevelXP = 100;

    public int maxHP = 100;
    public int currentHP;

    public int maxStamina = 100;
    public int currentStamina;

    public int attack;
    public int defence;
    [SerializeField] int special;
    [SerializeField] int specialDefence;
    public int dexterity;

    public string equippedWeaponName;
    public string equippedArmorName;

    public int weaponPower;
    public int armorDefence;

    public ItemsManager equippedWeapon, equippedArmor;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        InitializeExperienceRequirements();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddXP(100);
        }
    }

    private void InitializeExperienceRequirements()
    {
        xpForNextLevel = new int[maxLevel + 1];

        xpForNextLevel[1] = 50;

        for (int level = 2; level <= maxLevel; level++)
        {
            xpForNextLevel[level] =
                xpForNextLevel[level - 1] +
                20 +
                level * 20;
        }
    }

    public void AddXP(int amountOfXP)
    {
        currentXP += amountOfXP;

        while (playerLevel < maxLevel &&
               currentXP >= xpForNextLevel[playerLevel])
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

    public void AddHP(int amountHPtoAdd)
    {
        currentHP += amountHPtoAdd;
        if(currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void AddSP(int amountSPtoAdd)
    {
        currentStamina += amountSPtoAdd;
        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
    }

    public void EquipWeapon(ItemsManager weaponToEquip)
    {
        equippedWeapon = weaponToEquip;
        equippedWeaponName = equippedWeapon.itemName; 
        weaponPower = equippedWeapon.weaponDexterity;
    }

    public void EquipArmor(ItemsManager armorToEquip)
    {
        equippedArmor = armorToEquip;
        equippedArmorName = equippedArmor.itemName; 
        armorDefence = equippedArmor.armorDefence;
    }
}
