using UnityEngine;

[RequireComponent(typeof(AgentMover))]
[RequireComponent(typeof(EnemyAI))]
public class Agent : MonoBehaviour
{
    private AgentAnimations agentAnimations;
    private AgentMover agentMover;
    private EnemyAI enemyAI;

    private void Awake()
    {
        agentAnimations = GetComponentInChildren<AgentAnimations>();
        agentMover = GetComponent<AgentMover>();
        enemyAI = GetComponent<EnemyAI>();
    }

    private void OnEnable()
    {
        if (enemyAI == null)
        {
            return;
        }

        enemyAI.OnMovementInput.AddListener(SetMovementInput);
        enemyAI.OnPointerInput.AddListener(SetPointerInput);
    }

    private void OnDisable()
    {
        if (enemyAI == null)
        {
            return;
        }

        enemyAI.OnMovementInput.RemoveListener(SetMovementInput);
        enemyAI.OnPointerInput.RemoveListener(SetPointerInput);
    }

    private void SetMovementInput(Vector2 movementInput)
    {
        agentMover.MovementInput = movementInput;

        if (agentAnimations != null)
        {
            agentAnimations.PlayAnimation(movementInput);
        }
    }

    private void SetPointerInput(Vector2 pointerPosition)
    {
        if (agentAnimations == null)
        {
            return;
        }

        Vector2 lookDirection =
            pointerPosition - (Vector2)transform.position;

        agentAnimations.RotateToPointer(lookDirection);
    }
}