
using BattleSystem;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Battle System/Skill Library")]
public class BattleSkillLibrary : ScriptableObject
{
    public BattleSkills defaultMove;
    [SerializeField] private BattleSkills[] battleSkillList;

    public void LoadAllBattleSkills()
    {
        battleSkillList = Resources.LoadAll<BattleSkills>("Skills");
    }
    public BattleSkills GetBattleMoveByName(string moveName)
    {
        foreach (var move in battleSkillList)
        {
            if (move.skillName == moveName)
                return move;
        }
        return null;
    }
    public List<BattleSkills> GetBattleMoves(List<string> names)
    {
        List<BattleSkills> moves = new();

        foreach (var moveName in names)
        {
            Debug.Log($"Searching for move: {moveName}");
            BattleSkills move = GetBattleMoveByName(moveName);

            if (move != null)
                moves.Add(move);
        }

        return moves;
    }
}