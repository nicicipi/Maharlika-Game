using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private float waitToLoadTime = 2f;
    [SerializeField] private string fallbackScene = "Scene 0 Lyka House";

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
        LoadSavedScene();
    }

    private void LoadSavedScene()
    {
        string sceneToLoad = fallbackScene;

        if (PlayerPrefs.HasKey("Current_Scene"))
        {
            string savedScene = PlayerPrefs.GetString("Current_Scene");
            if (!string.IsNullOrWhiteSpace(savedScene))
            {
                sceneToLoad = savedScene;
            }
        }

        Debug.Log("LoadingScreen: Loading scene: " + sceneToLoad);

        if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            Debug.LogError("LoadingScreen: Scene not in build settings: " + sceneToLoad);
            sceneToLoad = fallbackScene;
        }

        SceneManager.LoadScene(sceneToLoad);
        StartCoroutine(LoadGameDataAfterSceneLoad());
    }

    private IEnumerator LoadGameDataAfterSceneLoad()
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        if (GameManager.instance != null)
        {
            GameManager.instance.RefreshPlayerStats();
            GameManager.instance.LoadData();

            Debug.Log("LoadingScreen: Game data loaded.");
        }

        if (QuestManager.instance != null)
        {
            QuestManager.instance.LoadQuestData();
            Debug.Log("LoadingScreen: Quest data loaded.");
        }

        if (Player.instance != null)
        {
            Player.instance.ResumeMovement();
            Debug.Log("LoadingScreen: Player movement resumed.");
        }
        else
        {
            Debug.LogWarning("LoadingScreen: Player instance not found.");
        }
    }
}
