using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BattleSystem;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleSkillButton : MonoBehaviour
{
    private BattleMoves _battleMove;
    private Button _button;
    public TextMeshProUGUI skillNameText, skillDescText, skillCostText;
    private UnityAction<BattleMoves> _onMoveSelection;
    public void SetupBattleMove(BattleMoves move, UnityAction<BattleMoves> onMoveSelection, bool isUsable)
    {
        if (_button == null)
            _button = GetComponent<Button>();

        _button.interactable = isUsable;
        _battleMove = move;

        _onMoveSelection = onMoveSelection;
        skillNameText.text = _battleMove.moveName;
        skillDescText.text = _battleMove.moveDescription;
        skillCostText.text = _battleMove.staminaCost.ToString();

        gameObject.SetActive(true);
    }
    public void Press()
    {
        _onMoveSelection?.Invoke(_battleMove);
    }
}
