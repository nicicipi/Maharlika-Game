using System.Collections;
using UnityEngine;

public class AgentAnimations : MonoBehaviour
{
    [SerializeField]
    private Transform visualTransform;

    [SerializeField]
    private float initialYRotation = 180f;

    [SerializeField]
    private float facingLeftYRotation = 0f;

    [SerializeField]
    private float facingRightYRotation = 180f;

    private Animator animator;  
    private bool hasNoticedPlayer;
    private Transform target;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (visualTransform == null)
        {
            visualTransform = transform;
        }

        SetVisualYRotation(initialYRotation);
    }
    private void Start()
    {
        if (target == null)
        {
            StartCoroutine(WaitForPlayer());
        }
    }
    private IEnumerator WaitForPlayer()
    {
        while (GameManager.Singleton.currentPlayer == null)
            yield return null;
        yield return new WaitForEndOfFrame();
        target = GameManager.Singleton.currentPlayer.transform;
    }
    public void FacePlayer()
    {
        hasNoticedPlayer = true;

        Vector2 directionToPlayer = target.position - transform.position;

        RotateToPointer(directionToPlayer);
    }

    public void RotateToPointer(Vector2 lookDirection)
    {
        if (!hasNoticedPlayer ||
            Mathf.Abs(lookDirection.x) < 0.01f)
        {
            return;
        }

        if (lookDirection.x > 0f)
        {
            // Player is to the right.
            SetVisualYRotation(facingRightYRotation);
        }
        else
        {
            // Player is to the left.
            SetVisualYRotation(facingLeftYRotation);
        }
    }

    public void PlayAnimation(Vector2 movementInput)
    {
        if (animator != null)
        {
            animator.SetBool(
                "isWalking",
                movementInput.sqrMagnitude > 0.001f
            );
        }
    }

    private void SetVisualYRotation(float yRotation)
    {
        Vector3 rotation = visualTransform.localEulerAngles;
        rotation.y = yRotation;
        visualTransform.localEulerAngles = rotation;
    }
}