using SaveSystem;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    public Player currentPlayer;
    public Player playerPrefab;
    public enum GameState
    {
        Dialogue,
        Battle,
        Shop,
        Cutscene,
        Roaming
    }
    public enum PlayState
    {
        MainMenu,
        Loading,
        Saving,
        Menus,
        GameOver,
        Gameplay,
    }
    private PlayState _currentPlayingState;
    public PlayState CurrentPlayingState => _currentPlayingState;
    private GameState _currentGameState;
    public GameState CurrentGameState => _currentGameState;
    public TextMeshProUGUI PlayingStateText;
    public TextMeshProUGUI GameStateText;

    private void Awake()
    {
        if (Singleton != null)
        {
            Debug.Log("GM Destroying self");
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        SetPlayState(PlayState.MainMenu);
    }
    void Update()
    {
        if (CurrentPlayingState == PlayState.Loading)
            return;

        if (Input.GetKeyUp(KeyCode.I))
        {
            Debug.Log("Data has been saved");
            DebugSave();
        }

        if (Input.GetKeyUp(KeyCode.O))
        {
            Debug.Log("Data has been loaded");
            LoadLastSave();
        }
    }
    public void SetAsCurrentPlayer(GameObject newPlayer)
    {
        currentPlayer = newPlayer.GetComponent<Player>();
    }

    public void SetGameState(GameState state)
    {
        Debug.Log("SettingGamestate: " + state);
        GameStateText.text = "GameState: " + state;
        _currentGameState = state;
    }
    public void SetPlayState(PlayState state)
    {
        PlayingStateText.text = "PlayState: " + state;
        _currentPlayingState = state;
    }

    #region SaveSystem
    public void DebugSave()
    {
        SaveGame(SceneManager.GetActiveScene().name, currentPlayer.transform.position);
    }
    public void SaveGame(string SceneName, Vector2 position)
    {
        PlayerDataHandler.Singleton.SavePosition(SceneName, position);
        Inventory.Singleton.SaveData(PlayerDataHandler.Singleton.currentPlayerData);
        PlayerDataHandler.Singleton.SaveData();
    }

    public void StartNewGame()
    {
        StartCoroutine(LoadSaveFileRoutine(new()));
    }
    public void LoadSave(PlayerData data)
    {
        StartCoroutine(LoadSaveFileRoutine(data));
    }
    public void LoadLastSave()
    {
        LoadSave(PlayerDataHandler.Singleton.cachedPlayerData);
    }
    public IEnumerator LoadSaveFileRoutine(PlayerData data)
    {
        GameManager.Singleton.SetPlayState(GameManager.PlayState.Loading);
        GameManager.Singleton.SetGameState(GameManager.GameState.Roaming);
        PlayerDataHandler.Singleton.LoadData(data);
        Inventory.Singleton.LoadData(PlayerDataHandler.Singleton.currentPlayerData);
        yield return new WaitForEndOfFrame();
        if(data.lastPosition != Vector2.zero)
        {
            SceneTransition.instance.TransitionToScene(data.lastSceneName.GetSceneAsset(), 0, data.lastPosition);
        }
        else
        {
            SceneTransition.instance.TransitionToScene(data.lastSceneName.GetSceneAsset(), 0);
        }
    }
    #endregion
}