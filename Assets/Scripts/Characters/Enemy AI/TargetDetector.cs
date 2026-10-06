using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField]
    private float targetDetectionRange = 5f;

    [SerializeField]
    private LayerMask obstaclesLayerMask;

    [SerializeField]
    private LayerMask playerLayerMask;

    [SerializeField]
    private bool showGizmos;

    private List<Transform> colliders;

    public override void Detect(AIData aiData)
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position,
            targetDetectionRange,
            playerLayerMask
        );

        if (playerCollider == null)
        {
            ClearTarget(aiData);
            return;
        }

        Vector2 directionToPlayer =
            playerCollider.transform.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            directionToPlayer.normalized,
            directionToPlayer.magnitude,
            obstaclesLayerMask | playerLayerMask
        );

        bool playerIsVisible =
            hit.collider != null &&
            (playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0;

        if (!playerIsVisible)
        {
            ClearTarget(aiData);
            return;
        }

        colliders = new List<Transform>
        {
            playerCollider.transform
        };

        aiData.targets = colliders;
    }

    private void ClearTarget(AIData aiData)
    {
        colliders = null;
        aiData.targets = null;
        aiData.currentTarget = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos)
        {
            return;
        }

        Gizmos.DrawWireSphere(
            transform.position,
            targetDetectionRange
        );
    }
}