//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    private bool isBattleActive;

    private BattleInstantiator currentBattleTrigger;

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

    [SerializeField] GameObject characterChoicePanel;
    [SerializeField] TextMeshProUGUI[] playerNames;

    [SerializeField] string gameOverScene;

    private bool runningAway;
    public int xpRewardAmount;
    public ItemsManager[] itemsReward;

    public int scrapsReward;

    private bool canRun;

    // --- TOP NOTIFICATION PANEL UI --- NOT YET IMPLEMENTED SEP 9 2026
    //[Header("Battle Notice UI")]
    //[SerializeField] GameObject battleNoticePanel;
    //[SerializeField] TextMeshProUGUI battleNoticeText;
    //private Coroutine noticeCoroutine;


    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning(
                "BattleManager: Duplicate instance destroyed on " +
                gameObject.name,
                this
            );

            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartBattle(new string[] { "Robbie", "Aaronos", "Robbie" }, true);
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

    public void StartBattle(string[] enemiesToSpawn, bool canRunAway)
    {

        if (!isBattleActive)
        {
            canRun = canRunAway;
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
        if (GameManager.instance == null)
        {
            Debug.LogError(
                "BattleManager: GameManager instance is missing."
            );

            return;
        }

        PlayerStats[] savedPlayerStats =
            GameManager.instance.GetPlayerStats();

        if (savedPlayerStats == null ||
            savedPlayerStats.Length == 0)
        {
            Debug.LogError(
                "BattleManager: No persistent PlayerStats were found."
            );

            return;
        }

        int battlePlayerIndex = 0;

        foreach (PlayerStats playerStats in savedPlayerStats)
        {
            if (playerStats == null ||
                !playerStats.gameObject.activeSelf)
            {
                continue;
            }

            if (battlePlayerIndex >= playerPositions.Length)
            {
                Debug.LogError(
                    "BattleManager: Not enough player positions."
                );

                break;
            }

            for (int i = 0; i < playerPrefabs.Length; i++)
            {
                if (playerPrefabs[i] == null ||
                    playerPrefabs[i].characterName !=
                    playerStats.playerName)
                {
                    continue;
                }

                BattleCharacters newPlayer = Instantiate(
                    playerPrefabs[i],
                    playerPositions[battlePlayerIndex].position,
                    playerPositions[battlePlayerIndex].rotation,
                    playerPositions[battlePlayerIndex]
                );

                activeCharacters.Add(newPlayer);

                ImportPlayerStats(
                    activeCharacters.Count - 1,
                    playerStats
                );

                battlePlayerIndex++;
                break;
            }
        }

        if (battlePlayerIndex == 0)
        {
            Debug.LogError(
                "BattleManager: No active players were registered for battle."
            );
        }
    }

    private void ImportPlayerStats(
    int battlePlayerIndex,
    PlayerStats player)
    {
        BattleCharacters battleCharacter =
            activeCharacters[battlePlayerIndex];

        battleCharacter.currentHP = player.currentHP;
        battleCharacter.maxHP = player.maxHP;
        battleCharacter.currentSP = player.currentStamina;
        battleCharacter.maxSP = player.maxStamina;
        battleCharacter.dexterity = player.dexterity;
        battleCharacter.defence = player.defence;
        battleCharacter.wpnPower = player.weaponPower;
        battleCharacter.armorDefence = player.armorDefence;
    }

    private void SettingUpBattle()
    {
        isBattleActive = true;
        GameManager.instance.battleIsActive = true;

        transform.position = Vector3.zero;

        if (battleScene != null)
        {
            battleScene.transform.position = Vector3.zero;
            battleScene.transform.rotation = Quaternion.identity;
            battleScene.SetActive(true);
        }
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
                if (activeCharacters[i].IsPlayer() && !activeCharacters[i].isDead)
                {
                    activeCharacters[i].KillPlayer();
                }

                if(!activeCharacters[i].IsPlayer() && !activeCharacters[i].isDead)
                {
                    activeCharacters[i].KillEnemy();
                }

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
                StartCoroutine(EndBattleCoroutine()); //print("Players won!");
            else if (allPlayerAreDead)
                StartCoroutine(GameOverCoroutine());  //print("You lost");

            //battleScene.SetActive(false);
            //GameManager.instance.battleIsActive = false;
            //isBattleActive = false;
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

        if (!isBattleActive || activeCharacters.Count == 0 ||
            currentTurn < 0 || currentTurn >= activeCharacters.Count ||
            activeCharacters[currentTurn] == null ||
            activeCharacters[currentTurn].currentHP <= 0 ||
            activeCharacters[currentTurn].IsPlayer())
        {
            yield break;
        }

        EnemyAttack();

        yield return new WaitForSeconds(1.5f);

        if (!isBattleActive || activeCharacters.Count == 0)
        {
            yield break;
        }

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

        if (players.Count == 0)
        {
            Debug.LogError(
                "BattleManager: EnemyAttack cancelled because no player characters are active."
            );

            isBattleActive = false;

            if (GameManager.instance != null)
            {
                GameManager.instance.battleIsActive = false;
            }

            return;
        }

        int selectedPlayerToAttack =
            players[Random.Range(0, players.Count)];

        string[] availableAttacks =
            activeCharacters[currentTurn].AttackMovesAvailable();

        if (availableAttacks == null ||
            availableAttacks.Length == 0)
        {
            Debug.LogError(
                "BattleManager: Current enemy has no available attacks."
            );

            return;
        }

        int selectedAttack =
            Random.Range(0, availableAttacks.Length);
        int movePower = 0;

        for (int i = 0; i < battleMovesList.Length; i++)
        {
            if (battleMovesList[i].moveName == availableAttacks[selectedAttack])
            {
                movePower = GettingMovePowerAndEffectInstanstiation(selectedPlayerToAttack,i);
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
            if(Enemies.Count > i && activeCharacters[Enemies[i]].currentHP > 0)
            {
                targetButtons[i].gameObject.SetActive(true);
                targetButtons[i].moveName = moveName;
                targetButtons[i].activeBattleTarget = Enemies[i];
                targetButtons[i].targetName.text = activeCharacters[Enemies[i]].characterName;
            }
            else
            {
                targetButtons[i].gameObject.SetActive(false);
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
        if (canRun)
        {
            if (Random.value > chanceToRunAway)
            {
                //isBattleActive = false;
                //battleScene.SetActive(false);
                runningAway = true;
                StartCoroutine(EndBattleCoroutine());
            }
            else
            {
                NextTurn();
                battleNotice.SetText("You failed to run away!");
                battleNotice.Activate();

            }
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
            //checks if its a weapon or armor, if it is, it wont show up in the items to use menu since you cant use those in battle
            if (item.itemType != ItemsManager.ItemType.Item)
                continue;

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
        // Prevent selecting equipment for battle use
        if (itemToUse.itemType != ItemsManager.ItemType.Item)
        {
            Debug.Log("Item is equipment, cannot be used in battle.");
            return;
        }

        // If the currently open character choice panel is visible and the player
        // selected a different item, close the character choice panel so it won't
        // remain open for the newly selected item.
        if (selectedItem != null && selectedItem != itemToUse && characterChoicePanel != null && characterChoicePanel.activeSelf)
        {
            characterChoicePanel.SetActive(false);
        }

        selectedItem = itemToUse;
        itemName.text = itemToUse.itemName;
        itemDescription.text = itemToUse.itemDescription;
    }

    public void OpenCharacterMenu()
    {
        if (selectedItem == null)
        {
            Debug.LogWarning(
                "BattleManager: No item has been selected."
            );

            return;
        }

        characterChoicePanel.SetActive(true);

        // Hide every character option first.
        for (int i = 0; i < playerNames.Length; i++)
        {
            if (playerNames[i] != null &&
                playerNames[i].transform.parent != null)
            {
                playerNames[i].transform.parent.gameObject.SetActive(false);
            }
        }

        int playerButtonIndex = 0;

        // Only display players that were actually added to this battle.
        foreach (BattleCharacters character in activeCharacters)
        {
            if (!character.IsPlayer())
            {
                continue;
            }

            if (playerButtonIndex >= playerNames.Length)
            {
                break;
            }

            playerNames[playerButtonIndex].text =
                character.characterName;

            playerNames[playerButtonIndex]
                .transform
                .parent
                .gameObject
                .SetActive(true);

            playerButtonIndex++;
        }
    }

    public void UseItemButton(int selectedPlayer)
    {
        // Validate selection and that it's the current player's turn
        if (selectedItem == null)
        {
            Debug.LogWarning("UseItemButton: no item selected.");
            return;
        }

        // Apply item to the selected player
        activeCharacters[selectedPlayer].UseItemInBattle(selectedItem);
        Inventory.instance.RemoveItem(selectedItem);

        UpdatePlayerStats();

        // Close UI panels (hides itemsToUseMenu and characterChoicePanel)
        CloseCharacterChoice();

        // Hide player buttons immediately so player can't act again before turn advances
        if (UIButtonHolder != null) UIButtonHolder.SetActive(false);

        // Using an item consumes the player's turn — advance to next character
        NextTurn();
    }

    public void CloseCharacterChoice()
    {
        characterChoicePanel.SetActive(false);
        itemsToUseMenu.SetActive(false);
    }

    public IEnumerator EndBattleCoroutine()
    {
        isBattleActive = false;
        GameManager.instance.battleIsActive = false;
        UIButtonHolder.SetActive(false);
        enemyTargetPanel.SetActive(false);
        skillChoicePanel.SetActive(false);
        yield return new WaitForSeconds(4f);

        if (!runningAway)
        {
            battleNotice.SetText("You won!");
            battleNotice.Activate();

            // Wait for the notification to finish completely before
            // starting the screen transition to the rewards.
            yield return StartCoroutine(battleNotice.WaitUntilFinished());

            if (!runningAway && currentBattleTrigger != null)
            {
                currentBattleTrigger.MarkEnemyDefeated();
                currentBattleTrigger = null;
            }
        }

        // Fade to black only after the battle notification has finished.
        if (SceneTransition.instance != null)
        {
            yield return StartCoroutine(SceneTransition.instance.FadeToBlack());
        }

        foreach (BattleCharacters playerInBattle in activeCharacters)
        {
            if (playerInBattle.IsPlayer())
            {
                foreach (PlayerStats playerWishStats in GameManager.instance.GetPlayerStats())
                {
                    if (playerInBattle.characterName == playerWishStats.playerName)
                    {
                        playerWishStats.currentHP = playerInBattle.currentHP;
                        playerWishStats.currentStamina = playerInBattle.currentSP;
                    }
                }
            }

            Destroy(playerInBattle.gameObject);
        }

        battleScene.SetActive(false);
        activeCharacters.Clear();

        //if (!runningAway && overworldEnemy != null)
        //{
        //    Destroy(overworldEnemy);
        //    overworldEnemy = null;
        //}

        if (!runningAway && currentBattleTrigger != null)
        {
            currentBattleTrigger.MarkEnemyDefeated();
            currentBattleTrigger = null;
        }


        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopBattleMusic();
        }

        if (runningAway)
        {
            if (currentBattleTrigger != null)
            {
                currentBattleTrigger.MoveEnemyAwayFromPlayer();
                currentBattleTrigger = null;
            }

            if (Player.instance != null)
            {
                Player.instance.ResumeMovement();
            }

            GameManager.instance.battleIsActive = false;
            runningAway = false;

            if (SceneTransition.instance != null)
            {
                yield return StartCoroutine(
                    SceneTransition.instance.FadeFromBlack()
                );
            }
        }

        else
        {
            BattleRewardsHandler.instance.PrepareRewardScreen(
                xpRewardAmount,
                itemsReward,
                scrapsReward
            );

            // Let the rewards panel activate while the screen is black.
            yield return null;
            yield return new WaitForEndOfFrame();

            if (SceneTransition.instance != null)
            {
                yield return StartCoroutine(SceneTransition.instance.FadeFromBlack());
            }

            BattleRewardsHandler.instance.PlayRewardSFX();
        }

        currentTurn = 0;

        

    }

    public IEnumerator GameOverCoroutine()
    {
        battleNotice.SetText("Game Over!");
        battleNotice.Activate();

        isBattleActive = false;

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(gameOverScene);
    }

    public void SetBattleTrigger(BattleInstantiator battleTrigger)
    {
        currentBattleTrigger = battleTrigger;
    }

    public void ResetBattleState()
    {
        StopAllCoroutines();

        foreach (BattleCharacters character in activeCharacters)
        {
            if (character != null)
            {
                Destroy(character.gameObject);
            }
        }

        activeCharacters.Clear();
        isBattleActive = false;
        waitingForTurn = false;
        currentTurn = 0;
        currentBattleTrigger = null;
        runningAway = false;

        if (battleScene != null)
        {
            battleScene.SetActive(false);
        }

        if (UIButtonHolder != null) UIButtonHolder.SetActive(false);
        if (enemyTargetPanel != null) enemyTargetPanel.SetActive(false);
        if (skillChoicePanel != null) skillChoicePanel.SetActive(false);
    }

}