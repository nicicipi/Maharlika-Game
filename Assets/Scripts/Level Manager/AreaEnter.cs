using System.Collections;
using UnityEngine;

public class AreaEnter : MonoBehaviour
{
    public string transitionAreaName;

    [SerializeField] float spawnYOffset;

    //private void Start()
    //{
    //    StartCoroutine(PlacePlayerAfterSceneSetup());
    //}

    //private IEnumerator PlacePlayerAfterSceneSetup()
    //{
    //    for (int frame = 0; frame < 120; frame++)
    //    {
    //        if (TryPlacePlayerAtMatchingArea())
    //            yield break;

    //        yield return null;
    //    }
    //}

    //public bool TryPlacePlayer()
    //{
    //    if (Player.Singleton == null ||
    //        string.IsNullOrEmpty(Player.Singleton.transitionName) ||
    //        transitionAreaName != Player.Singleton.transitionName)
    //    {
    //        return false;
    //    }

    //    Vector3 targetPosition = transform.position + Vector3.up * spawnYOffset;

    //    Player.Singleton.StopMovement();
    //    Player.Singleton.Teleport(targetPosition);
    //    Player.Singleton.transitionName = "";

    //    return true;
    //}

    //public static bool TryPlacePlayerAtMatchingArea()
    //{
    //    if (Player.Singleton == null)
    //    {
    //        return false;
    //    }

    //    if (string.IsNullOrEmpty(Player.Singleton.transitionName))
    //    {
    //        return true;
    //    }

    //    AreaEnter[] areaEnters = FindObjectsOfType<AreaEnter>();

    //    foreach (AreaEnter areaEnter in areaEnters)
    //    {
    //        if (areaEnter.TryPlacePlayer())
    //            return true;
    //    }

    //    return false;
    //}
}
