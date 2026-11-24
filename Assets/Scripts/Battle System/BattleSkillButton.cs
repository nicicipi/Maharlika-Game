using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BattleSystem;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleSkillButton : MonoBehaviour
{
    private BattleSkills _battleMove;
    private Button _button;
    public TextMeshProUGUI skillNameText, skillDescText, skillCostText;
    private UnityAction<BattleSkills> _onMoveSelection;
    public void SetupBattleMove(BattleSkills move, UnityAction<BattleSkills> onMoveSelection, bool isUsable)
    {
        if (_button == null)
            _button = GetComponent<Button>();

        _button.interactable = isUsable;
        _battleMove = move;

        _onMoveSelection = onMoveSelection;
        skillNameText.text = _battleMove.skillName;
        skillDescText.text = _battleMove.skillDescription;
        skillCostText.text = _battleMove.staminaCost.ToString();

        gameObject.SetActive(true);
    }
    public void Press()
    {
        _onMoveSelection?.Invoke(_battleMove);
    }
}
