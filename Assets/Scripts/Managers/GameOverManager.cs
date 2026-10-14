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

        if (Player.instance != null)
        {
            Player.instance.gameObject.SetActive(false);
        }

        if (MenuManager.instance != null)
        {
            MenuManager.instance.gameObject.SetActive(false);
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
        if (GameManager.instance != null)
        {
            GameManager manager = GameManager.instance;
            GameManager.instance = null;
            Destroy(manager.gameObject);
        }

        if (Player.instance != null)
        {
            Player player = Player.instance;
            Player.instance = null;
            Destroy(player.gameObject);
        }

        if (MenuManager.instance != null)
        {
            MenuManager menuManager = MenuManager.instance;
            MenuManager.instance = null;
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
