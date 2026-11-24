using BattleSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillChoicePanel : MonoBehaviour
{
    //depending on the move, this wont open
    public CharacterTargetPanel targetPanel;
    [SerializeField,Header("Skill Setup")] private BattleSkillButton skillButtonPrefab;
    [SerializeField] private List<BattleSkillButton> skillButtonList = new();
    [SerializeField] private Transform skillButtonParent;
    
    private BattleCharacters _currentSource;
    private UnityAction<BattleMoves, List<BattleCharacters>> _onSelectCallback;
    public void SetupChoices(BattleCharacters source, UnityAction<BattleMoves, List<BattleCharacters>> onSelectCallback)
    {
        _onSelectCallback = onSelectCallback;
        _currentSource = source;
        PopulateMoveList(source.battleMovesList);
        gameObject.SetActive(true);
    }

    private void PopulateMoveList(List<BattleMoves> moves)
    {
        foreach (var buttons in skillButtonList)
            buttons.gameObject.SetActive(false);

        foreach (var move in moves)
        {
            BattleSkillButton skillButton = GetSkillButton();
            bool isUsable = _currentSource.CharacterData.stats.currentStamina >= move.staminaCost;
            skillButton.SetupBattleMove(move, OnSelectMove, isUsable);
        }
    }
    private void OnSelectMove(BattleMoves move)
    {
        List<BattleCharacters> targettableCharacters = BattleManager.instance.GetTargettableCharacters(_currentSource, move.targetSelection);
        switch (move.targetSelection)
        {
            case TargetSelection.SingleEnemy:
                targetPanel.SetupTargets(targettableCharacters, OnSelectSingleTargetForSkill);
                break;
            case TargetSelection.SingleAlly:
                targetPanel.SetupTargets(targettableCharacters, OnSelectSingleTargetForSkill);
                break;
            default:
                _onSelectCallback?.Invoke(move, targettableCharacters);
                break;
        }

        void OnSelectSingleTargetForSkill(BattleCharacters character)
        {
            _onSelectCallback?.Invoke(move, new() { character });
            targetPanel.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private BattleSkillButton GetSkillButton()
    {
        foreach( var button in skillButtonList)
        {
            if(!button.gameObject.activeSelf)
                return button;
        }
        BattleSkillButton newButton = Instantiate(skillButtonPrefab, skillButtonParent);
        skillButtonList.Add(newButton);

        return newButton;
    }
}
