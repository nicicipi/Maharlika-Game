using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Called by Skill choice, Attack CHoice, and Consumable choice panels.
public class CharacterTargetPanel : MonoBehaviour
{
    public BattleTargetButtons[] targetButtons;
    private UnityAction<BattleCharacters> _onClickAction;
    public void SetupTargets(List<BattleCharacters> targetOption, UnityAction<BattleCharacters> OnClickAction)
    {
        gameObject.SetActive(true);
        Debug.Log(targetOption.Count);

        _onClickAction = OnClickAction;
        foreach (var buttons in targetButtons)
        {
            buttons.gameObject.SetActive(false);
        }
        int index = 0;
        for (int i = 0; i < targetOption.Count; i++)
        {
            index = i;
            targetButtons[index].SetupButton(targetOption[index], OnClickCallback); 
            targetButtons[index].gameObject.SetActive(true);
        }
    }

    private void OnClickCallback(BattleCharacters selectedCharacter)
    {
        _onClickAction?.Invoke(selectedCharacter);
        gameObject.SetActive(false);
    }
}
