using BattleSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    public class CharacterReferenceConfig : MonoBehaviour
    {
        public static CharacterReferenceConfig Singleton;
        private void Awake()
        {
            Singleton = this;
        }
        [SerializeField] private List<CharacterReference> _characterReferences;
        public CharacterReference GetReference(string characterName)
        {
            CharacterReference foundCharacter = _characterReferences.Find(x => x.characterName == characterName);

            if (foundCharacter == null)
            {
                Debug.LogError($"CRC: no such character named {characterName}");
            }
            return foundCharacter;
        }
    }
    [Serializable]
    public class CharacterReference
    {
        public string characterName;
        public Sprite battleSprite;
        public Sprite downedSprite;
        public Sprite dialogueSprite;
        public List<UnlockableSkills> unlockableSkills = new();

        public BattleSkills GetLevelUpSkill(int level)
        {
            foreach (var skill in unlockableSkills)
            {
                if (level == skill.level)
                    return skill.skill;
            }
            return null;
        }
    }

    [Serializable]
    public class UnlockableSkills
    {
        public BattleSkills skill;
        public int level;
    }
}