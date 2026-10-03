using System.Collections;
using UnityEngine;

public class AreaEnter : MonoBehaviour
{
    public string transitionAreaName;

    [SerializeField] float spawnYOffset;

    private void Start()
    {
        StartCoroutine(PlacePlayerAfterSceneSetup());
    }

    private IEnumerator PlacePlayerAfterSceneSetup()
    {
        yield return new WaitForEndOfFrame();

        if (Player.instance == null)
            yield break;

        if (string.IsNullOrEmpty(Player.instance.transitionName))
            yield break;

        if (transitionAreaName != Player.instance.transitionName)
            yield break;

        Vector3 targetPosition = transform.position + Vector3.up * spawnYOffset;

        Player.instance.StopMovement();
        Player.instance.Teleport(targetPosition);

        Player.instance.transitionName = "";
    }
}
