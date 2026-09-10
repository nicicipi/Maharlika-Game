using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] GameObject quitConfirmationPanel;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayBackgroundMusic(4);
        Player.instance.gameObject.SetActive(false);
        MenuManager.instance.gameObject.SetActive(false);
        BattleManager.instance.gameObject.SetActive(false);
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
        Destroy(GameManager.instance.gameObject);
        Destroy(Player.instance.gameObject);
        Destroy(MenuManager.instance.gameObject);
        Destroy(BattleManager.instance.gameObject);
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
