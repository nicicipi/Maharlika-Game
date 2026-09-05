using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // for sorting -- player always first

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] PlayerStats[] playerStats;

    public bool gameMenuOpened, dialogueBoxOpened, shopOpened;

    public int currentMoney;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);

        //If want the players to be automatically put in the gamemanager
        //playerStats = FindObjectsOfType<PlayerStats>();

        // Find all party members and player tag is always first
        playerStats = FindObjectsOfType<PlayerStats>()
            .OrderByDescending(p => p.CompareTag("Player"))
            .ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Data has been saved");
            SaveData();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Data has been loaded");
            LoadData();
        }


        if (gameMenuOpened || dialogueBoxOpened || shopOpened)
        {
            Player.instance.deactivateMovement = true; 
        }
        else 
        {
            Player.instance.deactivateMovement = false; 
        }
        
    }

    public PlayerStats[] GetPlayerStats()
    {
        return playerStats;
    }

    public void SaveData()
    {
        SavingPlayerPosition();
        SavingPlayerStats();

        PlayerPrefs.SetInt("Number_Of_Items", Inventory.instance.GetItemsList().Count);
        for (int i = 0; i < Inventory.instance.GetItemsList().Count; i++)
        {
            ItemsManager itemInInventory = Inventory.instance.GetItemsList()[i];
            PlayerPrefs.SetString("Item_" + i + "_Name", itemInInventory.itemName);

            if (itemInInventory.isStackable)
            {
                PlayerPrefs.SetInt("Item_" + i + "_Amount", itemInInventory.amount);
            }
        }
    }

    private static void SavingPlayerPosition()
    {
        PlayerPrefs.SetFloat("Player_Pos_X", Player.instance.transform.position.x);
        PlayerPrefs.SetFloat("Player_Pos_Y", Player.instance.transform.position.y);
        PlayerPrefs.SetFloat("Player_Pos_Z", Player.instance.transform.position.z);
    }

    private void SavingPlayerStats()
    {
        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy)
            {
                PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_active", 1);
            }
            else
            {
                PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_active", 0);
            }

            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Level", playerStats[i].playerLevel);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_CurrentXP", playerStats[i].currentXP);

            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_MaxHP", playerStats[i].maxHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_CurrentHP", playerStats[i].currentHP);

            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_MaxSP", playerStats[i].maxStamina);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_CurrentSP", playerStats[i].currentStamina);

            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Dexterity", playerStats[i].dexterity);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Defence", playerStats[i].defence);

            PlayerPrefs.SetString("Player_" + playerStats[i].playerName + "_EquippedWeapon", playerStats[i].equippedWeaponName);
            PlayerPrefs.SetString("Player_" + playerStats[i].playerName + "_EquippedArmor", playerStats[i].equippedArmorName);

            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_WeaponPower", playerStats[i].weaponPower);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_ArmorDefence", playerStats[i].armorDefence);

        }
    }

    public void LoadData()
    {
        LoadingPlayerPosition();
        LoadingPlayerStats();

        for (int i = 0; i < PlayerPrefs.GetInt("Number_Of_Items"); i++)
        {
            string itemName = PlayerPrefs.GetString("Item_" + i + "_Name");
            ItemsManager itemToAdd = ItemsAssets.instance.GetItemAsset(itemName);

            int itemAmount = 0;
            if(PlayerPrefs.HasKey("Items_" + i + "_Amount"))
            {
                itemAmount = PlayerPrefs.GetInt("Items_" + i + "_Amount");
            }

            Inventory.instance.AddItems(itemToAdd);
            if(itemToAdd.isStackable && itemAmount > 1)
            {
                itemToAdd.amount = itemAmount;
            }
        }
    }

    private void LoadingPlayerStats()
    {
        for (int i = 0; i < playerStats.Length; i++)
        {
            if (PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_active") == 0)
            {
                playerStats[i].gameObject.SetActive(false);
            }
            else
            {
                playerStats[i].gameObject.SetActive(true);
            }

            playerStats[i].playerLevel = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Level");
            playerStats[i].currentXP = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_CurrentXP");

            playerStats[i].maxHP = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_MaxHP");
            playerStats[i].currentHP = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_CurrentHP");

            playerStats[i].maxStamina = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_MaxSP");
            playerStats[i].currentStamina = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_CurrentSP");

            playerStats[i].dexterity = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Dexterity");
            playerStats[i].defence = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Defence");

            playerStats[i].equippedWeaponName = PlayerPrefs.GetString("Player_" + playerStats[i].playerName + "_EquippedWeapon");
            playerStats[i].equippedArmorName = PlayerPrefs.GetString("Player_" + playerStats[i].playerName + "_EquippedArmor");

            playerStats[i].weaponPower = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_WeaponPower");
            playerStats[i].armorDefence = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_ArmorDefence");
        }
    }

    private static void LoadingPlayerPosition()
    {
        Player.instance.transform.position = new Vector3(
            PlayerPrefs.GetFloat("Player_Pos_X"),
            PlayerPrefs.GetFloat("Player_Pos_Y"),
            PlayerPrefs.GetFloat("Player_Pos_Z")
            );
    }
}

