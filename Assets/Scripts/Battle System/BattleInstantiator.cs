using BattleUtils;
using SaveSystem;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SaveFlagHandler))]
public class BattleInstantiator : MonoBehaviour
{
    [SerializeField] private EnemyData[] enemyDatas;
    [SerializeField] private bool canRunAway;
    [SerializeField] private float runAwayDistance = 2f;

    [SerializeField] private float battleTriggerCooldown = 2f;
    private Collider2D battleTrigger;
    [SerializeField] private bool shouldCompleteQuest;

    [SerializeField] private bool deactivateAfterStarting;

    [SerializeField] private int battleMusicIndex;

    public string questToComplete;

    public UnityEvent OnDefeat;
    public UnityEvent OnPlayerRun;
    public UnityEvent OnBattleEnd;

    private void Awake()
    {
        battleTrigger = GetComponent<Collider2D>();
    }

    [ContextMenu("DebugBattle")]
    public void StartBattle()
    {
        if (GameManager.Singleton.CurrentGameState == GameManager.GameState.Battle)
            return;
        GameManager.Singleton.SetGameState(GameManager.GameState.Battle);


        AudioManager.instance.StartBattleMusic(battleMusicIndex);
        SetupBattle();
    }
    public void SetupBattle()
    {
        int selectedBattle = Random.Range(0, enemyDatas.Length);
        BattleManager.instance.SetupBattle(enemyDatas.ToList(), canRunAway, null, this);

        //BattleRewardsHandler.instance.markQuestComplete = shouldCompleteQuest;
        //BattleRewardsHandler.instance.questToComplete = questToComplete;
    }
    public void MoveEnemyAwayFromPlayer()
    {

        Transform enemy = transform.root;
        Transform player = GameManager.Singleton.currentPlayer.transform;

        Vector2 directionAwayFromPlayer =
            (enemy.position - player.transform.position).normalized;

        if (directionAwayFromPlayer == Vector2.zero)
        {
            directionAwayFromPlayer = Vector2.right;
        }

        Vector2 newPosition =
            (Vector2)player.transform.position +
            directionAwayFromPlayer * runAwayDistance;

        Rigidbody2D enemyRigidbody =
            enemy.GetComponent<Rigidbody2D>();

        if (enemyRigidbody != null)
        {
            enemyRigidbody.position = newPosition;
            enemyRigidbody.velocity = Vector2.zero;
        }
        else
        {
            enemy.position = newPosition;
        }

        if (battleTrigger != null)
        {
            battleTrigger.enabled = false;

            Invoke(
                nameof(EnableBattleTrigger),
                battleTriggerCooldown
            );
        }
    }

    private void EnableBattleTrigger()
    {
        if (battleTrigger != null)
        {
            battleTrigger.enabled = true;
        }
    }

    public void MarkEnemyDefeated()
    {
        OnDefeat?.Invoke();
    }
    public void PlayerRun()
    {
        OnPlayerRun?.Invoke();
    }

}