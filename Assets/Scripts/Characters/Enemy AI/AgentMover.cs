using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AgentMover : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2f;

    private Rigidbody2D rb2d;

    public Vector2 MovementInput { get; set; }

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (GameManager.Singleton.CurrentGameState == GameManager.GameState.Roaming && GameManager.Singleton.CurrentPlayingState == GameManager.PlayState.Gameplay)
            rb2d.velocity = MovementInput * moveSpeed;
        else
            rb2d.velocity = Vector2.zero;
    }
}