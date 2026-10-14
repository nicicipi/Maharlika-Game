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
    void Start()
    {
        CleanupPersistentGameObjects();

        if (PlayerPrefs.HasKey("Player_Pos_X"))
            continueButton.SetActive(true);
        else
            continueButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DestroyPersistentGameObjects()
    {
        PlayerStats[] persistentPlayerStats =
            FindObjectsOfType<PlayerStats>(true);

        foreach (PlayerStats playerStats in persistentPlayerStats)
        {
            if (playerStats != null)
            {
                Destroy(playerStats.gameObject);
            }
        }

        if (BattleManager.instance != null)
        {
            BattleManager battleManager =
                BattleManager.instance;

            BattleManager.instance = null;
            battleManager.ResetBattleState();
            Destroy(battleManager.gameObject);
        }

        if (GameManager.instance != null)
        {
            GameManager gameManager =
                GameManager.instance;

            GameManager.instance = null;
            Destroy(gameManager.gameObject);
        }

        if (Player.instance != null)
        {
            Player player =
                Player.instance;

            Player.instance = null;
            Destroy(player.gameObject);
        }

        if (Inventory.instance != null)
        {
            Inventory inventory =
                Inventory.instance;

            Inventory.instance = null;
            Destroy(inventory.gameObject);
        }

        if (QuestManager.instance != null)
        {
            QuestManager questManager =
                QuestManager.instance;

            QuestManager.instance = null;
            Destroy(questManager.gameObject);
        }

        if (MenuManager.instance != null)
        {
            MenuManager menuManager =
                MenuManager.instance;

            MenuManager.instance = null;
            Destroy(menuManager.gameObject);
        }
    }

    private void CleanupPersistentGameObjects()
    {
        // Destroy persistent Player singleton
        if (Player.instance != null)
        {
            Debug.Log("Destroying persistent Player singleton");
            Destroy(Player.instance.gameObject);
        }

        // Destroy persistent GameManager singleton
        if (GameManager.instance != null)
        {
            Debug.Log("Destroying persistent GameManager singleton");
            Destroy(GameManager.instance.gameObject);
        }

        // Destroy persistent MenuManager singleton
        if (MenuManager.instance != null)
        {
            Debug.Log("Destroying persistent MenuManager singleton");
            Destroy(MenuManager.instance.gameObject);
        }

        // Destroy persistent AudioManager if it exists
        var audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            Debug.Log("Destroying persistent AudioManager singleton");
            Destroy(audioManager.gameObject);
        }

        // Destroy persistent SceneTransition if it exists
        var sceneTransition = FindObjectOfType<SceneTransition>();
        if (sceneTransition != null)
        {
            Debug.Log("Destroying persistent SceneTransition singleton");
            Destroy(sceneTransition.gameObject);
        }

        // Destroy persistent DialogueManager if it exists
        var dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            Debug.Log("Destroying persistent DialogueManager singleton");
            Destroy(dialogueManager.gameObject);
        }

        // Destroy persistent QuestManager if it exists
        var questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            Debug.Log("Destroying persistent QuestManager singleton");
            Destroy(questManager.gameObject);
        }

        // Destroy persistent Inventory if it exists
        var inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            Debug.Log("Destroying persistent Inventory singleton");
            Destroy(inventory.gameObject);
        }

        DestroyPersistentBattleManager();
    }

    private void DestroyPersistentBattleManager()
    {
        if (BattleManager.instance == null)
        {
            return;
        }

        BattleManager oldBattleManager = BattleManager.instance;

        oldBattleManager.ResetBattleState();
        BattleManager.instance = null;

        Destroy(oldBattleManager.gameObject);
    }

    public void NewGameButton()
    {
        DestroyPersistentBattleManager();

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

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