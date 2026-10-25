//using System;
using BattleUtils;
using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    [Header("Tutorial")]
    [SerializeField] private BattleTutorialManager tutorial;
    [SerializeField] private ItemData tutorialItem;

    [Header("Battle Trigger")]
    private BattleInstantiator currentBattleTrigger;
    [SerializeField] private GameObject battleScene;

    [Header("Characters")]
    [SerializeField] private BattleCharacters[] enemyCharacters;
    [SerializeField] private BattleCharacters[] playerCharacters;
    [SerializeField] private List<BattleCharacters> activeCharacters;

    [Header("Turn Management")]
    [SerializeField] private int _currentCharacterIndex;
    [SerializeField] private bool waitingForTurn;
    private bool canRun;

    [Header("Battle UI")]
    [SerializeField] private GameObject UIButtonHolder;
    [SerializeField] private CharacterStatsPanel _characterStatsPanel;

    [Header("Battle Moves")]
    [SerializeField] private BattleMoves[] battleMovesList;
    [SerializeField] private BattleSkillButton[] skillButtons;
    public GameObject skillChoicePanel;



    [Header("Enemy Targeting")]
    [SerializeField] private GameObject enemyTargetPanel;
    [SerializeField] private BattleTargetButtons[] targetButtons;

    [Header("Battle Effects")]
    [SerializeField] private ParticleSystem characterAttackEffect;
    [SerializeField] private CharacterDamageGUI damageText;



    [Header("Notifications")]
    public BattleNotification battleNotice;

    [Header("Escape")]
    [SerializeField] private float chanceToRunAway = 0.7f;
    private bool runningAway;

    [Header("Items")]
    public GameObject itemsToUseMenu;
    [SerializeField] private ItemData selectedItem;
    [SerializeField] private GameObject itemSlotContainer;
    [SerializeField] private Transform itemSlotContainerParent;
    [SerializeField] private TextMeshProUGUI itemName, itemDescription;

    [Header("Character Selection")]
    [SerializeField] private GameObject characterChoicePanel;
    [SerializeField] private TextMeshProUGUI[] playerNames;

    [Header("Rewards")]
    public List<BattleRewardData> _rewardsPool = new();

    [Header("Scene Management")]
    [SerializeField] private string gameOverScene;

    private PlayerData _playerData => PlayerDataHandler.Singleton.currentPlayerData;


    // --- TOP NOTIFICATION PANEL UI --- NOT YET IMPLEMENTED SEP 9 2026
    //[Header("Battle Notice UI")]
    //[SerializeField] private GameObject battleNoticePanel;
    //[SerializeField] private TextMeshProUGUI battleNoticeText;
    //private Coroutine noticeCoroutine;


    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning(
                "BattleManager: Duplicate Singleton destroyed on " +
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

    void Update()
    {
    }

    public void StartBattle(EnemyData[] enemiesToSpawn, bool canRunAway)
    {
        StartCoroutine(StartbattleRoutinee(enemiesToSpawn, canRunAway));
    }
    IEnumerator StartbattleRoutinee(EnemyData[] enemiesToSpawn, bool canRunAway)
    {
        yield return SceneTransition.instance.FadeToBlack();
        ResetBattleState();
        battleScene.SetActive(true);

        waitingForTurn = true;
        canRun = canRunAway;


        _rewardsPool.Clear();
        ResetCharacterValues();

        SetupPlayerCharacters(_playerData.characters);
        SetupEnemies(enemiesToSpawn);

        AddExistingCharacters();
        SortBattleCharactersBasedOnDexterity();

        _currentCharacterIndex = -1;
        NextTurn();

        yield return SceneTransition.instance.FadeFromBlack();
        TryTriggerTutorial();
    }
    private void TryTriggerTutorial()
    {
        if (PlayerDataHandler.Singleton.currentPlayerData.triggeredFlags.Contains("BattleTutorial"))
        {
            tutorial.gameObject.SetActive(false);
            return;
        }

        PlayerDataHandler.Singleton.currentPlayerData.triggeredFlags.Add("BattleTutorial");
        Inventory.Singleton.AddItem(tutorialItem);

        tutorial.gameObject.SetActive(true);
        tutorial.StartTutorial();
    }
    private void ResetCharacterValues()
    {
        activeCharacters.Clear();
        foreach (var character in playerCharacters)
        {
            character.ClearData();
        }
        foreach (var character in enemyCharacters)
        {
            character.ClearData();
        }
    }
    private void AddExistingCharacters()
    {
        foreach (var character in playerCharacters)
        {
            if (character.CharacterData != null)
                activeCharacters.Add(character);
        }
        foreach (var character in enemyCharacters)
        {
            if (character.CharacterData != null)
                activeCharacters.Add(character);
        }
    }
    public BattleCharacters GetCurrentActiveCharacter()
    {
        return activeCharacters[_currentCharacterIndex];
    }
    private void SortBattleCharactersBasedOnDexterity()
    {
        activeCharacters.Sort((a, b) => b.CharacterData.stats.dexterity.CompareTo(a.CharacterData.stats.dexterity));
    }

    private void SetupEnemies(EnemyData[] enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            int index = i;
            CharacterDataPreset preset = enemiesToSpawn[index].Enemy;
            BattleCharacters enemyCharacter = enemyCharacters[index];
            enemyCharacter.SetupCharacter(preset.GetCopyOfPreset(), false, preset.normalSprite, preset.deathSprite);
            enemyCharacter.SetupDeathAction(() => { AddToRewards(enemiesToSpawn[index].rewards); } );
        }
    }

    private void AddToRewards(BattleRewardData rewardData)
    {
        _rewardsPool.Add(rewardData);
    }
    private void SetupPlayerCharacters(List<Character> Characters)
    {
        int battlePlayerIndex = 0;
        foreach(var character in Characters)
        {
            Sprite normalSprite = CharacterReferenceConfig.Singleton.GetReference(character.characterName).battleSprite;
            Sprite downedSprite = CharacterReferenceConfig.Singleton.GetReference(character.characterName).downedSprite;
            playerCharacters[battlePlayerIndex].SetupCharacter(character, true, normalSprite, downedSprite);
            battlePlayerIndex++;
        }
        _characterStatsPanel.SetupCharacters(Characters);
    }

    private void NextTurn()
    {
        _currentCharacterIndex++;

        if (_currentCharacterIndex >= activeCharacters.Count)
            _currentCharacterIndex = 0;

        waitingForTurn = true;
        UpdatePlayerCharacterStatView();
        UpdateDeathStatus();

        if (IsGameOver())
        {
            return;
        }
        HandleTurnActions();
    }
    private void HandleTurnActions()
    {
        if (!waitingForTurn)
            return;


        if (GetCurrentActiveCharacter().isPlayer)
        {
            UIButtonHolder.SetActive(true);
            //Player chooses their move on the uibuttonHolder
        }
        else
        {
            UIButtonHolder.SetActive(false);
            StartCoroutine(EnemyMoveCoroutine());
        }
    }
    public void UpdatePlayerCharacterStatView()
    {
        _characterStatsPanel.UpdateValues();
    }

    private void UpdateDeathStatus()
    {
        for (int i = 0; i < activeCharacters.Count; i++)
        {
            if (activeCharacters[i].CharacterData.stats.currentHP <= 0 && !activeCharacters[i].isDead)
            {
                activeCharacters[i].KillCharacter();
            }
        }
    }
    private bool IsGameOver()
    {
        bool allEnemiesAreDead = activeCharacters.FindAll(x => !x.isPlayer && !x.isDead).Count == 0;
        bool allPlayersAreDead = activeCharacters.FindAll(x => x.isPlayer && !x.isDead).Count == 0;

        if (allEnemiesAreDead)
            StartCoroutine(EndBattleCoroutine());
        else if (allPlayersAreDead)
            StartCoroutine(GameOverCoroutine());

        return allPlayersAreDead || allEnemiesAreDead;
    }

    public void PlayerAttack(string moveName, BattleCharacters selectEnemyTarget)
    {
        Debug.Log("BM: Move: " + moveName);
        //int selectEnemyTarget = 3;
        int movePower = 0;

        for (int i = 0; i < battleMovesList.Length; i++)
        {
            if (battleMovesList[i].moveName == moveName)
            {
                movePower = GettingMovePowerAndEffectInstanstiation(selectEnemyTarget, i);
            }
        }
        enemyTargetPanel.SetActive(false);
        DealDamageToCharacters(GetCurrentActiveCharacter(), selectEnemyTarget, movePower);

        NextTurn();
    }

    public IEnumerator EnemyMoveCoroutine()
    {
        waitingForTurn = false;

        yield return new WaitForSeconds(1f);

        EnemyAttack();
        UpdatePlayerCharacterStatView();

        yield return new WaitForSeconds(1f);

        NextTurn();
    }
    private void EnemyAttack()
    {
        List<BattleCharacters> players = activeCharacters.FindAll(x => x.isPlayer && !x.isDead);

        BattleCharacters selectedPlayerToAttack = players[Random.Range(0, players.Count)];

        //TODO update grabbing moves
        List<string> availableAttacks = GetCurrentActiveCharacter().AttackMovesAvailable();

        if (availableAttacks == null ||
            availableAttacks.Count== 0)
        {
            Debug.LogError(
                "BattleManager: Current enemy has no available attacks."
            );

            return;
        }

        int selectedAttack = Random.Range(0, availableAttacks.Count);
        int movePower = 0;

        for (int i = 0; i < battleMovesList.Length; i++)
        {
            if (battleMovesList[i].moveName == availableAttacks[selectedAttack])
            {
                movePower = GettingMovePowerAndEffectInstanstiation(selectedPlayerToAttack,i);
            }
        }

        DealDamageToCharacters(GetCurrentActiveCharacter(), selectedPlayerToAttack, movePower);

    }
    private void DealDamageToCharacters(BattleCharacters attacker, BattleCharacters defender, int movePower)
    {
        Character attackerData = attacker.CharacterData;
        Character defenderData = defender.CharacterData;

        float attackPower = attackerData.stats.attack + attackerData.stats.weaponPower;
        float defenceAmount = defenderData.stats.defence + defenderData.stats.armorDefence;

        float flatDamage = attackPower - defenceAmount;
        flatDamage = flatDamage <= 0 ? 0 : flatDamage;

        float damageAmount = (flatDamage + movePower) * Random.Range(0.9f, 1.1f);
        int damageToGive = (int)damageAmount;

        damageToGive = CalculateCritical(damageToGive);
        Debug.Log($"attackPower: {attackPower} defenceAmount: {defenceAmount} damageAmount: {damageAmount}");

        defenderData.stats.ModifyHP(-damageToGive);
        defender.TakeDamage();

        DisplayAttackEffect(defender.transform);
        DisplayDamageText(defender.transform, damageToGive);
    }
    private void DisplayDamageText(Transform position, int amount)
    {
        CharacterDamageGUI characterDamageText = Instantiate(damageText, position.transform.position, Quaternion.identity);
        characterDamageText.SetDamage(amount);
    }
    private void DisplayAttackEffect(Transform position)
    {
        Instantiate(characterAttackEffect,position.transform.position, Quaternion.identity );
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
    private int GettingMovePowerAndEffectInstanstiation(BattleCharacters characterToAttack, int i)
    {
        int movePower = battleMovesList[i].movePower;
        Instantiate(battleMovesList[i].theEffectToUse, characterToAttack.transform.position, Quaternion.identity);

        return movePower;
    }

    public void OpenTargetMenu(string moveName)
    {
        enemyTargetPanel.SetActive(true);

        List<BattleCharacters> EnemyTarget = activeCharacters.ToList().FindAll(x => !x.isPlayer && !x.isDead );
        
        foreach(var btn in targetButtons)
        {
            btn.gameObject.SetActive(false);
        }
        int i = 0;
        foreach( var btn in targetButtons)
        {
            if (i == EnemyTarget.Count)
                break;
            targetButtons[i].gameObject.SetActive(true);
            targetButtons[i].moveName = moveName;
            targetButtons[i].activeBattleTarget = EnemyTarget[i];
            targetButtons[i].targetName.text = EnemyTarget[i].CharacterData.characterName;
            i++;
        }
    }
    public void OpenSkillPanel()
    {
        skillChoicePanel.SetActive(true);

        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (GetCurrentActiveCharacter().AttackMovesAvailable().Count > i)
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
        if (Inventory.Singleton == null)
        {
            Debug.LogError(
                "BattleManager: Inventory Singleton is missing."
            );

            return;
        }

        if (itemsToUseMenu != null)
        {
            itemsToUseMenu.SetActive(true);
        }

        foreach (Transform itemSlot in itemSlotContainerParent)
        {
            Destroy(itemSlot.gameObject);
        }

        foreach (ItemData item in Inventory.Singleton.GetItemsList())
        {
            if (item == null ||
                item.itemType != ItemData.ItemType.Item)
            {
                continue;
            }

            RectTransform itemSlot =
                Instantiate(
                    itemSlotContainer,
                    itemSlotContainerParent
                ).GetComponent<RectTransform>();

            Image itemImage =
                itemSlot.Find("Item Image").GetComponent<Image>();

            itemImage.sprite = item.itemsImage;

            TextMeshProUGUI amountText =
                itemSlot.Find("Amount Text")
                    .GetComponent<TextMeshProUGUI>();

            amountText.text =
                item.amount > 1
                    ? item.amount.ToString()
                    : string.Empty;

            itemSlot.GetComponent<ItemButton>().itemOnButton = item;
        }
    }

    private static Transform ResolveItemContentParent(Transform configuredParent)
    {
        if (configuredParent == null)
        {
            return null;
        }

        Transform[] descendants =
            configuredParent.GetComponentsInChildren<Transform>(true);

        foreach (Transform descendant in descendants)
        {
            if (descendant != configuredParent &&
                descendant.name.IndexOf(
                    "Content",
                    System.StringComparison.OrdinalIgnoreCase
                ) >= 0)
            {
                return descendant;
            }
        }

        return configuredParent;
    }

    public void SelectItem(ItemData itemToUse)
    {
        // Prevent selecting equipment for battle use
        if (itemToUse.itemType != ItemData.ItemType.Item)
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
    private void ResetItemSelection()
    {
        selectedItem = null;
        itemName.text = "";
        itemDescription.text = "";
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

        for (int i = 0; i < playerNames.Length; i++)
        {
            if (playerNames[i] != null && playerNames[i].transform.parent != null)
                playerNames[i].transform.parent.gameObject.SetActive(false);
        }

        int playerButtonIndex = 0;

        foreach (BattleCharacters character in playerCharacters)
        {
            if (character.CharacterData == null)
                return;

            if (playerButtonIndex >= playerNames.Length)
                break;

            playerNames[playerButtonIndex].text = character.CharacterData.characterName;
            playerNames[playerButtonIndex].transform.parent.gameObject.SetActive(true);

            playerButtonIndex++;
        }
    }

    public void UseItemButton(int selectedPlayerIndex)
    {
        if (selectedItem == null)
        {
            Debug.LogWarning("UseItemButton: no item selected.");
            return;
        }

        playerCharacters[selectedPlayerIndex].CharacterData.stats.UseItemOnCharacter(selectedItem);
        Inventory.Singleton.RemoveItem(selectedItem);

        CloseCharacterChoice();

        selectedItem = null;
        if (UIButtonHolder != null) UIButtonHolder.SetActive(false);

        NextTurn();
    }

    public void CloseCharacterChoice()
    {
        characterChoicePanel.SetActive(false);
        itemsToUseMenu.SetActive(false);
        ResetItemSelection();
    }

    public IEnumerator EndBattleCoroutine()
    {
        UIButtonHolder.SetActive(false);
        enemyTargetPanel.SetActive(false);
        skillChoicePanel.SetActive(false);

        if (!runningAway)
        {
            BattleRewardsHandler.instance.PrepareRewardScreen(_rewardsPool);
            battleNotice.SetText("You won!");
            battleNotice.Activate();

            yield return StartCoroutine(battleNotice.WaitUntilFinished());

            if (!runningAway && currentBattleTrigger != null)
            {
                currentBattleTrigger.MarkEnemyDefeated();
                currentBattleTrigger = null;
            }
        }

        yield return (SceneTransition.instance.FadeToBlack());

        battleScene.SetActive(false);
        activeCharacters.Clear();

        AudioManager.instance.StopBattleMusic();

        if (runningAway)
        {
            if (currentBattleTrigger != null)
            {
                currentBattleTrigger.MoveEnemyAwayFromPlayer();
                currentBattleTrigger = null;
            }

            runningAway = false;

            if (SceneTransition.instance != null)
            {
                yield return SceneTransition.instance.FadeFromBlack();
            }
        }

        else
        {
            BattleRewardsHandler.instance.DisplayMessags();
            // Let the rewards panel activate while the screen is black.
            yield return null;
            yield return new WaitForEndOfFrame();

            if (SceneTransition.instance != null)
            {
                yield return (SceneTransition.instance.FadeFromBlack());
            }

            BattleRewardsHandler.instance.PlayRewardSFX();
        }

        _currentCharacterIndex = 0;


        GameManager.Singleton.SetGameState(GameState.Roaming);
    }

    public IEnumerator GameOverCoroutine()
    {
        GameManager.Singleton.SetPlayState(PlayState.GameOver);
        battleNotice.SetText("Game Over!");
        battleNotice.Activate();


        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(gameOverScene);
    }

    public void SetBattleTrigger(BattleInstantiator battleTrigger)
    {
        currentBattleTrigger = battleTrigger;
    }

    public void ResetBattleState()
    {
        activeCharacters.Clear();
        
        waitingForTurn = false;
        _currentCharacterIndex = 0;
        runningAway = false;

        battleScene.SetActive(false);
        UIButtonHolder.SetActive(false);
        enemyTargetPanel.SetActive(false);
        skillChoicePanel.SetActive(false);

        foreach (var character in playerCharacters)
        {
            character.gameObject.SetActive(false);
        }
        foreach (var character in enemyCharacters)
        {
            character.gameObject.SetActive(false);
        }
    }
}