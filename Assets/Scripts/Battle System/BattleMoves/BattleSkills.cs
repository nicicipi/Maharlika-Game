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
        public virtual void ApplyActionToTarget(BattleCharacters source,  BattleCharacters target)
        {
            Debug.LogError("ApplyActionToTarget not implemented in derived class");
        }
    }
    [Serializable]
    public class DamageAction : Action
    {
        public int damageValue;
        public override void ApplyActionToTarget(BattleCharacters source, BattleCharacters target)
        {
            // if source has multiplier, apply it to damageValue
            int finalDamage = damageValue;
            if (source != null) 
            { 
                finalDamage += source.CharacterData.stats.weaponPower + source.CharacterData.stats.attack;
            }
            target.CharacterData.stats.ModifyHP(-finalDamage);
            target.TakeDamage();
            target.DisplayDamageText(finalDamage);
            Debug.Log($"{source.CharacterData.characterName} attacks {target.CharacterData.characterName} for {finalDamage} damage");
        }
    }
    [Serializable]
    public class HealAction : Action
    {
        public bool percentageBased;
        public int healValue;
        public override void ApplyActionToTarget(BattleCharacters source, BattleCharacters target)
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
            target.RefreshSlider();
        }
    }
    [Serializable]
    public class StunAction : Action
    {
        public float chance = 0.5f;
        public override void ApplyActionToTarget(BattleCharacters source, BattleCharacters target)
        {
           // target.applystun();
           //
        }
    }

}
