using BattleUtils;
using SaveSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

    private void Awake()
    {
        battleTrigger = GetComponent<Collider2D>();
    }

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
        BattleManager.instance.SetBattleTrigger(this);
        BattleManager.instance.StartBattle(enemyDatas, canRunAway);

        BattleRewardsHandler.instance.markQuestComplete = shouldCompleteQuest;
        BattleRewardsHandler.instance.questToComplete = questToComplete;
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

}