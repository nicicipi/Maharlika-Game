using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq; // for sorting -- player always first

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] PlayerStats[] playerStats;

    public bool gameMenuOpened, dialogueBoxOpened, shopOpened, battleIsActive;

    public int currentMoney;

    private readonly HashSet<string> defeatedEnemyIds =
        new HashSet<string>();

    // Start is called before the first frame update
    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        RefreshPlayerStats();
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


        if (gameMenuOpened || dialogueBoxOpened || shopOpened || battleIsActive)
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

    public void ResetRuntimeState()
    {
        gameMenuOpened = false;
        dialogueBoxOpened = false;
        shopOpened = false;
        battleIsActive = false;
        defeatedEnemyIds.Clear();
    }

    public void SaveData()
    {
        SavingPlayerPosition();
        SavingPlayerStats();

        PlayerPrefs.SetString(
            "Current_Scene",
            SceneManager.GetActiveScene().name
        );

        PlayerPrefs.SetInt(
            "Number_Of_Items",
            Inventory.instance.GetItemsList().Count
        );

        for (int i = 0; i < Inventory.instance.GetItemsList().Count; i++)
        {
            ItemsManager itemInInventory =
                Inventory.instance.GetItemsList()[i];

            PlayerPrefs.SetString(
                "Item_" + i + "_Name",
                itemInInventory.itemName
            );

            if (itemInInventory.isStackable)
            {
                PlayerPrefs.SetInt(
                    "Item_" + i + "_Amount",
                    itemInInventory.amount
                );
            }
        }

        foreach (string defeatedEnemyId in defeatedEnemyIds)
        {
            PlayerPrefs.SetInt(
                "EnemyDefeated_" + defeatedEnemyId,
                1
            );
        }

        PlayerPrefs.Save();
        Debug.Log("Data has been saved.");
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

            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Attack", playerStats[i].attack);
        }
    }

    public void LoadData()
    {
        if (playerStats == null ||
            playerStats.Length == 0)
        {
            Debug.LogError(
                "GameManager: Player roster is empty while loading data."
            );

            return;
        }

        if (Player.instance != null &&
            string.IsNullOrEmpty(Player.instance.transitionName))
        {
            LoadingPlayerPosition();
        }

        LoadingPlayerStats();

        // Keep the existing inventory-loading code below.
        for (int i = 0; i < PlayerPrefs.GetInt("Number_Of_Items"); i++)
        {
            string itemName = PlayerPrefs.GetString("Item_" + i + "_Name");
            ItemsManager itemToAdd =
                ItemsAssets.instance.GetItemAsset(itemName);

            if (itemToAdd == null)
            {
                Debug.LogWarning(
                    "GameManager: Could not find saved item: " +
                    itemName
                );

                continue;
            }

            Inventory.instance.AddItems(itemToAdd);

            string amountKey = "Item_" + i + "_Amount";

            if (itemToAdd.isStackable &&
                PlayerPrefs.HasKey(amountKey))
            {
                itemToAdd.amount =
                    PlayerPrefs.GetInt(amountKey);
            }
        }
    }

    private static void LoadingPlayerPosition()
    {
        if (Player.instance == null)
        {
            Debug.LogWarning(
                "GameManager: Cannot load player position because Player.instance is missing."
            );

            return;
        }

        Player.instance.transform.position = new Vector3(
            PlayerPrefs.GetFloat("Player_Pos_X"),
            PlayerPrefs.GetFloat("Player_Pos_Y"),
            PlayerPrefs.GetFloat("Player_Pos_Z")
            );
    }

    private void LoadingPlayerStats()
    {
        for (int i = 0; i < playerStats.Length; i++)
        {
            PlayerStats player = playerStats[i];
            string playerKey = "Player_" + player.playerName + "_";

            player.gameObject.SetActive(
                PlayerPrefs.GetInt(playerKey + "active", 1) == 1
            );

            player.playerLevel =
                PlayerPrefs.GetInt(playerKey + "Level");
            
            player.currentXP =
                PlayerPrefs.GetInt(playerKey + "CurrentXP");

            player.maxHP =
                PlayerPrefs.GetInt(playerKey + "MaxHP");

            player.currentHP =
                PlayerPrefs.GetInt(playerKey + "CurrentHP");

            player.maxStamina =
                PlayerPrefs.GetInt(playerKey + "MaxSP");

            player.currentStamina =
                PlayerPrefs.GetInt(playerKey + "CurrentSP");

            player.dexterity =
                PlayerPrefs.GetInt(playerKey + "Dexterity");

            player.defence =
                PlayerPrefs.GetInt(playerKey + "Defence");

            player.equippedWeaponName =
                PlayerPrefs.GetString(playerKey + "EquippedWeapon");

            player.equippedArmorName =
                PlayerPrefs.GetString(playerKey + "EquippedArmor");

            player.weaponPower =
                PlayerPrefs.GetInt(playerKey + "WeaponPower");

            player.armorDefence =
                PlayerPrefs.GetInt(playerKey + "ArmorDefence");
        }
    }

    public void RegisterDefeatedEnemy(string enemyId)
    {
        if (!string.IsNullOrEmpty(enemyId))
        {
            defeatedEnemyIds.Add(enemyId);
            PlayerPrefs.SetInt("EnemyDefeated_" + enemyId, 1);
            PlayerPrefs.Save();
        }
    }

    public void RefreshPlayerStats()
    {
        if (playerStats == null ||
            playerStats.Length == 0)
        {
            playerStats = FindObjectsOfType<PlayerStats>(true)
                .OrderBy(player => player.playerName)
                .ToArray();
        }

        if (playerStats == null ||
            playerStats.Length == 0)
        {
            Debug.LogError(
                "GameManager: No PlayerStats objects were found."
            );

            return;
        }

        foreach (PlayerStats player in playerStats)
        {
            if (player == null)
            {
                continue;
            }

            // Move the data objects under the persistent GameManager.
            // This prevents them from being destroyed during scene changes.
            if (player.transform.parent != transform)
            {
                player.transform.SetParent(transform, true);
            }
        }

        Debug.Log(
            "GameManager: Persistent player roster contains " +
            playerStats.Length +
            " player(s)."
        );
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}

