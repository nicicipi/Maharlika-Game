using BattleSystem;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackAdjustments", menuName = "Battle System/Status Effects/AttackAdjustments")]
public class StatusEffect_AttackAdjustments : StatusEffectData
{
    public bool percent;
    public int AttackAmount;
    public Color color = new Color(0.439f, 0.749f, 0.337f);
    public string decription;
    public override void ApplyStatusEffect(BattleCharacters target)
    {
        NotificationPanel.Singleton.ShowNotification($"{target.CharacterName} " + decription);
        target.FlickerColor(color, duration, 1);
        if(percent)
            target.temporaryPercentageStats.attack += AttackAmount;
        else
            target.temporaryFlatStats.attack += AttackAmount;
    }

    public override bool OnTurnStart(BattleCharacters target)
    {
        NotificationPanel.Singleton.ShowNotification($"{target.CharacterName} " + decription);
        target.FlickerColor(color, duration, 1);
        return true;
    }

    public override bool RemoveStatusEffect(BattleCharacters target)
    {
        if(percent)
            target.temporaryPercentageStats.attack -= AttackAmount;
        else
            target.temporaryFlatStats.attack -= AttackAmount;
        return true;
    }
}
