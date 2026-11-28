using BattleSystem;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenceAdjustments", menuName = "Battle System/Status Effects/DefenceAdjustments")]
public class StatusEffect_DefenceAdjustments : StatusEffectData
{
    public bool percent;
    public int DefenceAmount;
    public Color color = new Color(0.443f, 0.663f, 0.749f);
    public string decription;

    public override void ApplyStatusEffect(BattleCharacters target)
    {
        NotificationPanel.Singleton.ShowNotification($"{target.CharacterName} " + decription);
        target.FlickerColor(color, duration, 2);
        if (percent)
            target.temporaryPercentageStats.defence += DefenceAmount;
        else
            target.temporaryFlatStats.defence += DefenceAmount;

    }

    public override bool OnTurnStart(BattleCharacters target)
    {
        NotificationPanel.Singleton.ShowNotification($"{target.CharacterName} " + decription);
        target.FlickerColor(color, duration, 2);
        return true;
    }

    public override bool RemoveStatusEffect(BattleCharacters target)
    {
        if (percent)
            target.temporaryPercentageStats.defence -= DefenceAmount;
        else
            target.temporaryFlatStats.defence -= DefenceAmount;
        return true;
    }
}
