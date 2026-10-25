using System.Collections;
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

    public UnityEvent<Vector2> OnMovementInput = new();
    public UnityEvent<Vector2> OnPointerInput = new();

   
    private void OnEnable()
    {
        StartCoroutine(AIBehaviour());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
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
        
    }
    IEnumerator AIBehaviour()
    {
        while (true)
        {
            yield return null;
            if (aiData == null)
            {
                continue;
            }

            // every second only
            if(Time.frameCount % 60 == 0)
                PerformDetection();

            if (GameManager.Singleton.CurrentGameState != GameManager.GameState.Roaming)
            {
                StopAIBehavior();
                continue;
            }

            if (aiData.targets == null || aiData.targets.Count == 0)
            {
                StopAIBehavior();
                continue;
            }


            if (aiData.currentTarget == null)
            {
                if (TryGetTarget(out Transform target))
                    aiData.currentTarget = target;
            }

            Vector2 movementInput = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
            if (aiData != null && aiData.currentTarget != null && OnPointerInput != null && aiData.currentTarget.position != null)
            {
                OnPointerInput?.Invoke(aiData.currentTarget.position);
                OnMovementInput?.Invoke(movementInput);
            }
        }
    }
    private bool TryGetTarget(out Transform targetTransform)
    {
        targetTransform = null;
        foreach (var target in aiData.targets)
        {
            if (target != null)
            {
                targetTransform = target;
                break;
            }
        }
        return targetTransform != null;
    }
    private void StopAIBehavior()
    {
        aiData.currentTarget = null;
        OnMovementInput?.Invoke(Vector2.zero);
    }
}