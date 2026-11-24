using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace BattleSystem
{
    [CreateAssetMenu(fileName = "New Battle Move", menuName = "Battle Move")]
    public class BattleMoves : ScriptableObject
    {
        public string moveName;
        public string moveDescription;
        public int staminaCost;
        public float moveDuration = 2f;
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
            target.StartCoroutine(HealAnimation(target.transform, 0.5f));
            target.DisplayHealText(finalHeal);
            target.RefreshSlider();
        }

        private IEnumerator HealAnimation(Transform transform, float duration)
        {
            Vector3 originalScale = transform.localScale;
            Color originalColor = transform.GetComponent<SpriteRenderer>().color;
            Color healColor = Color.green;

            SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();

            Vector3 squishScale = new(originalScale.x * 1.5f, originalScale.y * 0.8f, originalScale.z);
            Vector3 stretchScale = new(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;

                if (t < 0.5f)
                    transform.localScale = Vector3.Lerp(squishScale, stretchScale, t * 2f);
                else
                    transform.localScale = Vector3.Lerp(stretchScale, originalScale, (t - 0.5f) * 2f);

                spriteRenderer.color = Color.Lerp(healColor, originalColor, t);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = originalScale;
            spriteRenderer.color = originalColor;
        }
    }

}
