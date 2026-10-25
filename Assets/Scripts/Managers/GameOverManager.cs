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
        SceneManager.LoadScene("MainMenu");
        GameManager.Singleton.SetGameState(GameManager.GameState.Roaming);
        GameManager.Singleton.SetPlayState(GameManager.PlayState.MainMenu);
    }

    public void LoadLastSave()
    {
        GameManager.Singleton.SetGameState(GameManager.GameState.Roaming);
        GameManager.Singleton.SetPlayState(GameManager.PlayState.Gameplay);
        GameManager.Singleton.LoadLastSave();
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
