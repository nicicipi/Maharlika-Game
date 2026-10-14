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

    [Header("New Game Defaults")]
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private int startingXP = 0;
    [SerializeField] private int startingMaxHP = 100;
    [SerializeField] private int startingMaxStamina = 30;
    [SerializeField] private int startingAttack = 10;
    [SerializeField] private int startingDefence = 9;
    [SerializeField] private int startingSpecial = 3;
    [SerializeField] private int startingSpecialDefence = 5;
    [SerializeField] private int startingDexterity = 3;

    // Start is called before the first frame update
    private void Start()
    {
        instance = this;

        xpForNextLevel = new int[maxLevel + 1];

        xpForNextLevel[1] = baseLevelXP;

        for (int level = 2; level <= maxLevel; level++)
        {
            xpForNextLevel[level] =
                xpForNextLevel[level - 1] +
                20 +
                level * 20;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    AddXP(100);
        //}
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

    public void ResetToNewGameDefaults()
    {
        playerLevel = startingLevel;
        currentXP = startingXP;

        maxHP = startingMaxHP;
        currentHP = startingMaxHP;

        maxStamina = startingMaxStamina;
        currentStamina = startingMaxStamina;

        attack = startingAttack;
        defence = startingDefence;
        special = startingSpecial;
        specialDefence = startingSpecialDefence;
        dexterity = startingDexterity;

        equippedWeaponName = "None";
        equippedArmorName = "None";
        equippedWeapon = null;
        equippedArmor = null;

        weaponPower = 0;
        armorDefence = 0;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
