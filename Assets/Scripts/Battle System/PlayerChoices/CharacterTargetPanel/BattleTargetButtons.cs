using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleTargetButtons : MonoBehaviour
{
    public BattleCharacters activeBattleTarget;
    public TextMeshProUGUI targetName;
    private Button _button;
    private UnityAction<BattleCharacters> _onClickAction;
    void Start()
    {
        targetName = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetupButton(BattleCharacters character, UnityAction<BattleCharacters> OnClickAction)
    {
        if(_button == null)
        {
            _button = GetComponent<Button>();
        }
        activeBattleTarget = character;
        targetName.text = character.CharacterData.characterName;
        _onClickAction = OnClickAction;
    }
    public void Press()
    {
        _onClickAction?.Invoke(activeBattleTarget);
    }
}
