using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int moveSpeed = 1;

    [SerializeField] Rigidbody2D playerRigidBody;
    [SerializeField] Animator playerAnimator;

    public string transitionName;

    private bool _movementDisabled => GameManager.Singleton.CurrentPlayingState != GameManager.PlayState.Gameplay || GameManager.Singleton.CurrentGameState != GameManager.GameState.Roaming;
    private bool deactivateMovement;
    private Vector3 bottomLeftEdge;
    private Vector3 topRightEdge;

    private bool transitionMovementLocked;

    private bool limitsSet = false;


    private void Awake()
    {
        if (playerRigidBody != null)
        {
            playerRigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            playerRigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_movementDisabled)
        {
            if (!deactivateMovement)
            {
                LockMovement();
            }
            playerRigidBody.velocity = Vector2.zero;
            return;
        }
        deactivateMovement = false;
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        playerRigidBody.velocity = new Vector2(horizontalMovement, verticalMovement) * moveSpeed;

        playerAnimator.SetFloat("movementX", playerRigidBody.velocity.x);
        playerAnimator.SetFloat("movementY", playerRigidBody.velocity.y);

        if ((horizontalMovement != 0 || verticalMovement != 0))
        {
            playerAnimator.SetFloat("lastX", horizontalMovement);
            playerAnimator.SetFloat("lastY", verticalMovement);
        }

        if (limitsSet)
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, bottomLeftEdge.x, topRightEdge.x),
                Mathf.Clamp(transform.position.y, bottomLeftEdge.y, topRightEdge.y),
                Mathf.Clamp(transform.position.z, bottomLeftEdge.z, topRightEdge.z)
            );
        }
    }

    private void SetIdleAnimation()
    {
        if (playerAnimator == null)
        {
            return;
        }

        playerAnimator.SetFloat("movementX", 0f);
        playerAnimator.SetFloat("movementY", 0f);
    }

    public void SetLimit(Vector3 bottomEdgeToSet, Vector3 topEdgeToSet)
    {
        bottomLeftEdge = bottomEdgeToSet;
        topRightEdge = topEdgeToSet;
        limitsSet = true;
    }

    // Public helper to move the player safely (uses Rigidbody2D to avoid physics snap issues)
    public void Teleport(Vector3 worldPosition)
    {
        // set transform immediately (visual)
        transform.position = worldPosition;

        // place rigidbody at same spot to avoid physics correcting later
        if (playerRigidBody != null)
        {
            playerRigidBody.position = new Vector2(worldPosition.x, worldPosition.y);
            playerRigidBody.velocity = Vector2.zero;
        }
    }

    public void LockMovement()
    {
        deactivateMovement = true;

        if (playerRigidBody != null)
        {
            playerRigidBody.velocity = Vector2.zero;
            playerRigidBody.angularVelocity = 0f;
        }
        SetIdleAnimation();
    }

    public void SetTransitionMovementLocked(bool locked)
    {
        transitionMovementLocked = locked;

        if (playerRigidBody != null)
        {
            playerRigidBody.velocity = Vector2.zero;
            playerRigidBody.angularVelocity = 0f;
            playerRigidBody.simulated = true;
        }

        // Disabling this component stops Player.Update() from reading input.
        enabled = !locked;
    }

    public void StopMovement()
    {
        SetIdleAnimation();
        SetTransitionMovementLocked(true);
    }

    public void ResumeMovement()
    {
        SetTransitionMovementLocked(false);

        if (playerRigidBody != null)
        {
            playerRigidBody.velocity = Vector2.zero;
            playerRigidBody.angularVelocity = 0f;
        }
    }
}
