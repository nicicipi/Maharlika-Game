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
        if (mapRenderer != null)
        {

            // SpriteRenderer.bounds gives the world-space bounding box
            bottomLeftEdge = mapRenderer.bounds.min + new Vector3(0.5f, 1f, 0f);
            topRightEdge = mapRenderer.bounds.max + new Vector3(-0.5f, -1f, 0f);

            Player.instance.SetLimit(bottomLeftEdge, topRightEdge);
        }
        else
        {
            Debug.LogWarning("LevelManager: mapRenderer is not assigned!");
        }
    }
}