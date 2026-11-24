using BattleSystem;
using SaveSystem;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ChacterDataConfig", menuName = "Characters/CharacterDataPreset")]
public class CharacterDataPreset : ScriptableObject
{
    [SerializeField] private Character characterPreset;
    public List<BattleMoves> battleMoves;
    public Sprite normalSprite;
    public Sprite deathSprite;
    public Character GetCopyOfPreset()
    {
        return new(characterPreset);
    }
}
