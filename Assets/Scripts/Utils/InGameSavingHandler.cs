using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameSavingHandler : MonoBehaviour
{
    public Transform spawnPosition;
    public bool RefreshPlayer;
    public void SaveGame()
    {
        if (GameManager.Singleton.CurrentPlayingState == GameManager.PlayState.Saving)
        {
            Debug.LogWarning("Trying to save game while already saving.");
            return;
        }
        StartCoroutine(SaveGameRoutine());
    }

    public IEnumerator SaveGameRoutine()
    {
        GameManager.Singleton.SetPlayState(GameManager.PlayState.Saving);
        yield return SceneTransition.instance.FadeToBlack();
        if (RefreshPlayer)
        {
            AudioManager.instance.PlaySFX(20);
            PlayerDataHandler.Singleton.FullRefresh();
        }
        GameManager.Singleton.SaveGame(SceneManager.GetActiveScene().name, spawnPosition.position);
        yield return SceneTransition.instance.FadeFromBlack();
        GameManager.Singleton.SetPlayState(GameManager.PlayState.Gameplay);
    }
}
