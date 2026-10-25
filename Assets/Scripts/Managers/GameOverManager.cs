using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] GameObject quitConfirmationPanel;
    // Start is called before the first frame update
    private void Start()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBackgroundMusic(4);
        }

        if (GameMenuManager.instance != null)
        {
            GameMenuManager.instance.gameObject.SetActive(false);
        }

        if (BattleManager.instance != null)
        {
            BattleManager.instance.gameObject.SetActive(false);
        }
    }

    public void QuitToMainMenu()
    {
        DestroyGameSession();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLastSave()
    {
        DestroyGameSession();
        SceneManager.LoadScene("LoadingScene");
    }

    private static void DestroyGameSession()
    {
        if (GameManager.Singleton != null)
        {
            GameManager manager = GameManager.Singleton;
            GameManager.Singleton = null;
            Destroy(manager.gameObject);
        }
        if (GameMenuManager.instance != null)
        {
            GameMenuManager menuManager = GameMenuManager.instance;
            GameMenuManager.instance = null;
            Destroy(menuManager.gameObject);
        }

        if (BattleManager.instance != null)
        {
            BattleManager battleManager = BattleManager.instance;
            BattleManager.instance = null;
            Destroy(battleManager.gameObject);
        }
    }

    public void OpenQuitConfirmation()
    { 
        quitConfirmationPanel.SetActive(true);
    }

    public void CloseQuitConfirmation()
    {
        quitConfirmationPanel.SetActive(false);
    }
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }


}
