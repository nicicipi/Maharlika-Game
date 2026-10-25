using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mapRenderer;

    private Vector3 bottomLeftEdge;
    private Vector3 topRightEdge;

    void Start()
    {
        if (mapRenderer == null)
        {
            Debug.LogError("LevelManager: mapRenderer is not assigned!" + gameObject.GetInstanceID());
            return;
        }

        StartCoroutine(WaitForPlayerSpawn());
    }

    IEnumerator WaitForPlayerSpawn()
    {
        while (GameManager.Singleton.currentPlayer == null)
            yield return null;

        bottomLeftEdge = mapRenderer.bounds.min + new Vector3(0.5f, 1f, 0f);
        topRightEdge = mapRenderer.bounds.max + new Vector3(-0.5f, -1f, 0f);

        GameManager.Singleton.currentPlayer.SetLimit(bottomLeftEdge, topRightEdge);
    }
}