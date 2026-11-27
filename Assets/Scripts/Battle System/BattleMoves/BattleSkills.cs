using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace BattleSystem
{
    [CreateAssetMenu(fileName = "New Battle Move", menuName = "Battle System/Battle Skill")]
    public class BattleSkills : ScriptableObject
    {
        public string skillName;
        public string skillDescription;
        public int staminaCost;
        public float skillDuration = 2f;
        public TargetSelection targetSelection;
        [Tooltip("Make sure to right click and create new instance of the Action")]
        public List<BattleAction> actions = new();
    }
    [Serializable]
    public class BattleAction
    {
        [SerializeReference, SubclassSelector] public Action action = new();
    }
    public enum TargetSelection
    {
        Self,
        SingleEnemy,
        RandomEnemy,
        AllEnemies,
        SingleAlly,
        RandomAlly,
        AllAllies
    }
    [Serializable]
    public class Action
    {
        public float actionDuration = 1f;
        public GameObject actionVFX;
        public virtual IEnumerator ApplyActionToTarget(BattleCharacters source,  BattleCharacters target)
        {
            Debug.LogError("ApplyActionToTarget not implemented in derived class");
            yield return new WaitForSeconds(actionDuration);
        }
    }
    [Serializable]
    public class DamageAction : Action
    {
        public int damageValue;
        public override IEnumerator ApplyActionToTarget(BattleCharacters source, BattleCharacters target)
        {
            // if source has multiplier, apply it to damageValue
            int finalDamage = damageValue;
            if (source != null) 
            { 
                finalDamage += source.CharacterData.stats.weaponPower + source.CharacterData.stats.attack;
            }
            target.DisplayDamageText(target.CharacterData.stats.TakeDamageWithMitigation(finalDamage));
            target.TakeDamage();
            Debug.Log($"{source.CharacterData.characterName} attacks {target.CharacterData.characterName} for {finalDamage} damage");
            yield return new WaitForSeconds(actionDuration);
        }
    }
    [Serializable]
    public class HealAction : Action
    {
        public bool percentageBased;
        public int healValue;
        public override IEnumerator ApplyActionToTarget(BattleCharacters source, BattleCharacters target)
        {
            // if source has multiplier, apply it to healValue
            int finalHeal = healValue;
            if (target != null && percentageBased)
            {
                finalHeal = (int)( target.CharacterData.stats.maxHP / healValue);
            }
            target.CharacterData.stats.ModifyHP(finalHeal);
            target.TakeHealing();
            target.DisplayHealText(finalHeal);
            yield return new WaitForSeconds(actionDuration);
        }
    }
    [Serializable]
    public class StatusEffectAction : Action
    {
        public int turnCount;
        public StatusEffectData statusEffect;
        public override IEnumerator ApplyActionToTarget(BattleCharacters source, BattleCharacters target)
        {
            target.AddStatusEffect(statusEffect,turnCount);
            yield return new WaitForSeconds(actionDuration);
        }
    }
}
