using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private List<SteeringBehaviour> steeringBehaviours;

    [SerializeField]
    private List<Detector> detectors;

    [SerializeField]
    private AIData aiData;

    [SerializeField]
    private ContextSolver movementDirectionSolver;

    [SerializeField]
    private float detectionDelay = 0.05f;

    public UnityEvent<Vector2> OnMovementInput;
    public UnityEvent<Vector2> OnPointerInput;

    private void Start()
    {
        InvokeRepeating(
            nameof(PerformDetection),
            0f,
            detectionDelay
        );
    }

    private void PerformDetection()
    {
        foreach (Detector detector in detectors)
        {
            if (detector != null)
            {
                detector.Detect(aiData);
            }
        }
    }

    private void Update()
    {
        if (aiData.targets == null ||
            aiData.targets.Count == 0)
        {
            aiData.currentTarget = null;

            // Immediately stop the enemy when the player is not visible.
            OnMovementInput?.Invoke(Vector2.zero);
            return;
        }

        if (aiData.currentTarget == null ||
            !aiData.targets.Contains(aiData.currentTarget))
        {
            aiData.currentTarget = aiData.targets[0];
        }

        Vector2 movementInput =
            movementDirectionSolver.GetDirectionToMove(
                steeringBehaviours,
                aiData
            );

        OnPointerInput?.Invoke(aiData.currentTarget.position);
        OnMovementInput?.Invoke(movementInput);
    }
}