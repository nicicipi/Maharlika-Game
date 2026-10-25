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
        public PlayerData LoadGame(string fileName)
        {
            string path = GetSavePath(fileName);
            if (!File.Exists(path)) 
            { 
                Debug.LogWarning($"Save file not found: {fileName}"); 
                return null; 
            }
            PlayerData loadedData = JsonUtility.FromJson<PlayerData>(File.ReadAllText(path));

            return loadedData;

        }
        public void DeleteGame(string fileName)
        {
            string path = GetSavePath(fileName);
            if (File.Exists(path)) File.Delete(path);
        }
        #endregion

        #endregion

        #region  Player Data Updates
        public void FullRefresh()
        {
            foreach(var character in currentPlayerData.characters)
            {
                character.stats.currentHP = character.stats.maxHP;
                character.stats.currentStamina = character.stats.maxStamina;
            }
        }
        public void SavePosition(string asset, Vector2 position)
        {
            currentPlayerData.lastSceneName = asset;
            currentPlayerData.lastPosition = position;
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