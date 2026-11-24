using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class CombatManager
    {
        private List<BattleCharacters> _playerCharacters;
        private List<BattleCharacters> _enemyCharacters;
        private List<BattleCharacters> _targetCharacters;
        public CombatManager(List<BattleCharacters> PlayerCharacters, List<BattleCharacters> EnemyCharacters)
        {
            _playerCharacters = PlayerCharacters;
            _enemyCharacters = EnemyCharacters;
        }
        public void ApplyCombatAction(BattleCharacters source, List<BattleCharacters> targets, List<Action> battleActions)
        {
            foreach (var action in battleActions)
            {
                foreach (var target in targets)
                {
                    action.ApplyActionToTarget(source, target);
                }
            }
        }

        private IEnumerator SelectTargetRoutine(TargetSelection targetSelectionType)
        {
            _targetCharacters = null;
            switch (targetSelectionType)
            {
                case TargetSelection.Self:
                    break;
                case TargetSelection.SingleEnemy:
                    break;
                case TargetSelection.RandomEnemy:
                    break;
                case TargetSelection.AllEnemies:
                    break;
                case TargetSelection.SingleAlly:
                    break;
                case TargetSelection.RandomAlly:
                    break;
                case TargetSelection.AllAllies:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return new WaitUntil(()=> _targetCharacters != null);
        }

        public void SetTarget()
        {

        }


    }
}
