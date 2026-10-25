using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private float waitToLoadTime = 2f;

    private void Start()
    {
        if (waitToLoadTime > 0)
        {
            StartCoroutine(LoadScene());
        }
        else
        {
            LoadSavedScene();
        }
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSecondsRealtime(waitToLoadTime);
        //LoadSavedScene();
    }

    private void LoadSavedScene()
    {
        //string sceneToLoad = PlayerDataHandler.Singleton.currentPlayerData.lastSceneName;
        //SceneManager.LoadScene(sceneToLoad);
        //StartCoroutine(LoadGameDataAfterSceneLoad());
    }

    private IEnumerator LoadGameDataAfterSceneLoad()
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        if (GameManager.Singleton != null)
        {
            //GameManager.Singleton.LoadData();

            Debug.Log("LoadingScreen: Game data loaded.");
        }

        if (QuestManager.instance != null)
        {
            QuestManager.instance.LoadQuestData();
            Debug.Log("LoadingScreen: Quest data loaded.");
        }
    }
}
