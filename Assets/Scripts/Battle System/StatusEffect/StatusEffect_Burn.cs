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
            target.ApplyVFX(statusEffectVFX, 1.1f);
            target.ApplyVFX(statusEffectVFX, 1.1f);
            target.ApplyVFX(statusEffectVFX, 1.1f);
        }
        target.DisplayDamageText(target.CharacterData.stats.TakeDamageWithMitigation(damagePerTurn));
        target.RefreshSlider();
        target.FlickerColor(new Color(1f, 0.5f, 0f, 1f), duration, 3);

    }
}
