using BattleSystem;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun", menuName = "Battle System/Status Effects/Stun")]
public class StatusEffect_Stun : StatusEffectData
{
    public override void ApplyStatusEffect(BattleCharacters target)
    {
        NotificationPanel.Singleton.ShowNotification($"{target.CharacterData.characterName} is stunned!");
    }
    public override bool OnTurnStart(BattleCharacters target)
    {
        target.ForceEndTurn();
        return true;
    }
}
