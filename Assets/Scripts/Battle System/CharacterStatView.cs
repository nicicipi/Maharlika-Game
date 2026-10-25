using SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatView : MonoBehaviour
{
    public TextMeshProUGUI CharacterName;
    [Header("HP")]
    public TextMeshProUGUI HPText;
    public Slider HPSlider;
    [Header("SP")]
    public TextMeshProUGUI SPText;
    public Slider SPSlider;
    private Character _character;
    public void SetupCharacter(Character character)
    {
        _character = character;
    }
    public void UpdateValues()
    {
        if (_character == null || !gameObject.activeSelf)
        {
            return;
        }
        CharacterName.SetText(_character.characterName);
        HPText.SetText($"HP: {GetStatValue(_character.stats.currentHP, _character.stats.maxHP)}");
        HPSlider.maxValue = _character.stats.maxHP;
        HPSlider.value = _character.stats.currentHP;
        SPText.SetText($"HP: {GetStatValue(_character.stats.currentStamina, _character.stats.maxStamina)}");
        SPSlider.maxValue = _character.stats.maxStamina;
        SPSlider.value = _character.stats.currentStamina;
    }
    public void ClearReferences()
    {
        _character = null;
    }

    public string GetStatValue(int current, int max)
    {
        return $"{current}/{max}";
    }
}
