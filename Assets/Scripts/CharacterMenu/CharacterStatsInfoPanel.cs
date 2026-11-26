using BattleSystem;
using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsInfoPanel : MonoBehaviour
{
    [SerializeField] private Image _characterImage;
    [SerializeField] private TextMeshProUGUI _characterName;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _HP;
    [SerializeField] private TextMeshProUGUI _SP;
    [SerializeField] private TextMeshProUGUI _Attack;
    [SerializeField] private TextMeshProUGUI _Defence;
    [SerializeField] private TextMeshProUGUI _Weapon;
    [SerializeField] private TextMeshProUGUI _WeaponPower;
    [SerializeField] private TextMeshProUGUI _Armor;
    [SerializeField] private TextMeshProUGUI _ArmorPower;

    public void SetupCharacterData(Character characterData)
    {
        if (characterData == null || characterData.stats == null)
            return;

        CharacterStats stats = characterData.stats;

        _characterImage.sprite = CharacterReferenceConfig.Singleton.GetReference(characterData.characterName).dialogueSprite;

        _characterName.text = characterData.characterName;
        _level.text = $"Lv. {stats.playerLevel}";

        _HP.text = $"{stats.currentHP}/{stats.maxHP}";
        _SP.text = $"{stats.currentStamina}/{stats.maxStamina}";

        _Attack.text = stats.attack.ToString();
        _Defence.text = stats.defence.ToString();

        _Weapon.text = stats.equippedWeaponName;
        _WeaponPower.text = stats.weaponPower.ToString();

        _Armor.text = stats.equippedArmorName;
        _ArmorPower.text = stats.armorDefence.ToString();
    }
}