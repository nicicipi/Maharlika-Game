using BattleSystem;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun", menuName = "Battle System/Status Effects/Stun")]
public class StatusEffect_Stun : StatusEffectData
{
    public override void ApplyStatusEffect(BattleCharacters target)
    {
        NotificationPanel.Singleton.ShowNotification($"{target.CharacterData.characterName} is stunned!");
        target.FlickerColor(new Color(1f, 1f, 1f, 0.25f), duration, 3);
    }
    public override bool OnTurnStart(BattleCharacters target)
    {
        NotificationPanel.Singleton.ShowNotification($"{target.CharacterData.characterName} is stunned! Skipping turn!");
        target.ForceEndTurn();
        target.SpawnVFX(statusEffectVFX, 0f);
        target.FlickerColor(new Color(1f, 1f, 1f, 0.25f), duration, 3);
        return true;
    }
}
