using BattleSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Burn", menuName = "Battle System/Status Effects/Burn")]
public class StatusEffect_Burn : StatusEffectData
{
    public int damagePerTurn = 5;

    public override void ApplyStatusEffect(BattleCharacters target)
    {
        NotificationPanel.Singleton.ShowNotification($"{target.CharacterData.characterName} is burning!");
        AudioManager.instance.PlaySFX(38);
        target.SpawnVFX(statusEffectVFX);
        target.FlickerColor(new Color(1f, 0.5f, 0f, 1f), duration, 3);
    }
    public override bool OnTurnEnd(BattleCharacters target)
    {
        ApplyBurn(target);
        return true;
    }

    private void ApplyBurn(BattleCharacters target)
    {
        NotificationPanel.Singleton.ShowNotification($"{target.CharacterData.characterName} got burnt!");
        AudioManager.instance.PlaySFX(38);
        if (statusEffectVFX)
        {
            target.SpawnVFX(statusEffectVFX, 1.1f);
            target.SpawnVFX(statusEffectVFX, 1.1f);
            target.SpawnVFX(statusEffectVFX, 1.1f);
        }
        target.TakeDamage(damagePerTurn, new Color(1f, 0.5f, 0f, 1f), false);
        target.FlickerColor(new Color(1f, 0.5f, 0f, 1f), duration, 3);

    }
}
