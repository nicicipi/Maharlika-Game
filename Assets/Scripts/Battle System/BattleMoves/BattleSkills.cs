using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace BattleSystem
{
    [CreateAssetMenu(fileName = "New Battle Move", menuName = "Battle System/Battle Skill")]
    public class BattleSkills : ScriptableObject
    {
        public MoveType moveType;
        public string skillName;
        public string skillDescription;
        public int staminaCost;
        public float skillDuration = 2f;
        public TargetSelection targetSelection;
        [Tooltip("Make sure to right click and create new instance of the Action")]
        public List<BattleAction> actions = new();
    }
    public enum MoveType
    {
        Attack,
        Buff,
        Debuff,
        Status,
        Block,
        Explode
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
    public class RecoverStamina : Action
    {
        public int value;
        public override IEnumerator ApplyActionToTarget(BattleCharacters source, BattleCharacters target)
        {
            source.ModifySP(value);
            BattleManager.instance._characterStatsPanel.UpdateSPValues();
            yield return null;
        }
    }
    [Serializable]
    public class DamagSelfAction : Action
    {
        public int damageValue;
        public override IEnumerator ApplyActionToTarget(BattleCharacters source, BattleCharacters target)
        {
            // if source has multiplier, apply it to damageValue
            int finalDamage = damageValue;
            if (source != null) 
            { 
                finalDamage += source.WeaponPower + source.Attack;
            }
            source.SpawnVFX(actionVFX, 1.2f);
            source.SpawnVFX(actionVFX, 1.2f);
            source.SpawnVFX(actionVFX, 1.2f);
            source.TakeDamage(finalDamage, Color.red);
            Debug.Log($"{source.CharacterName} attacks {target.CharacterName} for {finalDamage} damage");
            yield return new WaitForSeconds(actionDuration);
        }
    }
    [Serializable]
    public class ScaleFlicker : Action
    {
        public float targetScaleMultiplier;
        public bool source = true;
        public override IEnumerator ApplyActionToTarget(BattleCharacters source, BattleCharacters target)
        {
            yield return ScaleRoutine(this.source ? source : target);
        }

        private IEnumerator ScaleRoutine(BattleCharacters source)
        {
            float elapsed = 0f;
            Vector3 originalScale = source.isPlayer ? Vector3.one * 2f : Vector3.one;
            Vector3 enlargedScale = originalScale * targetScaleMultiplier;
            while (elapsed < actionDuration)
            {
                float t = elapsed / actionDuration;

                float scaleT = t < 0.5f ? t * 2f : (1f - t) * 2f;
                source.transform.localScale = Vector3.Lerp(originalScale, enlargedScale, scaleT);

                elapsed += Time.deltaTime;
                yield return null;
            }
            source.transform.localScale = originalScale;
        }
    }
    [Serializable]
    public class NotificationAction : Action
    {
        public string notificationText;
        public Color color;
        public override IEnumerator ApplyActionToTarget(BattleCharacters source, BattleCharacters target)
        {
            NotificationPanel.Singleton.ShowNotification(notificationText);
            source.FlickerColor(color, actionDuration, 2);
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
                finalDamage += source.WeaponPower + source.Attack;
            }
            target.SpawnVFX(actionVFX, 1.2f);
            target.TakeDamage(finalDamage, Color.red);
            Debug.Log($"{source.CharacterName} attacks {target.CharacterName} for {finalDamage} damage");
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
                finalHeal = (int)( target.GetHPValues().max / healValue);
            }
            target.SpawnVFX(actionVFX, 0f);
            target.TakeHealing(finalHeal);
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

    [Serializable]
    public class AddBlock : Action
    {
        public int blockAmount;
        public override IEnumerator ApplyActionToTarget(BattleCharacters source, BattleCharacters target)
        {
            target.FlickerColor(new Color(0.25f, 0.25f, 1f, 1f), actionDuration, 3);
            target.SpawnVFX(actionVFX, 0f);
            target.ModifyBlock(blockAmount);
            yield return new WaitForSeconds(actionDuration);
        }
    }
}
