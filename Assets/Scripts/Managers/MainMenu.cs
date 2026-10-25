using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject continueButton;
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
        GameManager.Singleton.LoadLastSave();
    }
}