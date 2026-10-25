using System.IO;
using UnityEditor;
using UnityEngine;

namespace SaveSystem
{
    public class PlayerDataHandler : MonoBehaviour
    {
        public static PlayerDataHandler Singleton;
        private void Awake()
        {
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }
            Singleton = this;

            LoadGame("save1");
        }
        public PlayerData cachedPlayerData = new();
        public PlayerData currentPlayerData = new();

        #region PlayerData updates
        //Changes the Current player data to the new data, and the current data to the cached data.
        public void LoadData(PlayerData data)
        {
            cachedPlayerData = new(data);
            currentPlayerData = new(cachedPlayerData);
        }
        public void SaveData()
        {
            Debug.Log("PDH: Saved data!");
            LoadData(currentPlayerData);
            SaveGame(currentPlayerData);
        }
        #region File Saving
        private string GetSavePath(string fileName) => Path.Combine(Application.persistentDataPath, fileName + ".json");
        public void SaveGame(PlayerData data) => File.WriteAllText(GetSavePath(data.playerSaveName), JsonUtility.ToJson(data, true));
        public void LoadGame(string fileName)
        {
            string path = GetSavePath(fileName);
            if (!File.Exists(path)) 
            { 
                Debug.LogWarning($"Save file not found: {fileName}"); 
                return; 
            }
            PlayerData loadedData = JsonUtility.FromJson<PlayerData>(File.ReadAllText(path));
            if (loadedData != null)
                LoadData(loadedData);

        }
        #endregion

        #endregion

        #region  Player Data Updates

        public void SavePosition(string asset, int position)
        {
            currentPlayerData.lastSceneName = asset;
            currentPlayerData.lastPositionIndex = position;
        }
        public void AddDefeatedEnemies(string enemyId)
        {
            Debug.Log($"PDH: defeated {enemyId}");
            currentPlayerData.defeatedEnemies++;
        }

        public void AddXPToCharacter(string character, int xp)
        {
            Character foundCharacter = currentPlayerData.characters.Find(x => x.characterName == character);
            if (foundCharacter == null)
                return;

            foundCharacter.stats.AddXP(xp);
        }
        public void AddXPToPresentCharacters(int xp)
        {
            foreach (var character in currentPlayerData.characters)
            {
                character.stats.AddXP(xp);
            }
        }
        public void ModifyMoney(int amount)
        {
            currentPlayerData.money += amount;
        }


        public bool IsTriggered(string flagName)
        {
            return currentPlayerData.triggeredFlags.Contains(flagName);
        }
        public void AddToTriggeredFlag(string flagName)
        {
            currentPlayerData.triggeredFlags.Add(flagName);
        }
        #endregion
    }
}