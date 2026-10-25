using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Leveltext;
    public TextMeshProUGUI HPtext;
    public Slider HPSlider;
    public TextMeshProUGUI SPtext;
    public Slider SPSlider;
    public TextMeshProUGUI XPText;
    public Slider XPSLider;
    public Image charSprite;
    private void UpdateSlider(Slider slider, float minValue, float maxValue, float currentValue, TextMeshProUGUI text)
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = currentValue;
        text.text = $"{currentValue}/{maxValue}";
    }
    public void UpdateInfo(Character character)
    {
        if (character == null || character.stats == null) return;

        Name.text = character.characterName;
        Leveltext.text = $"{character.stats.playerLevel}";
        UpdateSlider(HPSlider, 0, character.stats.maxHP, character.stats.currentHP, HPtext);
        UpdateSlider(SPSlider, 0, character.stats.maxStamina, character.stats.maxStamina, SPtext);
        UpdateSlider(XPSLider, character.stats.xpForNextLevel[character.stats.playerLevel -1], character.stats.xpForNextLevel[character.stats.playerLevel],character.stats.currentXP, XPText);
        charSprite.sprite = CharacterReferenceConfig.Singleton.GetReference(character.characterName).dialogueSprite;
    }
}
