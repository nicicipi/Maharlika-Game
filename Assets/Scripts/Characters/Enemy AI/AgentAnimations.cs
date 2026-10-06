using UnityEngine;

public class AgentAnimations : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RotateToPointer(Vector2 lookDirection)
    {
        if (lookDirection.x > 0f)
        {
            spriteRenderer.flipX = true;
        }
        else if (lookDirection.x < 0f)
        {
            spriteRenderer.flipX = false;
        }
    }

    public void PlayAnimation(Vector2 movementInput)
    {
        animator.SetBool(
            "isWalking",
            movementInput.sqrMagnitude > 0.001f
        );
    }
}