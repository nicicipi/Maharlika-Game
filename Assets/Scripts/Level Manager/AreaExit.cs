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
        if (isLoading)
            return;

        TryStartTransition(collision);
    }

    private void TryStartTransition(Collider2D collision)
    {
        if (hasTriggered || !collision.CompareTag("Player"))
            return;

        if (SceneTransition.instance == null)
        {
            Debug.LogError("AreaExit: SceneTransition instance is missing.");
            return;
        }

        hasTriggered = true;

        Player.instance.StopMovement();
        Player.instance.transitionName = transitionAreaName;

        SceneTransition.instance.TransitionToScene(sceneToLoad);
    }
}
