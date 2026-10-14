using System.Collections;
using UnityEngine;

public class NpcWalkAway : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector2 walkDirection = Vector2.right;
    [SerializeField] private float walkDistance = 3f;
    [SerializeField] private float walkSpeed = 1.5f;

    [Header("Animation")]
    [SerializeField] private Animator npcAnimator;
    [SerializeField] private string walkingTrigger = "walkAway";

    [Header("Persistence")]
    [SerializeField] private string walkAwayId = "ChiefSaved";
    [SerializeField] private bool destroyAfterWalkingAway = true;

    [Header("Collision")]
    [SerializeField] private Collider2D[] collidersToDisableWhileWalking;

    private Coroutine walkCoroutine;

    private void Awake()
    {
        if (!string.IsNullOrWhiteSpace(walkAwayId) &&
            PlayerPrefs.GetInt("NpcWalkAway_" + walkAwayId, 0) == 1)
        {
            Destroy(transform.root.gameObject);
        }
    }

    public void WalkAway()
    {
        if (walkCoroutine != null)
        {
            return;
        }

        Vector2 direction = walkDirection.normalized;

        if (direction == Vector2.zero)
        {
            return;
        }

        SetPhysicalCollidersEnabled(false);
        SetWalkingAnimation();

        if (!string.IsNullOrWhiteSpace(walkAwayId))
        {
            PlayerPrefs.SetInt("NpcWalkAway_" + walkAwayId, 1);
            PlayerPrefs.Save();
        }

        walkCoroutine = StartCoroutine(WalkCoroutine(direction));
    }

    private IEnumerator WalkCoroutine(Vector2 direction)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition =
            startPosition + (Vector3)(direction * walkDistance);

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                walkSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
        walkCoroutine = null;

        if (destroyAfterWalkingAway)
        {
            Destroy(transform.root.gameObject);
        }
    }

    private void SetWalkingAnimation()
    {
        if (npcAnimator != null && !string.IsNullOrWhiteSpace(walkingTrigger))
        {
            npcAnimator.ResetTrigger(walkingTrigger);
            npcAnimator.SetTrigger(walkingTrigger);
        }
    }

    private void SetPhysicalCollidersEnabled(bool enabled)
    {
        if (collidersToDisableWhileWalking == null)
        {
            return;
        }

        foreach (Collider2D colliderToToggle in collidersToDisableWhileWalking)
        {
            if (colliderToToggle != null)
            {
                colliderToToggle.enabled = enabled;
            }
        }
    }

    public void StopWalking()
    {
        if (walkCoroutine != null)
        {
            StopCoroutine(walkCoroutine);
            walkCoroutine = null;
        }

        SetPhysicalCollidersEnabled(true);
    }
}
