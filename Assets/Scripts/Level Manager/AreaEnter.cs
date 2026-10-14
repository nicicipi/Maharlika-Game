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
        for (int frame = 0; frame < 120; frame++)
        {
            if (TryPlacePlayerAtMatchingArea())
                yield break;

            yield return null;
        }
    }

    public bool TryPlacePlayer()
    {
        if (Player.instance == null ||
            string.IsNullOrEmpty(Player.instance.transitionName) ||
            transitionAreaName != Player.instance.transitionName)
        {
            return false;
        }

        Vector3 targetPosition = transform.position + Vector3.up * spawnYOffset;

        Player.instance.StopMovement();
        Player.instance.Teleport(targetPosition);
        Player.instance.transitionName = "";

        return true;
    }

    public static bool TryPlacePlayerAtMatchingArea()
    {
        if (Player.instance == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(Player.instance.transitionName))
        {
            return true;
        }

        AreaEnter[] areaEnters = FindObjectsOfType<AreaEnter>();

        foreach (AreaEnter areaEnter in areaEnters)
        {
            if (areaEnter.TryPlacePlayer())
                return true;
        }

        return false;
    }
}
