using UnityEngine;

public class AreaExit : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] string transitionAreaName;

    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered || !collision.CompareTag("Player"))
            return;

        hasTriggered = true;
        Player.instance.transitionName = transitionAreaName;

        SceneTransition.instance.TransitionToScene(sceneToLoad);
    }
}
