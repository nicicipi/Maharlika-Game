using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject LoadGameScreen;
    [SerializeField] float waitToLoadTime;

    public void NewGameButton()
    {
        GameManager.Singleton.StartNewGame();
    }

    public void ExitButton()
    {
        Debug.Log("quited the game");
        Application.Quit();
    }

    public void ContinueButton()
    {
        // Open LoadGameScreen
        LoadGameScreen.SetActive(true);
        GameManager.Singleton.LoadLastSave();
    }
}