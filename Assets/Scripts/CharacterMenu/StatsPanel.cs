using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour
{
    public CharacterStatsInfoPanel[] CharactersPanel;
    public GameObject[] CharactersPanelButtons;
    private List<Character> _characters;
    [SerializeField] private Character _selectedCharacter;

    public Button weaponButton;
    public Button armorButton;
    private void Start()
    {
        weaponButton.onClick.AddListener(UnequipWeapon);
        armorButton.onClick.AddListener(UnequipArmor);
    }
    private void OnEnable()
    {
        _characters = PlayerDataHandler.Singleton.currentPlayerData.characters;

        RefreshData();
        OpenCharacterPanel(0);
    }
    private void RefreshData()
    {
        foreach (var button in CharactersPanelButtons)
            button.gameObject.SetActive(false);

        for (int i = 0; i < _characters.Count; i++)
        {
            CharactersPanelButtons[i].gameObject.SetActive(true);
            CharactersPanel[i].SetupCharacterData(_characters[i]);
        }
    }
    public void OpenCharacterPanel(int index)
    {
        _selectedCharacter = _characters[index];
        for (int i = 0; i < _characters.Count; i++)
        {
            CharactersPanel[i].gameObject.SetActive(false);
        }

        CharactersPanel[index].gameObject.SetActive(true);
        RefreshInteractables();
    }
    private void UnequipArmor()
    {
        Debug.Log("Unequip armor " + _selectedCharacter.stats.equippedArmorName);
        AudioManager.instance.PlaySFX(4);
        _selectedCharacter.stats.UnequipArmor();
        RefreshInteractables();
        RefreshData();
    }
    private void UnequipWeapon()
    {
        Debug.Log("Unequip weapon " + _selectedCharacter.stats.equippedWeaponName);
        AudioManager.instance.PlaySFX(4);
        _selectedCharacter.stats.UnequipWeapon();
        RefreshInteractables();
        RefreshData();
    }
    private void RefreshInteractables()
    {
        weaponButton.interactable = _selectedCharacter.stats.equippedWeaponName != "None";
        armorButton.interactable = _selectedCharacter.stats.equippedArmorName != "None";
    }

}
