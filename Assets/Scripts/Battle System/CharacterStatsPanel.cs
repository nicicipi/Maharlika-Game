using SaveSystem;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsPanel : MonoBehaviour
{
    public CharacterStatView[] characterStatViews;
    public void SetupCharacters(List<Character> characters)
    {
        foreach(var statView in characterStatViews)
        {
            statView.gameObject.SetActive(false);
        }

        for(int i = 0; i < characters.Count; i++)
        {
            characterStatViews[i].gameObject.SetActive(true);
            characterStatViews[i].SetupCharacter(characters[i]);
            characterStatViews[i].UpdateValues();
        }
    }
    public void UpdateValues()
    {
        foreach (var statView in characterStatViews)
        {
            statView.UpdateValues();
        }
    }
}
