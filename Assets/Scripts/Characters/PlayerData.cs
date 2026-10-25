using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SaveSystem
{
    [Serializable]
    public class PlayerData
    {
        public string playerSaveName;
        public string lastSceneName;
        public int lastPositionIndex;
        public int defeatedEnemies = 0;
        public int money = 0;
        public List<string> triggeredFlags = new();
        [SerializeField] public List<Character> characters = new();
        [SerializeField] public List<Item> items = new();
        public PlayerData(PlayerData toCopy)
        {
            if (toCopy == null) return;

            this.playerSaveName = toCopy.playerSaveName;
            this.lastSceneName = toCopy.lastSceneName;
            this.lastPositionIndex = toCopy.lastPositionIndex;
            this.defeatedEnemies = toCopy.defeatedEnemies;
            this.money = toCopy.money;

            Copy(triggeredFlags, toCopy.triggeredFlags);
            Copy(characters, toCopy.characters, x => new Character(x));
            Copy(items, toCopy.items, x => new Item(x));
        }

        private void Copy<T>(List<T> baseClass, List<T> toCopy, Func<T, T> copy)
        {
            baseClass.Clear();
            foreach (var item in toCopy) 
                baseClass.Add(copy(item));
        }
        private void Copy<T>(List<T> baseClass, List<T> toCopy)
        {
            baseClass.Clear();
            baseClass.AddRange(toCopy);
        }
        public PlayerData()
        {
            playerSaveName = "save1";
            lastSceneName = "Scene 1 Lyka Interior";
            lastPositionIndex = 0;
            defeatedEnemies = 0;
            money = 0;
            characters = new()
            {
                new()
            };
        }

        public Character GetCharacter(string name)
        {
            foreach(var character in characters)
            {
                if (name == character.characterName)
                    return character;
            }
            return null;
        }
    }
    [Serializable]
    public class Character
    {
        public string characterName = "Lyka";
        public CharacterStats stats = new();
        public List<string> moveSets = new();
        public Character(Character toCopy)
        {
            this.characterName = toCopy.characterName;
            this.stats = toCopy.stats != null ? new CharacterStats(toCopy.stats) : new();

            moveSets.Clear();
            foreach(var item in toCopy.moveSets)
            {
                this.moveSets.Add(item);
            }
        }

        public Character()
        {
            characterName = "Lyka";
            stats = new();
            moveSets = new()
            {
                "Slash 2",
                "Slash 3",
                "Impact"
            };
        }
    }
    [Serializable]
    public class Item
    {
        public string itemName;
        public int itemCount;
        public Item()
        {
            itemName = "none";
            itemCount = 0;
        }
        public Item(Item toCopy)
        {
            itemName = toCopy.itemName;
            itemCount = toCopy.itemCount;
        }
    }
}
