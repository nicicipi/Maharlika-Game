using System;
using System.Collections;
using UnityEngine;

namespace BattleSystem
{
    public class StatusEffectData : ScriptableObject
    {
        public string effectName;
        public string description;
        public Sprite icon;
        public float duration = 1f;
        public GameObject statusEffectVFX;
        public virtual void ApplyStatusEffect(BattleCharacters target)
        {
        }
        public virtual bool OnTurnStart(BattleCharacters target) 
        {
            return false;
        }
        public virtual bool OnTurnEnd(BattleCharacters target)
        {
            return false;
        }
        public virtual bool RemoveStatusEffect(BattleCharacters target) 
        {
            return false;
        }
    }

    [System.Serializable]
    public class ActiveStatusEffect
    {
        public StatusEffectData data;
        public int remainingDuration;

        public ActiveStatusEffect(StatusEffectData data, int duration)
        {
            this.data = data;
            this.remainingDuration = duration;
        }
        public void ReduceDuration()
        {
            remainingDuration--;
        }
    }

}