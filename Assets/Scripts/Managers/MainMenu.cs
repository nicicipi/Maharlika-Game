using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] string newGameScene;
    [SerializeField] GameObject continueButton;
    [SerializeField] float waitToLoadTime;

    // Start is called before the first frame update
    private void Start()
    {

        if (PlayerPrefs.HasKey("Player_Pos_X"))
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
    }

    public void NewGameButton()
    {

        StartCoroutine(StartNewGame());
    }

    private IEnumerator StartNewGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetForNewGame();
        }


        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("NewGamePending", 1);
        PlayerPrefs.Save();

        yield return null;

        GameManager.instance.SetPlayState(GameManager.PlayState.Gameplay);
        SceneManager.LoadScene(newGameScene);
    }

    public void ExitButton()
    {
        Debug.Log("quited the game");
        Application.Quit();
    }

    public void ContinueButton()
    {
        SceneManager.LoadScene("LoadingScene");
        //StartCoroutine(ContinueGame());
    }

    //private IEnumerator ContinueGame()
    //{
    //    yield return new WaitForSeconds(waitToLoadTime);

    //    SceneManager.LoadScene(PlayerPrefs.GetString("Current_Scene"));
    //    GameManager.instance.LoadData();
    //    QuestManager.instance.LoadQuestData();
    //}
}