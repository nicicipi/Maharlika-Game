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
    public static BattleManager instance;
    private BattleInstantiator _currentBattleTrigger;
    private PlayerData _playerData => PlayerDataHandler.Singleton.currentPlayerData;
    private BattleSkills _currentBattleMove;
    [SerializeReference] private CameraShaker _cameraShaker;


    #region Refactor
    [Header("Tutorial")]
    [SerializeField] private BattleTutorialManager tutorial;
    [SerializeField] private ItemData tutorialItem;


    [Header("Battle Moves")]
    public BattleSkillLibrary battleMoveLibrary;
    public CharacterDamageGUI characterDamageText;
    public CharacterDamageGUI characterHealText;
    [Header("Battle UI")]
    public GameObject battleScene;
    public GameObject UI_Choices;
    public SkillChoicePanel UI_Skills;
    public InventoryPanel UI_Items;
    public CharacterTargetPanel UI_BasicAttackTargets;
    [SerializeField] public CharacterStatsPanel _characterStatsPanel;
    [SerializeField] private List<BattleCharacters> _battleCharacterViewPlayers = new();
    [SerializeField] private List<BattleCharacters> _battleCharacterViewEnemies = new();
    public NotificationPanel battleNotice => NotificationPanel.Singleton;

    [Header("Battle Settings")]
    private UIState _currentUIState;
    private TurnState _currentTurnState;
    private List<BattleCharacters> _activeBattleCharacters = new();
    private BattleCharacters _currentPlayerCharacter;
    private bool _waitingForPlayerInput = true;
    private bool _runningAway = false;
    private float _runawayChance = 0.3f;
    private Coroutine _currentPlayerRoutine;
    private Coroutine _battleRoutine;

    [Header("Rewards")]
    [SerializeField] private List<BattleRewardData> _rewardsPool = new();
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
                RunAway(_runawayChance);
                break;
        }
    }

    private void SetTurnState(TurnState state)
    {
        _characterStatsPanel.UpdateSPValues();
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
        if (_battleRoutine != null)
            Debug.LogError("Battle is stil active");

        _runawayChance = canRunAway ? 0.3f : 0f;

        _runningAway = false;
        _rewardsPool.Clear();
        GameManager.Singleton.SetGameState(GameState.Battle);
        _currentBattleTrigger = currentBattleTrigger; ;
        PlayerCharacters ??= _playerData.characters;
        SetupBattleCharacters(PlayerCharacters, Enemies);
        _battleRoutine = StartCoroutine(BattleRoutine());
        TryTriggerTutorial();
    }
    private void TryTriggerTutorial()
    {

        if (PlayerDataHandler.Singleton.IsTriggered("BattleTutorial"))
        {
            tutorial.gameObject.SetActive(false);
            return;
        }
        //_runawayChance = 0f;
        PlayerDataHandler.Singleton.AddToTriggeredFlag("BattleTutorial");
        Inventory.Singleton.AddItem(tutorialItem);

        tutorial.gameObject.SetActive(true);
        tutorial.StartTutorial();
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

        foreach (EnemyData character in Enemies)
        {
            _battleCharacterViewEnemies[battleEnemyIndex].SetupCharacter(character.Enemy.GetCopyOfPreset(), character.Enemy.battleMoves, false, character.Enemy.normalSprite, character.Enemy.deathSprite);
            _battleCharacterViewEnemies[battleEnemyIndex].SetupDeathAction(() => { AddRewards(character.rewards); });
            _activeBattleCharacters.Add(_battleCharacterViewEnemies[battleEnemyIndex]);
            battleEnemyIndex++;
        }
    }

    private void AddRewards(BattleRewardData rewardData)
    {
        _rewardsPool.Add(rewardData);
    }
    #endregion

    #region Actions
    private IEnumerator BattleRoutine()
    {
        yield return SceneTransition.instance.FadeToBlack();
        battleScene.SetActive(true);
        yield return SceneTransition.instance.FadeFromBlack();
        yield return new WaitForSeconds(1f);
        while (true)
        {
            
            yield return PlayerInputRoutine(); //wait for player input
            
            if (_runningAway)
                break;

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
            if (!player.isPlayer || _runningAway || player.isDead)
                continue;

            if (GameEndCheck())
                break;

            player.SelectCharacter(true);

            yield return player.ProcessStatusEffects(true);
             
            if (player.isDead || GameEndCheck())
                continue;

            if (!player.skipTurn)
            {
                _currentPlayerCharacter = player;
                _waitingForPlayerInput = true;

                SetUIState(UIState.ChoiceSelection);

                yield return new WaitUntil(() => !_waitingForPlayerInput);
            }
            player.skipTurn = false;

            player.SelectCharacter(false);

            if (GameEndCheck())
                break;

            yield return player.ProcessStatusEffects(false);
            
            if (GameEndCheck())
                break;
        }
        _currentPlayerCharacter = null;
    }
    public void TriggerPlayerInputEnd()
    {
        _waitingForPlayerInput = false;
    }

    private void OnPlayerSelectSkill(BattleSkills move, List<BattleCharacters> targets)
    {
        if(_currentPlayerRoutine != null)
        {
            Debug.Log("Player Skill routine is in progress");
            return; 
        }    
        _currentPlayerRoutine = StartCoroutine(PlayerSkillRoutine(move, targets));
    }
    private IEnumerator PlayerSkillRoutine(BattleSkills move, List<BattleCharacters> targets)
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
            Debug.Log("Default attack target selected: " + selectedTarget.CharacterData.characterName);
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

            enemy.SelectCharacter(true);

            yield return enemy.ProcessStatusEffects(true);

            if (enemy.isDead || GameEndCheck())
            {
                enemy.SelectCharacter(false);
                continue;
            }

            if (!enemy.skipTurn)
                yield return ExecuteSimpleEnemyAI(enemy);

            enemy.skipTurn = false;

            yield return enemy.ProcessStatusEffects(false);
            enemy.SelectCharacter(false);

            if (GameEndCheck())
                break;
        }
    }
    private IEnumerator ExecuteSimpleEnemyAI(BattleCharacters enemy)
    {
        List<BattleSkills> availableSkills = enemy.battleMovesList;

        if (availableSkills.Count == 0)
        {
            Debug.LogError($"Enemy {enemy.CharacterData.characterName} has no available skills.");
            yield break;
        }

        BattleSkills selectedSkill = availableSkills[Random.Range(0, availableSkills.Count)];
        List<BattleCharacters> targets = GetTargettableCharacters(enemy, selectedSkill.targetSelection );

        if (targets.Count == 0)
        {
            Debug.LogWarning($"No valid targets for {selectedSkill.skillName}.");
            yield break;
        }

        // Pick one target for single-target actions.
        if (selectedSkill.targetSelection == TargetSelection.SingleEnemy ||
            selectedSkill.targetSelection == TargetSelection.SingleAlly)
        {
            BattleCharacters target = targets[Random.Range(0, targets.Count)];
            targets.Clear();
            targets.Add(target);
        }
        yield return ExecuteMove(enemy, selectedSkill, targets);

    }
    public IEnumerator ExecuteMove(BattleCharacters source, BattleSkills battleMove, BattleCharacters target)
    {
        source.CharacterData.stats.AddSP(-battleMove.staminaCost);
        _currentBattleMove = battleMove;
        _characterStatsPanel.UpdateSPValues();
        _characterStatsPanel.UpdateHPValues();
        Debug.Log($"Executing Move {battleMove.skillName}, source: {source.CharacterData.characterName}, target: {target.CharacterData.characterName}" );
        foreach (var battleAction in battleMove.actions)
        {
            // VFX
            target.SpawnVFX(battleAction.action.actionVFX);
            // Stats
            yield return battleAction.action.ApplyActionToTarget(source, target);
            _characterStatsPanel.UpdateHPValues();
        }
        yield return new WaitForSeconds(battleMove.skillDuration);
    }
    public IEnumerator ExecuteMove(BattleCharacters source, BattleSkills battleMove, List<BattleCharacters> targets)
    {
        source.CharacterData.stats.AddSP(-battleMove.staminaCost);
        _currentBattleMove = battleMove;
        _characterStatsPanel.UpdateSPValues();
        _characterStatsPanel.UpdateHPValues();
        foreach (var target in targets)
        {
            Debug.Log($"Executing Move {battleMove.skillName}, attacker: {source.CharacterData.characterName}, defender: {target.CharacterData.characterName}");
            foreach (var battleAction in battleMove.actions)
            {
                // VFX
                target.SpawnVFX(battleAction.action.actionVFX);
                // Stats
                yield return battleAction.action.ApplyActionToTarget(source, target);
                _characterStatsPanel.UpdateHPValues();

            }
        }
        yield return new WaitForSeconds(battleMove.skillDuration);
    }
    private bool GameEndCheck()
    {
        bool allEnemiesAreDead = _activeBattleCharacters.FindAll(x => !x.isPlayer && !x.isDead).Count == 0;
        bool allPlayersAreDead = _activeBattleCharacters.FindAll(x => x.isPlayer && !x.isDead).Count == 0;
        return allEnemiesAreDead || allPlayersAreDead;
    }
    private void BattleEnd()
    {
        _battleRoutine = null;
        bool allEnemiesAreDead = _activeBattleCharacters.FindAll(x => !x.isPlayer && !x.isDead).Count == 0;
        bool allPlayersAreDead = _activeBattleCharacters.FindAll(x => x.isPlayer && !x.isDead).Count == 0;

        if (allEnemiesAreDead)
            StartCoroutine(EndBattleCoroutine(BattleResult.Victory));
        else if (allPlayersAreDead)
            StartCoroutine(EndBattleCoroutine(BattleResult.Defeat));
        else if (_runningAway)
            StartCoroutine(EndBattleCoroutine(BattleResult.PlayerRun));

    }
    private IEnumerator EndBattleCoroutine(BattleResult result)
    {
        yield return SceneTransition.instance.FadeToBlack();
        battleScene.SetActive(false);
        _activeBattleCharacters.Clear();
        AudioManager.instance.StopBattleMusic();

        switch (result)
        {
            case BattleResult.Victory:
                BattleRewardsHandler.instance.PrepareRewardScreen(_rewardsPool);
                BattleRewardsHandler.instance.DisplayMessages();
                if (_currentBattleTrigger != null)
                {
                    _currentBattleTrigger.MarkEnemyDefeated();
                    _currentBattleTrigger = null;
                }
                break;
            case BattleResult.Defeat:
                GameManager.Singleton.SetPlayState(PlayState.GameOver);
                battleNotice.ShowNotification("Game Over!");
                yield return new WaitForSeconds(3f);
                SceneManager.LoadScene("GameOverScene");
                yield break;
            case BattleResult.PlayerRun:
                if (_currentBattleTrigger != null)
                {
                    _currentBattleTrigger.PlayerRun();
                    _currentBattleTrigger = null;
                }
                break;
        }

        yield return SceneTransition.instance.FadeFromBlack();
        yield return new WaitForSeconds(1f);
        GameManager.Singleton.SetGameState(GameState.Roaming);
        _currentBattleTrigger.OnBattleEnd?.Invoke();
    }
    public void RunAway(float chance)
    {
        if (Random.value < chance)
        {
            _runningAway = true;
            battleNotice.ShowNotification("Succesfully ran away!");
        }
        else
        {
            battleNotice.ShowNotification("You failed to run away!");
        }
        TriggerPlayerInputEnd();
    }
    #endregion

    #region Helpers

    public void ShakeCamera(float duration = 0.5f, float power = 3f)
    {
        _cameraShaker.ShakeCamera(duration, power);
    }
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
                    targets.AddRange(aliveEnemies);
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
                targets.AddRange(aliveAllies);
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
    public enum BattleResult
    {
        Victory,
        Defeat,
        PlayerRun,
        EnemyRun
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
}