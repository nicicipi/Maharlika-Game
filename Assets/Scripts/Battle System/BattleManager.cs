using BattleSystem;
using BattleUtils;
using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static GameManager;

public class BattleManager : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds3 = new WaitForSeconds(3f);
    public static BattleManager instance;

    //[Header("Tutorial")]
    //[SerializeField] private BattleTutorialManager tutorial;
    //[SerializeField] private ItemData tutorialItem;

    //[Header("Battle Trigger")]
    private BattleInstantiator _currentBattleTrigger;
    //[SerializeField] private GameObject battleScene;

    //[Header("Characters")]
    //[SerializeField] private BattleCharacters[] enemyCharacters;
    //[SerializeField] private BattleCharacters[] playerCharacters;
    //[SerializeField] private List<BattleCharacters> activeCharacters;

    //[Header("Turn Management")]
    //[SerializeField] private int _currentCharacterIndex;
    //[SerializeField] private bool waitingForTurn;
    //private bool canRun;

    //[Header("Battle UI")]
    //[SerializeField] private GameObject UIButtonHolder;
    //[SerializeField] private CharacterStatsPanel _characterStatsPanel;

    //[Header("Battle Moves")]
    //[SerializeField] private BattleMoves[] battleMovesList;
    //[SerializeField] private BattleSkillButton[] skillButtons;
    //public GameObject skillChoicePanel;



    //[Header("Enemy Targeting")]
    //[SerializeField] private GameObject enemyTargetPanel;
    //[SerializeField] private BattleTargetButtons[] targetButtons;

    //[Header("Battle Effects")]
    //[SerializeField] private ParticleSystem characterAttackEffect;
    //[SerializeField] private CharacterDamageGUI damageText;


    //[Header("Escape")]
    //[SerializeField] private float chanceToRunAway = 0.7f;
    //private bool runningAway;

    //[Header("Items")]
    //public GameObject itemsToUseMenu;
    //[SerializeField] private ItemData selectedItem;
    //[SerializeField] private GameObject itemSlotContainer;
    //[SerializeField] private Transform itemSlotContainerParent;
    //[SerializeField] private TextMeshProUGUI itemName, itemDescription;

    //[Header("Character Selection")]
    //[SerializeField] private GameObject characterChoicePanel;
    //[SerializeField] private TextMeshProUGUI[] playerNames;

    //[Header("Rewards")]
    //public List<BattleRewardData> _rewardsPool = new();

    //[Header("Scene Management")]
    //[SerializeField] private string gameOverScene;

    private PlayerData _playerData => PlayerDataHandler.Singleton.currentPlayerData;


    // --- TOP NOTIFICATION PANEL UI --- NOT YET IMPLEMENTED SEP 9 2026
    //[Header("Battle Notice UI")]
    //[SerializeField] private GameObject battleNoticePanel;
    //[SerializeField] private TextMeshProUGUI battleNoticeText;
    //private Coroutine noticeCoroutine;
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

    #region Refactor
    public enum BattleState
    {
        Starting,
        Active,
        Victory,
        Defeat,
        Inactive
    }
    public enum UIState
    {
        None = 0,
        ChoiceSelection = 1,
        // choices:
        AttackSelection = 2,
        SkillSelection = 3,
        ItemSelection = 4,
        RunAttempt = 5
    }
    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn,
    }
    [Header("Battle Moves")]
    public BattleMoveLibrary battleMoveLibrary;
    [Header("Battle UI")]
    public GameObject battleScene;
    public GameObject UI_Choices;
    public SkillChoicePanel UI_Skills;
    public InventoryPanel UI_Items;
    public CharacterTargetPanel UI_BasicAttackTargets;
    [SerializeField] private CharacterStatsPanel _characterStatsPanel;
    [SerializeField] private List<BattleCharacters> _battleCharacterViewPlayers = new();
    [SerializeField] private List<BattleCharacters> _battleCharacterViewEnemies = new();
    public BattleNotification battleNotice;

    [Header("Battle Settings")]
    private BattleState _currentBattleState;
    private UIState _currentUIState;
    private TurnState _currentTurnState;
    private List<BattleCharacters> _activeBattleCharacters = new();
    private BattleCharacters _currentPlayerCharacter;
    private bool _waitingForPlayerInput = true;
    private Coroutine _currentPlayerRoutine;
    #region StateUpdates

    public void SetUIState(int index)
    {
        UIState state = index switch
        {
            0 => UIState.None,
            1 => UIState.ChoiceSelection,
            2 => UIState.AttackSelection,
            3 => UIState.SkillSelection,
            4 => UIState.ItemSelection,
            5 => UIState.RunAttempt,
            _ => throw new System.NotImplementedException()
        };
        SetUIState(state);
    }
    public void SetUIState(UIState state)
    {
        _currentUIState = state;
        UI_Choices.SetActive(false);
        UI_Skills.gameObject.SetActive(false);
        UI_Items.gameObject.SetActive(false);
        UI_BasicAttackTargets.gameObject.SetActive(false);
        switch (state)
        {
            case UIState.None:
                break;
            case UIState.ChoiceSelection:
                UI_Choices.SetActive(true);
                break;
            case UIState.AttackSelection:
                StartCoroutine(DefaultAttackRoutine());
                break;
            case UIState.SkillSelection:
                // opens up skill selection, and then target selection, once confirmed- it will trigger end of player input.
                UI_Skills.SetupChoices(_currentPlayerCharacter, OnPlayerSelectSkill);
                break;
            case UIState.ItemSelection:
                UI_Items.SetupInventory(Inventory.Singleton.GetItemsList());
                break;
            case UIState.RunAttempt:
                break;
        }
    }

    private void SetTurnState(TurnState state)
    {
        _currentTurnState = state;
        switch (state)
        {
            case TurnState.PlayerTurn:
                SetUIState(UIState.ChoiceSelection);
                break;
            case TurnState.EnemyTurn:
                SetUIState(UIState.None);
                break;
        }
    }
    #endregion

    public void SetupBattle(List<EnemyData> Enemies, bool canRunAway, List<Character> PlayerCharacters = null, BattleInstantiator currentBattleTrigger = null)
    {
        GameManager.Singleton.SetGameState(GameState.Battle);
        _currentBattleTrigger = currentBattleTrigger; ;
        PlayerCharacters ??= _playerData.characters;
        SetupBattleCharacters(PlayerCharacters, Enemies);
        StartCoroutine(BattleRoutine());
    }

    private void SetupBattleCharacters(List<Character> PlayerCharacters, List<EnemyData> Enemies)
    {
        foreach (var character in _battleCharacterViewEnemies)
            character.gameObject.SetActive(false);
        
        foreach (var character in _battleCharacterViewPlayers)
            character.gameObject.SetActive(false);

        int battlePlayerIndex = 0;
        int battleEnemyIndex = 0;
        foreach (var character in PlayerCharacters)
        {
            Sprite normalSprite = CharacterReferenceConfig.Singleton.GetReference(character.characterName).battleSprite;
            Sprite downedSprite = CharacterReferenceConfig.Singleton.GetReference(character.characterName).downedSprite;
            _battleCharacterViewPlayers[battlePlayerIndex].SetupCharacter(character, battleMoveLibrary.GetBattleMoves(character.moveSets), true, normalSprite, downedSprite);
            _activeBattleCharacters.Add(_battleCharacterViewPlayers[battlePlayerIndex]);
            battlePlayerIndex++;
        }
        _characterStatsPanel.SetupCharacters(PlayerCharacters);

        foreach (var character in PlayerCharacters)
        {
            CharacterDataPreset preset = Enemies[battleEnemyIndex].Enemy;
            _battleCharacterViewEnemies[battleEnemyIndex].SetupCharacter(preset.GetCopyOfPreset(), Enemies[battleEnemyIndex].Enemy.battleMoves, false, preset.normalSprite, preset.deathSprite);
            _activeBattleCharacters.Add(_battleCharacterViewEnemies[battleEnemyIndex]);
            battleEnemyIndex++;
        }

        //enemyCharacter.SetupDeathAction(() => { AddToRewards(enemiesToSpawn[index].rewards); });

    }
    #endregion

    #region Actions
    private IEnumerator BattleRoutine()
    {
        _currentBattleState = BattleState.Starting;
        battleScene.SetActive(true);
        yield return SceneTransition.instance.FadeFromBlack();

        _currentBattleState = BattleState.Active;

        while (true)
        {
            yield return PlayerInputRoutine(); //wait for player input
            
            yield return EnemyInputRoutine(); //wait for enemy AI to 

            if (GameEndCheck())
                break;
        }
        BattleEnd();
    }

    private IEnumerator PlayerInputRoutine()
    {
        SetTurnState(TurnState.PlayerTurn);
        foreach(var player in _activeBattleCharacters)
        {
            if (!player.isPlayer)
                continue;
            SetUIState(UIState.ChoiceSelection);
            _waitingForPlayerInput = true;
            _currentPlayerCharacter = player;
            yield return new WaitUntil(() => !_waitingForPlayerInput);

            if (GameEndCheck())
                break;
        }
    }
    public void TriggerPlayerInputEnd()
    {
        _waitingForPlayerInput = false;
    }

    private void OnPlayerSelectSkill(BattleMoves move, List<BattleCharacters> targets)
    {
        if(_currentPlayerRoutine != null)
        {
            Debug.Log("Player Skill routine is in progress");
            return; 
        }    
        _currentPlayerRoutine = StartCoroutine(PlayerSkillRoutine(move, targets));
    }
    private IEnumerator PlayerSkillRoutine(BattleMoves move, List<BattleCharacters> targets)
    {
        SetUIState(UIState.None);
        yield return ExecuteMove(_currentPlayerCharacter, move, targets);
        TriggerPlayerInputEnd();
        _currentPlayerRoutine = null;
    }

    private IEnumerator DefaultAttackRoutine()
    {
        BattleCharacters target = null;
        bool selected = false;

        UI_BasicAttackTargets.SetupTargets(GetAllEnemies(), DefaultAttack);

        while (!selected)
            yield return null;

        yield return ExecuteMove(_currentPlayerCharacter, battleMoveLibrary.defaultMove, target);
        TriggerPlayerInputEnd();

        void DefaultAttack(BattleCharacters selectedTarget)
        {
            target = selectedTarget;
            selected = true;
        }
    }
    private IEnumerator EnemyInputRoutine()
    {
        SetTurnState(TurnState.EnemyTurn);

        // for each alive enemy character, execute AI
        foreach (var enemy in _activeBattleCharacters)
        {
            if (enemy.isDead || enemy.isPlayer)
                continue;
            if (GameEndCheck())
                break;
            // Execute AI for this enemy
            yield return ExecuteSimpleEnemyAI(enemy);
        }
    }
    private IEnumerator ExecuteSimpleEnemyAI(BattleCharacters enemy)
    {
        var alivePlayers = _activeBattleCharacters.Where(p => !p.isDead && p.isPlayer).ToList();
        var target = alivePlayers[Random.Range(0, alivePlayers.Count)];

        var availableSkills = enemy.CharacterData.moveSets;
        if (enemy.CharacterData.moveSets.Count == 0)
            Debug.LogError("Enemy Movesets missing");
        var selectedSkillName = availableSkills[Random.Range(0, availableSkills.Count)];
        var selectedSkill = battleMoveLibrary.GetBattleMoveByName(selectedSkillName);
        yield return ExecuteMove(enemy, selectedSkill, GetTargettableCharacters(enemy, selectedSkill.targetSelection));
    }
    public IEnumerable ExecuteMove(BattleCharacters source, BattleMoves battleMove, BattleCharacters target)
    {
        foreach (var battleAction in battleMove.actions)
        {
            // Stats
            battleAction.action.ApplyActionToTarget(source, target);

            // VFX
            target.ApplyVFX(battleAction.action.actionVFX);
            yield return new WaitForSeconds(battleAction.action.actionDuration);
        }
        yield return new WaitForSeconds(battleMove.moveDuration);
    }
    public IEnumerable ExecuteMove(BattleCharacters source, BattleMoves battleMove, List<BattleCharacters> targets)
    {
        foreach (var target in targets)
        {
            foreach(var battleAction in battleMove.actions) 
            {
                // Stats
                battleAction.action.ApplyActionToTarget(source, target);
                
                // VFX
                target.ApplyVFX(battleAction.action.actionVFX);
                yield return new WaitForSeconds(battleAction.action.actionDuration);
            }
        }
        yield return new WaitForSeconds(battleMove.moveDuration);
    }
    private bool GameEndCheck()
    {
        bool allEnemiesAreDead = _activeBattleCharacters.FindAll(x => !x.isPlayer && !x.isDead).Count == 0;
        bool allPlayersAreDead = _activeBattleCharacters.FindAll(x => x.isPlayer && !x.isDead).Count == 0;
        return allEnemiesAreDead || allPlayersAreDead;
    }
    private void BattleEnd()
    {
        bool allEnemiesAreDead = _activeBattleCharacters.FindAll(x => !x.isPlayer && !x.isDead).Count == 0;
        bool allPlayersAreDead = _activeBattleCharacters.FindAll(x => x.isPlayer && !x.isDead).Count == 0;

        if (allEnemiesAreDead)
            StartCoroutine(EndBattleCoroutine());
        else if (allPlayersAreDead)
            StartCoroutine(GameOverCoroutine());
    }
    private IEnumerator EndBattleCoroutine()
    {
        battleScene.SetActive(false);
        _activeBattleCharacters.Clear();
        AudioManager.instance.StopBattleMusic();
        BattleRewardsHandler.instance.DisplayMessags();
        if (_currentBattleTrigger != null)
        {
            _currentBattleTrigger.MarkEnemyDefeated();
            _currentBattleTrigger = null;
        }
        yield return SceneTransition.instance.FadeFromBlack();

        GameManager.Singleton.SetGameState(GameState.Roaming);
    }
    public IEnumerator GameOverCoroutine()
    {
        GameManager.Singleton.SetPlayState(PlayState.GameOver);
        battleNotice.SetText("Game Over!");
        battleNotice.Activate();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameOverScene");
    }
    #endregion

    #region Helpers
    public List<BattleCharacters> GetAllAllies()
    {
        return _activeBattleCharacters.Where(c => c.isPlayer && !c.isDead).ToList();
    }
    public List<BattleCharacters> GetAllEnemies()
    {
        return GetTargettableCharacters(_currentPlayerCharacter, TargetSelection.SingleEnemy);
    }
    public List<BattleCharacters> GetTargettableCharacters(BattleCharacters source, TargetSelection targetSelection)
    {
        List<BattleCharacters> targets = new();
        
        List<BattleCharacters> allytargets = source.isPlayer ? _battleCharacterViewPlayers : _battleCharacterViewEnemies;
        List<BattleCharacters> enemytargets = source.isPlayer ? _battleCharacterViewEnemies : _battleCharacterViewPlayers;

        allytargets.RemoveAll((x) => { return !_activeBattleCharacters.Contains(x); });
        enemytargets.RemoveAll((x) => { return !_activeBattleCharacters.Contains(x); });

        switch (targetSelection)
        {
            case TargetSelection.Self:
                targets.Add(source);
                break;
            case TargetSelection.SingleEnemy:
                var aliveEnemies = enemytargets.Where(e => !e.isDead).ToList();
                if (aliveEnemies.Count > 0)
                    targets.Add(aliveEnemies[Random.Range(0, aliveEnemies.Count)]);
                break;
            case TargetSelection.RandomEnemy:
                var aliveEnemiesRandom = enemytargets.Where(e => !e.isDead).ToList();
                if (aliveEnemiesRandom.Count > 0)
                    targets.Add(aliveEnemiesRandom[Random.Range(0, aliveEnemiesRandom.Count)]);
                break;
            case TargetSelection.AllEnemies:
                targets.AddRange(enemytargets.Where(e => !e.isDead));
                break;
            case TargetSelection.SingleAlly:
                var aliveAllies = allytargets.Where(p => !p.isDead).ToList();
                if (aliveAllies.Count > 0)
                    targets.Add(aliveAllies[Random.Range(0, aliveAllies.Count)]);
                break;
            case TargetSelection.RandomAlly:
                var aliveAlliesRandom = allytargets.Where(p => !p.isDead).ToList();
                if (aliveAlliesRandom.Count > 0)
                    targets.Add(aliveAlliesRandom[Random.Range(0, aliveAlliesRandom.Count)]);
                break;
            case TargetSelection.AllAllies:
                targets.AddRange(allytargets.Where(p => !p.isDead));
                break;
        }
        return targets;

    }

    #endregion

    //public void StartBattle(EnemyData[] enemiesToSpawn, bool canRunAway)
    //{
    //    StartCoroutine(StartbattleRoutine(enemiesToSpawn, canRunAway));
    //}
    //private IEnumerator StartbattleRoutine(EnemyData[] enemiesToSpawn, bool canRunAway)
    //{
    //    yield return SceneTransition.instance.FadeToBlack();
    //    ResetBattleState();
    //    battleScene.SetActive(true);

    //    waitingForTurn = true;
    //    canRun = canRunAway;


    //    _rewardsPool.Clear();
    //    ResetCharacterValues();

    //    SetupPlayerCharacters(_playerData.targets);
    //    SetupEnemies(enemiesToSpawn);

    //    AddExistingCharacters();
    //    SortBattleCharactersBasedOnDexterity();

    //    _currentCharacterIndex = -1;
    //    NextTurn();

    //    yield return SceneTransition.instance.FadeFromBlack();
    //    TryTriggerTutorial();
    //}
    //private void TryTriggerTutorial()
    //{
    //    if (PlayerDataHandler.Singleton.currentPlayerData.triggeredFlags.Contains("BattleTutorial"))
    //    {
    //        tutorial.gameObject.SetActive(false);
    //        return;
    //    }

    //    PlayerDataHandler.Singleton.currentPlayerData.triggeredFlags.Add("BattleTutorial");
    //    Inventory.Singleton.AddItem(tutorialItem);

    //    tutorial.gameObject.SetActive(true);
    //    tutorial.StartTutorial();
    //}
    //private void ResetCharacterValues()
    //{
    //    activeCharacters.Clear();
    //    foreach (var character in playerCharacters)
    //    {
    //        character.ClearData();
    //    }
    //    foreach (var character in enemyCharacters)
    //    {
    //        character.ClearData();
    //    }
    //}
    //private void AddExistingCharacters()
    //{
    //    foreach (var character in playerCharacters)
    //    {
    //        if (character.CharacterData != null)
    //            activeCharacters.Add(character);
    //    }
    //    foreach (var character in enemyCharacters)
    //    {
    //        if (character.CharacterData != null)
    //            activeCharacters.Add(character);
    //    }
    //}
    //private BattleCharacters GetCurrentActiveCharacter()
    //{
    //    return activeCharacters[_currentCharacterIndex];
    //}
    //private void SortBattleCharactersBasedOnDexterity()
    //{
    //    activeCharacters.Sort((a, b) => b.CharacterData.stats.dexterity.CompareTo(a.CharacterData.stats.dexterity));
    //}
    //private void SetupEnemies(EnemyData[] enemiesToSpawn)
    //{
    //    for (int i = 0; i < enemiesToSpawn.Length; i++)
    //    {
    //        int index = i;
    //        CharacterDataPreset preset = enemiesToSpawn[index].Enemy;
    //        BattleCharacters enemyCharacter = enemyCharacters[index];
    //        enemyCharacter.SetupCharacter(preset.GetCopyOfPreset(), false, preset.normalSprite, preset.deathSprite);
    //        enemyCharacter.SetupDeathAction(() => { AddToRewards(enemiesToSpawn[index].rewards); } );
    //    }
    //}
    //private void AddToRewards(BattleRewardData rewardData)
    //{
    //    _rewardsPool.Add(rewardData);
    //}
    //private void SetupPlayerCharacters(List<Character> Characters)
    //{
    //    int battlePlayerIndex = 0;
    //    foreach(var character in Characters)
    //    {
    //        Sprite normalSprite = CharacterReferenceConfig.Singleton.GetReference(character.characterName).battleSprite;
    //        Sprite downedSprite = CharacterReferenceConfig.Singleton.GetReference(character.characterName).downedSprite;
    //        playerCharacters[battlePlayerIndex].SetupCharacter(character, true, normalSprite, downedSprite);
    //        battlePlayerIndex++;
    //    }
    //    _characterStatsPanel.SetupCharacters(Characters);
    //}
    //private void NextTurn()
    //{
    //    _currentCharacterIndex++;

    //    if (_currentCharacterIndex >= activeCharacters.Count)
    //        _currentCharacterIndex = 0;

    //    waitingForTurn = true;
    //    UpdatePlayerCharacterStatView();
    //    UpdateDeathStatus();

    //    if (IsGameOver())
    //    {
    //        return;
    //    }
    //    HandleTurnActions();
    //}
    //private void HandleTurnActions()
    //{
    //    if (!waitingForTurn)
    //        return;


    //    if (GetCurrentActiveCharacter().isPlayer)
    //    {
    //        UIButtonHolder.SetActive(true);
    //        //Player chooses their move on the uibuttonHolder
    //    }
    //    else if (!GetCurrentActiveCharacter().isDead )
    //    {
    //        UIButtonHolder.SetActive(false);
    //        StartCoroutine(EnemyMoveCoroutine());
    //    }
    //    else
    //    {
    //        NextTurn();
    //    }
    //}
    //public void UpdatePlayerCharacterStatView()
    //{
    //    _characterStatsPanel.UpdateValues();
    //}
    //private void UpdateDeathStatus()
    //{
    //    for (int i = 0; i < activeCharacters.Count; i++)
    //    {
    //        if (activeCharacters[i].CharacterData.stats.currentHP <= 0 && !activeCharacters[i].isDead)
    //        {
    //            activeCharacters[i].KillCharacter();
    //        }
    //    }
    //}
    //private bool IsGameOver()
    //{
    //    bool allEnemiesAreDead = activeCharacters.FindAll(x => !x.isPlayer && !x.isDead).Count == 0;
    //    bool allPlayersAreDead = activeCharacters.FindAll(x => x.isPlayer && !x.isDead).Count == 0;

    //    if (allEnemiesAreDead)
    //        StartCoroutine(EndBattleCoroutine());
    //    else if (allPlayersAreDead)
    //        StartCoroutine(GameOverCoroutine());

    //    return allPlayersAreDead || allEnemiesAreDead;
    //}
    //public void PlayerAttack(string moveName, BattleCharacters selectEnemyTarget)
    //{
    //    Debug.Log("BM: Move: " + moveName);
    //    //int selectEnemyTarget = 3;
    //    int movePower = 0;

    //    for (int i = 0; i < battleMovesList.Length; i++)
    //    {
    //        if (battleMovesList[i].moveName == moveName)
    //        {
    //            movePower = GettingMovePowerAndEffectInstanstiation(selectEnemyTarget, i);
    //        }
    //    }
    //    enemyTargetPanel.SetActive(false);
    //    DealDamageToCharacters(GetCurrentActiveCharacter(), selectEnemyTarget, movePower);

    //    NextTurn();
    //}
    //public IEnumerator EnemyMoveCoroutine()
    //{
    //    waitingForTurn = false;

    //    yield return new WaitForSeconds(1f);

    //    EnemyAttack();
    //    UpdatePlayerCharacterStatView();

    //    yield return new WaitForSeconds(1f);

    //    NextTurn();
    //}
    //private void EnemyAttack()
    //{
    //    List<BattleCharacters> players = activeCharacters.FindAll(x => x.isPlayer && !x.isDead);

    //    BattleCharacters selectedPlayerToAttack = players[Random.Range(0, players.Count)];

    //    //TODO update grabbing moves
    //    List<string> availableAttacks = GetCurrentActiveCharacter().AttackMovesAvailable();

    //    if (availableAttacks == null ||
    //        availableAttacks.Count== 0)
    //    {
    //        Debug.LogError(
    //            "BattleManager: Current enemy has no available attacks."
    //        );

    //        return;
    //    }

    //    int selectedAttack = Random.Range(0, availableAttacks.Count);
    //    int movePower = 0;

    //    for (int i = 0; i < battleMovesList.Length; i++)
    //    {
    //        if (battleMovesList[i].moveName == availableAttacks[selectedAttack])
    //        {
    //            movePower = GettingMovePowerAndEffectInstanstiation(selectedPlayerToAttack,i);
    //        }
    //    }

    //    DealDamageToCharacters(GetCurrentActiveCharacter(), selectedPlayerToAttack, movePower);

    //}
    //private void DealDamageToCharacters(BattleCharacters attacker, BattleCharacters defender, int movePower)
    //{
    //    Character attackerData = attacker.CharacterData;
    //    Character defenderData = defender.CharacterData;

    //    float attackPower = attackerData.stats.attack + attackerData.stats.weaponPower;
    //    float defenceAmount = defenderData.stats.defence + defenderData.stats.armorDefence;

    //    float flatDamage = attackPower - defenceAmount;
    //    flatDamage = flatDamage <= 0 ? 0 : flatDamage;

    //    float damageAmount = (flatDamage + movePower) * Random.Range(0.9f, 1.1f);
    //    int damageToGive = (int)damageAmount;

    //    damageToGive = CalculateCritical(damageToGive);
    //    Debug.Log($"attackPower: {attackPower} defenceAmount: {defenceAmount} damageAmount: {damageAmount}");

    //    defenderData.stats.ModifyHP(-damageToGive);
    //    defender.TakeDamage();

    //    DisplayAttackEffect(defender.transform);
    //    DisplayDamageText(defender.transform, damageToGive);
    //}
    //private void DisplayDamageText(Transform position, int amount)
    //{
    //    CharacterDamageGUI characterDamageText = Instantiate(damageText, position.transform.position, Quaternion.identity);
    //    characterDamageText.SetDamage(amount);
    //}
    //private void DisplayAttackEffect(Transform position)
    //{
    //    Instantiate(characterAttackEffect,position.transform.position, Quaternion.identity );
    //}
    //private int CalculateCritical(int damageToGive)
    //{
    //    if(Random.value <= 0.1f) // needs to be 0.1 so its a lower chance for a crit
    //    {
    //        Debug.Log("Critical Hit! Instead of " + damageToGive
    //            + " points. " + (damageToGive * 2) + " was dealt.");

    //        return (damageToGive * 2);
    //    }
    //    return damageToGive;

    //}
    //private int GettingMovePowerAndEffectInstanstiation(BattleCharacters characterToAttack, int i)
    //{
    //    //TODO FIX THIS
    //    int movePower = 0 /*battleMovesList[i].movePower*/;
    //    //Instantiate(battleMovesList[i].theEffectToUse, characterToAttack.transform.position, Quaternion.identity);

    //    return movePower;
    //}
    //public void OpenTargetMenu(string moveName)
    //{
    //    enemyTargetPanel.SetActive(true);

    //    List<BattleCharacters> EnemyTarget = activeCharacters.ToList().FindAll(x => !x.isPlayer && !x.isDead );

    //    foreach(var btn in targetButtons)
    //    {
    //        btn.gameObject.SetActive(false);
    //    }
    //    int i = 0;
    //    foreach( var btn in targetButtons)
    //    {
    //        if (i == EnemyTarget.Count)
    //            break;
    //        targetButtons[i].gameObject.SetActive(true);
    //        targetButtons[i].moveName = moveName;
    //        targetButtons[i].activeBattleTarget = EnemyTarget[i];
    //        targetButtons[i].targetName.text = EnemyTarget[i].CharacterData.characterName;
    //        i++;
    //    }
    //}
    //public void OpenSkillPanel()
    //{
    //    skillChoicePanel.SetActive(true);

    //    for (int i = 0; i < skillButtons.Length; i++)
    //    {
    //        if (GetCurrentActiveCharacter().AttackMovesAvailable().Count > i)
    //        {
    //            skillButtons[i].gameObject.SetActive(true);
    //            skillButtons[i].skillName = GetCurrentActiveCharacter().AttackMovesAvailable()[i];
    //            skillButtons[i].skillNameText.text = skillButtons[i].skillName;

    //            for(int j = 0; j < battleMovesList.Length; j++)
    //            {
    //                if (battleMovesList[j].moveName == skillButtons[i].skillName)
    //                {
    //                    skillButtons[i].skillCost = battleMovesList[j].staminaCost;
    //                    skillButtons[i].skillCostText.text = skillButtons[i].skillCost.ToString();

    //                    skillButtons[i].skillDesc = "" + battleMovesList[j].moveDescription;
    //                    skillButtons[i].skillDescText.text = skillButtons[i].skillDesc.ToString();

    //                }
    //            }
    //        }
    //        else
    //        {
    //            skillButtons[i].gameObject.SetActive(false);
    //        }
    //    }


    //}
    //public void RunAway()
    //{
    //    if (canRun)
    //    {
    //        if (Random.value > chanceToRunAway)
    //        {
    //            //isBattleActive = false;
    //            //battleScene.SetActive(false);
    //            runningAway = true;
    //            StartCoroutine(EndBattleCoroutine());
    //        }
    //        else
    //        {
    //            NextTurn();
    //            battleNotice.SetText("You failed to run away!");
    //            battleNotice.Activate();

    //        }
    //    }
    //}
    //public void UpdateItemsInInventory()
    //{
    //    if (Inventory.Singleton == null)
    //    {
    //        Debug.LogError(
    //            "BattleManager: Inventory Singleton is missing."
    //        );

    //        return;
    //    }

    //    if (itemsToUseMenu != null)
    //    {
    //        itemsToUseMenu.SetActive(true);
    //    }

    //    foreach (Transform itemSlot in itemSlotContainerParent)
    //    {
    //        Destroy(itemSlot.gameObject);
    //    }

    //    foreach (var item in Inventory.Singleton.GetItemsList())
    //    {
    //        if (item == null ||
    //            item.itemData.itemType != ItemData.ItemType.Consumable)
    //        {
    //            continue;
    //        }

    //        RectTransform itemSlot =
    //            Instantiate(
    //                itemSlotContainer,
    //                itemSlotContainerParent
    //            ).GetComponent<RectTransform>();

    //        Debug.Log("AMOUNT " + item.amount);
    //        itemSlot.GetComponent<ItemButton>().SetupItem(item.itemData, item.amount);
    //    }
    //}
    //public void SelectItem(ItemData itemToUse)
    //{
    //    // Prevent selecting equipment for battle use
    //    if (itemToUse.itemType != ItemData.ItemType.Consumable)
    //    {
    //        Debug.Log("Consumable is equipment, cannot be used in battle.");
    //        return;
    //    }

    //    // If the currently open character choice panel is visible and the player
    //    // selected a different item, close the character choice panel so it won't
    //    // remain open for the newly selected item.
    //    if (selectedItem != null && selectedItem != itemToUse && characterChoicePanel != null && characterChoicePanel.activeSelf)
    //    {
    //        characterChoicePanel.SetActive(false);
    //    }

    //    selectedItem = itemToUse;
    //    itemName.text = itemToUse.itemName;
    //    itemDescription.text = itemToUse.itemDescription;
    //}
    //private void ResetItemSelection()
    //{
    //    selectedItem = null;
    //    itemName.text = "";
    //    itemDescription.text = "";
    //}
    //public void OpenCharacterMenu()
    //{
    //    if (selectedItem == null)
    //    {
    //        Debug.LogWarning(
    //            "BattleManager: No item has been selected."
    //        );

    //        return;
    //    }

    //    characterChoicePanel.SetActive(true);

    //    for (int i = 0; i < playerNames.Length; i++)
    //    {
    //        if (playerNames[i] != null && playerNames[i].transform.parent != null)
    //            playerNames[i].transform.parent.gameObject.SetActive(false);
    //    }

    //    int playerButtonIndex = 0;

    //    foreach (BattleCharacters character in playerCharacters)
    //    {
    //        if (character.CharacterData == null)
    //            return;

    //        if (playerButtonIndex >= playerNames.Length)
    //            break;

    //        playerNames[playerButtonIndex].text = character.CharacterData.characterName;
    //        playerNames[playerButtonIndex].transform.parent.gameObject.SetActive(true);

    //        playerButtonIndex++;
    //    }
    //}
    //public void UseItemButton(int selectedPlayerIndex)
    //{
    //    if (selectedItem == null)
    //    {
    //        Debug.LogWarning("UseItemButton: no item selected.");
    //        return;
    //    }

    //    playerCharacters[selectedPlayerIndex].CharacterData.stats.UseItemOnCharacter(selectedItem);
    //    Inventory.Singleton.RemoveItem(selectedItem);

    //    CloseCharacterChoice();

    //    selectedItem = null;
    //    if (UIButtonHolder != null) UIButtonHolder.SetActive(false);

    //    NextTurn();
    //}
    //public void CloseCharacterChoice()
    //{
    //    characterChoicePanel.SetActive(false);
    //    itemsToUseMenu.SetActive(false);
    //    ResetItemSelection();
    //}
    //public IEnumerator EndBattleCoroutine()
    //{
    //    UIButtonHolder.SetActive(false);
    //    enemyTargetPanel.SetActive(false);
    //    skillChoicePanel.SetActive(false);

    //    if (!runningAway)
    //    {
    //        BattleRewardsHandler.instance.PrepareRewardScreen(_rewardsPool);
    //        battleNotice.SetText("You won!");
    //        battleNotice.Activate();

    //        yield return StartCoroutine(battleNotice.WaitUntilFinished());

    //        if (!runningAway && _currentBattleTrigger != null)
    //        {
    //            _currentBattleTrigger.MarkEnemyDefeated();
    //            _currentBattleTrigger = null;
    //        }
    //    }

    //    yield return (SceneTransition.instance.FadeToBlack());

    //    battleScene.SetActive(false);
    //    activeCharacters.Clear();

    //    AudioManager.instance.StopBattleMusic();

    //    if (runningAway)
    //    {
    //        if (_currentBattleTrigger != null)
    //        {
    //            _currentBattleTrigger.MoveEnemyAwayFromPlayer();
    //            _currentBattleTrigger = null;
    //        }

    //        runningAway = false;

    //        if (SceneTransition.instance != null)
    //        {
    //            yield return SceneTransition.instance.FadeFromBlack();
    //        }
    //    }

    //    else
    //    {
    //        BattleRewardsHandler.instance.DisplayMessags();
    //        // Let the rewards panel activate while the screen is black.
    //        yield return null;
    //        yield return new WaitForEndOfFrame();

    //        if (SceneTransition.instance != null)
    //        {
    //            yield return (SceneTransition.instance.FadeFromBlack());
    //        }

    //        BattleRewardsHandler.instance.PlayRewardSFX();
    //    }

    //    _currentCharacterIndex = 0;


    //    GameManager.Singleton.SetGameState(GameState.Roaming);
    //}
    //public IEnumerator GameOverCoroutine()
    //{
    //    GameManager.Singleton.SetPlayState(PlayState.GameOver);
    //    battleNotice.SetText("Game Over!");
    //    battleNotice.Activate();


    //    yield return new WaitForSeconds(2f);

    //    SceneManager.LoadScene(gameOverScene);
    //}
    //public void SetBattleTrigger(BattleInstantiator battleTrigger)
    //{
    //    _currentBattleTrigger = battleTrigger;
    //}
    //public void ResetBattleState()
    //{
    //    activeCharacters.Clear();

    //    waitingForTurn = false;
    //    _currentCharacterIndex = 0;
    //    runningAway = false;

    //    battleScene.SetActive(false);
    //    UIButtonHolder.SetActive(false);
    //    enemyTargetPanel.SetActive(false);
    //    skillChoicePanel.SetActive(false);

    //    foreach (var character in playerCharacters)
    //    {
    //        character.gameObject.SetActive(false);
    //    }
    //    foreach (var character in enemyCharacters)
    //    {
    //        character.gameObject.SetActive(false);
    //    }
    //}
}
[System.Serializable]
public class BattleMoveLibrary
{
    public BattleMoves defaultMove;
    public BattleMoves[] battleMovesList;
    public BattleMoves GetBattleMoveByName(string moveName)
    {
        foreach (var move in battleMovesList)
        {
            if (move.moveName == moveName)
                return move;
        }
        return null;
    }
    public List<BattleMoves> GetBattleMoves(List<string> names)
    {
        List<BattleMoves> moves = new();
        foreach (var moveName in names)
        {
            var move = GetBattleMoveByName(moveName);
            if (move != null)
                moves.Add(move);
        }
        return moves;
    }
}