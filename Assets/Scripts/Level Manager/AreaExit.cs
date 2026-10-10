using UnityEngine;

using System.Collections;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] string transitionAreaName;

    [Header("Scene Transition")]
    [SerializeField] float fadeToBlackDelay = 0.35f;

    private bool isLoading;
    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryStartTransition(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryStartTransition(collision);
    }

    private void TryStartTransition(Collider2D collision)
    {
        if (isLoading || hasTriggered || !IsPlayerCollider(collision))
            return;

        if (SceneTransition.instance == null)
        {
            Debug.LogError("AreaExit: SceneTransition instance is missing.");
            return;
        }

        if (Player.instance == null)
        {
            Debug.LogError("AreaExit: Player instance is missing.");
            return;
        }

        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogError("AreaExit: Scene to load is not assigned.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            Debug.LogError("AreaExit: Scene is not in the build settings: " + sceneToLoad);
            return;
        }

        isLoading = true;
        hasTriggered = true;

        Player.instance.StopMovement();
        Player.instance.transitionName = transitionAreaName;

        SceneTransition.instance.TransitionToScene(sceneToLoad);
    }

    private bool IsPlayerCollider(Collider2D collision)
    {
        return collision.CompareTag("Player") || collision.transform.root.CompareTag("Player");
    }
}
