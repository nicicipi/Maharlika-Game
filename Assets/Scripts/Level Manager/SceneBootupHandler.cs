using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBootupHandler : MonoBehaviour
{
    public Transform[] sceneEntryPoints;
    public void SpawnPlayerOnPoint(GameObject playerPrefab, int entryPoint)
    {
        Debug.Log($"Spawning player on pos: {entryPoint}");
        GameObject player = Instantiate(playerPrefab, sceneEntryPoints[entryPoint].position, Quaternion.identity);

        GameManager.Singleton.SetAsCurrentPlayer(player);
    }
    public void SpawnPlayerOnPoint(GameObject playerPrefab, Vector2 position)
    {
        Debug.Log($"Spawning player on pos: {position}");
        GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);

        GameManager.Singleton.SetAsCurrentPlayer(player);
    }
}
