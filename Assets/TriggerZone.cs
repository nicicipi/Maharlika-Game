using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField]
    private bool oneShot = true;

    [SerializeField]
    private string collisionTag = "Player";

    [Header("Destroy Settings")]
    [SerializeField]
    private bool destroyAfterTrigger;

    [SerializeField]
    private bool rememberDestroyedState;

    [SerializeField]
    private string triggerId;

    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private bool alreadyEntered;

    private string DestroyedKey =>
        "TriggerZone_Destroyed_" + triggerId;

    private void Start()
    {
        if (!rememberDestroyedState)
        {
            return;
        }

        if (string.IsNullOrEmpty(triggerId))
        {
            Debug.LogWarning(
                "TriggerZone requires a Trigger ID when remember state is enabled.",
                this
            );

            return;
        }

        if (PlayerPrefs.GetInt(DestroyedKey, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsCorrectCollision(collision))
        {
            return;
        }

        if (oneShot && alreadyEntered)
        {
            return;
        }

        alreadyEntered = true;
        onTriggerEnter?.Invoke();

        if (rememberDestroyedState &&
            !string.IsNullOrEmpty(triggerId))
        {
            PlayerPrefs.SetInt(DestroyedKey, 1);
            PlayerPrefs.Save();
        }

        if (destroyAfterTrigger)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsCorrectCollision(collision))
        {
            return;
        }

        onTriggerExit?.Invoke();

        if (!oneShot)
        {
            alreadyEntered = false;
        }
    }

    private bool IsCorrectCollision(Collider2D collision)
    {
        return string.IsNullOrEmpty(collisionTag) ||
               collision.CompareTag(collisionTag);
    }
}