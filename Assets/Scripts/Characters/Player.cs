using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField] int moveSpeed = 1;

    [SerializeField] Rigidbody2D playerRigidBody;
    [SerializeField] Animator playerAnimator;

    public string transitionName;

    public bool deactivateMovement = false;

    private Vector3 bottomLeftEdge;
    private Vector3 topRightEdge;

    private bool transitionMovementLocked;

    // new flag to avoid clamping until limits are provided
    private bool limitsSet = false;

    // Start is called before the first frame update
    void Start()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

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
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        bool movementLocked = deactivateMovement || transitionMovementLocked;

        if (movementLocked)
        {
            playerRigidBody.velocity = Vector2.zero;
        }
        else
        {
            playerRigidBody.velocity =
                new Vector2(horizontalMovement, verticalMovement) * moveSpeed;
        }

        playerAnimator.SetFloat("movementX", playerRigidBody.velocity.x);
        playerAnimator.SetFloat("movementY", playerRigidBody.velocity.y);

        if (!movementLocked &&
            (horizontalMovement != 0 || verticalMovement != 0))
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

    public void SetMovementLocked(bool locked)
    {
        deactivateMovement = locked;

        if (playerRigidBody != null)
        {
            playerRigidBody.velocity = Vector2.zero;
            playerRigidBody.angularVelocity = 0f;
            playerRigidBody.simulated = true;
        }
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
