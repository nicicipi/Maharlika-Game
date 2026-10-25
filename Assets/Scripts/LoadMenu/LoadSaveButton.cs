using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadSaveButton : MonoBehaviour
{
    public string SaveFileName = "Save1";
    public TextMeshProUGUI SaveName;
    public TextMeshProUGUI SceneName;

    private PlayerData _saveFile;

    private void OnEnable()
    {
        LoadPlayerData();
    }
    public void LoadPlayerData()
    {
        _saveFile = PlayerDataHandler.Singleton.LoadGame(SaveFileName);
        if(_saveFile == null)
        {
            SaveName.text = "No Data";
            SetupNewSave();
        }
        else
        {
            SetupButtons(_saveFile);
        }
    }

    public void SetupButtons(PlayerData data)
    {
        SaveName.text = SaveFileName;
        SceneName.text = data.lastSceneName;
    }

    public void StartSavedGame()
    {
        if (_saveFile == null)
            return;
        SetupButtons(_saveFile);
        GameManager.Singleton.LoadSave(_saveFile);
        PlayerDataHandler.Singleton.SaveGame(_saveFile);
    }

    public void DeleteSave()
    {
        PlayerDataHandler.Singleton.DeleteGame(SaveFileName);
        _saveFile = null;
        SetupNewSave();
    }

    private void SetupNewSave()
    {
        SaveName.text = "No Data";
        SceneName.text = "";
        _saveFile = new()
        {
            playerSaveName = SaveFileName
        };

    }
}
