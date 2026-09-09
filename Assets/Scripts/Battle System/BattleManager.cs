//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    private bool isBattleActive;

    [SerializeField] GameObject battleScene;
    [SerializeField] List<BattleCharacters> activeCharacters = new List<BattleCharacters>();

    [SerializeField] Transform[] playerPositions, enemiesPositions;

    [SerializeField] BattleCharacters[] playerPrefabs, enemiesPrefabs;

    [SerializeField] int currentTurn;
    [SerializeField] bool waitingForTurn;
    [SerializeField] GameObject UIButtonHolder;

    [SerializeField] BattleMoves[] battleMovesList;

    [SerializeField] ParticleSystem characterAttackEffect;
    [SerializeField] CharacterDamageGUI damageText;

    [SerializeField] GameObject[] playerBattleStats;
    [SerializeField] TextMeshProUGUI[] playersNameText;
    [SerializeField] TextMeshProUGUI[] playerHealth, playerStamina;
    [SerializeField] Slider[] playerHealthSlider, playerStaminaSlider;

    [SerializeField] GameObject enemyTargetPanel;
    [SerializeField] BattleTargetButtons[] targetButtons;

    public GameObject skillChoicePanel;
    [SerializeField] BattleSkillButton[] skillButtons;

    public BattleNotification battleNotice;

    [SerializeField] float chanceToRunAway = 0.7f;

    public GameObject itemsToUseMenu;
    [SerializeField] ItemsManager selectedItem;
    [SerializeField] GameObject itemSlotContainer;
    [SerializeField] Transform itemSlotContainerParent;
    [SerializeField] TextMeshProUGUI itemName, itemDescription;


    // --- TOP NOTIFICATION PANEL UI --- NOT YET IMPLEMENTED SEP 9 2026
    //[Header("Battle Notice UI")]
    //[SerializeField] GameObject battleNoticePanel;
    //[SerializeField] TextMeshProUGUI battleNoticeText;
    //private Coroutine noticeCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartBattle(new string[] { "Robbie", "Aaronos", "Robbie" });
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            NextTurn();
        }

        CheckPlayerButtonHolder();

    }

    private void CheckPlayerButtonHolder()
    {
        if (isBattleActive)
        {
            if (waitingForTurn)
            {
                if (activeCharacters[currentTurn].IsPlayer())
                    UIButtonHolder.SetActive(true);
                else
                {
                    UIButtonHolder.SetActive(false);
                    StartCoroutine(EnemyMoveCoroutine());
                }
            }
        }
    }

    public void StartBattle(string[] enemiesToSpawn)
    {

        if (!isBattleActive)
        {
            SettingUpBattle();
            AddingPlayers();
            AddingEnemies(enemiesToSpawn);
            UpdatePlayerStats();

            waitingForTurn = true;
            //currentTurn = Random.Range(0, activeCharacters.Count); 
                //makes it that when you start a battle, enemies can go attack first since it starts on a random turn
            currentTurn = 0;

        }
    }

    private void AddingEnemies(string[] enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            if (enemiesToSpawn[i] != "")
            {
                for (int j = 0; j < enemiesPrefabs.Length; j++)
                {
                    if (enemiesPrefabs[j].characterName == enemiesToSpawn[i])
                    {
                        BattleCharacters newEnemy = Instantiate(
                            enemiesPrefabs[j],
                            enemiesPositions[i].position,
                            enemiesPositions[i].rotation,
                            enemiesPositions[i]
                        );

                        activeCharacters.Add(newEnemy);
                    }
                }
            }
        }
    }

    private void AddingPlayers()
    {
        for (int i = 0; i < GameManager.instance.GetPlayerStats().Length; i++)
        {
            if (GameManager.instance.GetPlayerStats()[i].gameObject.activeInHierarchy)
            {
                for (int j = 0; j < playerPrefabs.Length; j++)
                {
                    if (playerPrefabs[j].characterName == GameManager.instance.GetPlayerStats()[i].playerName)
                    {
                        BattleCharacters newPlayer = Instantiate(
                            playerPrefabs[j],
                            playerPositions[i].position,
                            playerPositions[i].rotation,
                            playerPositions[i]
                        );

                        activeCharacters.Add(newPlayer);

                        ImportPlayerStats(i);
                    }
                }
            }
        }
    }

    private void ImportPlayerStats(int i)
    {
        PlayerStats player = GameManager.instance.GetPlayerStats()[i];

        activeCharacters[i].currentHP = player.currentHP;
        activeCharacters[i].maxHP = player.maxHP;

        activeCharacters[i].currentSP = player.currentStamina;
        activeCharacters[i].maxSP = player.maxStamina;

        activeCharacters[i].dexterity = player.dexterity;
        activeCharacters[i].defence = player.defence;

        activeCharacters[i].wpnPower = player.weaponPower;
        activeCharacters[i].armorDefence = player.armorDefence;
    }

    private void SettingUpBattle()
    {
        isBattleActive = true;
        transform.position = new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            transform.position.z
            );
        GameManager.instance.battleIsActive = true;
 
        battleScene.SetActive(true);
    }

    private void NextTurn()
    {
        currentTurn++;

        if (currentTurn >= activeCharacters.Count)
            currentTurn = 0;

        waitingForTurn = true;
        UpdateBattle();
        UpdatePlayerStats();
    }

    private void UpdateBattle()
    {
        bool allEnemiesAreDead = true;
        bool allPlayerAreDead = true;

        for (int i = 0; i < activeCharacters.Count; i++)
        {
            if (activeCharacters[i].currentHP < 0)
                activeCharacters[i].currentHP = 0;

            if (activeCharacters[i].currentHP == 0)
            {
                //kill character

            }
            else
            {
                if (activeCharacters[i].IsPlayer())
                    allPlayerAreDead = false;
                else
                    allEnemiesAreDead = false;
            }
        }

        if(allEnemiesAreDead || allPlayerAreDead)
        {
            if (allEnemiesAreDead)
                print("Players won!");
            else if (allPlayerAreDead)
                print("You lost");

            battleScene.SetActive(false);
            GameManager.instance.battleIsActive = false;
            isBattleActive = false;
        }
        else
        {
            while (activeCharacters[currentTurn].currentHP == 0)
            {
                currentTurn++;
                if(currentTurn >= activeCharacters.Count)
                {
                    currentTurn = 0;
                }
            }
        }

    }

    public IEnumerator EnemyMoveCoroutine()
    {
        waitingForTurn = false;

        yield return new WaitForSeconds(1f);
        EnemyAttack();

        yield return new WaitForSeconds(1.5f);
        NextTurn();
    }

    private void EnemyAttack()
    {
        List<int> players = new List<int>();

        for (int i = 0; i < activeCharacters.Count; i++)
        {
            if (activeCharacters[i].IsPlayer() && activeCharacters[i].currentHP > 0)
            {
                players.Add(i);
            }
        }
        int selectedPlayerToAttack = players[Random.Range(0, players.Count)];

        int selectedAttack = Random.Range(0, activeCharacters[currentTurn].AttackMovesAvailable().Length);
        int movePower = 0;

        for (int i = 0; i < battleMovesList.Length; i++)
        {
            if (battleMovesList[i].moveName == activeCharacters[currentTurn].AttackMovesAvailable()[selectedAttack])
            {
                movePower = GettingMovePowerAndEffectInstanstiation(selectedPlayerToAttack, i);
            }
        }

        // instantiating the effect of which character is currently attacking
        InstantiateEffectOnAttackingCharacter();

        DealDamageToCharacters(selectedPlayerToAttack, movePower);

        UpdatePlayerStats();
    }

    private void InstantiateEffectOnAttackingCharacter()
    {
        Instantiate(
            characterAttackEffect,
            activeCharacters[currentTurn].transform.position,
            activeCharacters[currentTurn].transform.rotation
            );
    }

    private void DealDamageToCharacters(int selectedCharacterToAttack, int movePower)
    {
        float attackPower = activeCharacters[currentTurn].dexterity + activeCharacters[currentTurn].wpnPower;
        float defenceAmount = activeCharacters[selectedCharacterToAttack].defence + activeCharacters[selectedCharacterToAttack].armorDefence;

        float damageAmount = (attackPower / defenceAmount) * movePower * Random.Range(0.9f, 1.1f);
        int damageToGive = (int)damageAmount;

        damageToGive = CalculateCritical(damageToGive);

        Debug.Log(activeCharacters[currentTurn].characterName
            + " just dealt " + damageAmount + "(" + damageToGive
            + ")" + " to " + activeCharacters[selectedCharacterToAttack]);

        activeCharacters[selectedCharacterToAttack].TakeHPDamage(damageToGive);

        CharacterDamageGUI characterDamageText = Instantiate(
            damageText,
            activeCharacters[selectedCharacterToAttack].transform.position,
            activeCharacters[selectedCharacterToAttack].transform.rotation
        );

        characterDamageText.SetDamage(damageToGive);
    }

    private int CalculateCritical(int damageToGive)
    {
        if(Random.value <= 0.1f) // needs to be 0.1 so its a lower chance for a crit
        {
            Debug.Log("Critical Hit! Instead of " + damageToGive
                + " points. " + (damageToGive * 2) + " was dealt.");

            return (damageToGive * 2);
        }
        return damageToGive;

    }

    public void UpdatePlayerStats()
    {
        for(int i = 0; i < playersNameText.Length; i++)
        {
            if(activeCharacters.Count > i)
            {
                if (activeCharacters[i].IsPlayer())
                {
                    BattleCharacters playerData = activeCharacters[i];

                    playersNameText[i].text = playerData.characterName;

                    playerHealth[i].text = playerData.currentHP + "/" + playerData.maxHP;
                    playerStamina[i].text = playerData.currentSP + "/" + playerData.maxSP;

                    playerHealthSlider[i].maxValue = playerData.maxHP;
                    playerHealthSlider[i].value = playerData.currentHP;

                    playerStaminaSlider[i].maxValue = playerData.maxSP;
                    playerStaminaSlider[i].value = playerData.currentSP;
                }

                else
                {
                    //playersNameText[i].gameObject.SetActive(false); -- only removing the player name and not the sliders form the battle ui when a character isnt actually joined in the party
                    playerBattleStats[i].gameObject.SetActive(false);

                }
            }
            else
            {
                //playersNameText[i].gameObject.SetActive(false); -- only removing the player name and not the sliders form the battle ui when a character isnt actually joined in the party
                playerBattleStats[i].gameObject.SetActive(false);
            }
        }
    }

    //Player attacking methods

    public void PlayerAttack(string moveName, int selectEnemyTarget)
    {
        //int selectEnemyTarget = 3;
        int movePower = 0;

        for(int i = 0; i < battleMovesList.Length; i++)
        {
            if (battleMovesList[i].moveName == moveName)
            {
                movePower = GettingMovePowerAndEffectInstanstiation(selectEnemyTarget, i);
            }
        }

        InstantiateEffectOnAttackingCharacter();

        DealDamageToCharacters(selectEnemyTarget, movePower);

        NextTurn();

        enemyTargetPanel.SetActive(false);
    }

    public void OpenTargetMenu(string moveName)
    {
        enemyTargetPanel.SetActive(true);

        List<int> Enemies = new List<int>();
        for(int i = 0; i < activeCharacters.Count; i++)
        {
            if (!activeCharacters[i].IsPlayer())
            {
                Enemies.Add(i);
            }
        }

        //Debug.Log(Enemies.Count);

        for(int i = 0; i < targetButtons.Length; i++)
        {
            if(Enemies.Count > i)
            {
                targetButtons[i].gameObject.SetActive(true);
                targetButtons[i].moveName = moveName;
                targetButtons[i].activeBattleTarget = Enemies[i];
                targetButtons[i].targetName.text = activeCharacters[Enemies[i]].characterName;
            }
        }

    }

    private int GettingMovePowerAndEffectInstanstiation(int selectCharacterTarget, int i)
    {
        int movePower;
        Instantiate(
            battleMovesList[i].theEffectToUse,
            activeCharacters[selectCharacterTarget].transform.position,
            activeCharacters[selectCharacterTarget].transform.rotation
        );

        movePower = battleMovesList[i].movePower;
        return movePower;
    }

    public void OpenSkillPanel()
    {
        skillChoicePanel.SetActive(true);

        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (activeCharacters[currentTurn].AttackMovesAvailable().Length > i)
            {
                skillButtons[i].gameObject.SetActive(true);
                skillButtons[i].skillName = GetCurrentActiveCharacter().AttackMovesAvailable()[i];
                skillButtons[i].skillNameText.text = skillButtons[i].skillName;

                for(int j = 0; j < battleMovesList.Length; j++)
                {
                    if (battleMovesList[j].moveName == skillButtons[i].skillName)
                    {
                        skillButtons[i].skillCost = battleMovesList[j].staminaCost;
                        skillButtons[i].skillCostText.text = skillButtons[i].skillCost.ToString();

                        skillButtons[i].skillDesc = "" + battleMovesList[j].moveDescription;
                        skillButtons[i].skillDescText.text = skillButtons[i].skillDesc.ToString();

                    }
                }
            }
            else
            {
                skillButtons[i].gameObject.SetActive(false);
            }
        }

        
    }

    public BattleCharacters GetCurrentActiveCharacter()
    {
        return activeCharacters[currentTurn];
    }

    public void RunAway()
    {
        if (Random.value > chanceToRunAway)
        {
            isBattleActive = false;
            battleScene.SetActive(false);
        }
        else
        {
            NextTurn();
            battleNotice.SetText("You failed to run away!");
            battleNotice.Activate();

        }
    }

    public void UpdateItemsInInventory()
    {
        itemsToUseMenu.SetActive(true);

        foreach (Transform itemSlot in itemSlotContainerParent)
        {
            Destroy(itemSlot.gameObject);
        }

        foreach (ItemsManager item in Inventory.instance.GetItemsList())
        {
            RectTransform itemSlot = Instantiate(itemSlotContainer, itemSlotContainerParent).GetComponent<RectTransform>();

            Image itemImage = itemSlot.Find("Item Image").GetComponent<Image>(); //item image remember this when checking for the sprite of your items in inventory, if you change it, it wont work here
            itemImage.sprite = item.itemsImage;

            TextMeshProUGUI itemsAmountText = itemSlot.Find("Amount Text").GetComponent<TextMeshProUGUI>();
            if (item.amount > 1)
                itemsAmountText.text = item.amount.ToString();
            else
                itemsAmountText.text = "";

            itemSlot.GetComponent<ItemButton>().itemOnButton = item;
        }
    }

    public void selectedItemToUse(ItemsManager itemToUse)
    {
        selectedItem = itemToUse;
        itemName.text = itemToUse.itemName;
        itemDescription.text = itemToUse.itemDescription; 
    }

}