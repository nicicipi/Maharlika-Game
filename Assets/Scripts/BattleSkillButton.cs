using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleSkillButton : MonoBehaviour
{
    public string skillName, skillDesc;
    public int skillCost;

    public TextMeshProUGUI skillNameText, skillDescText, skillCostText;

    public void Press()
    {
        if(BattleManager.instance.GetCurrentActiveCharacter().currentSP >= skillCost)
        {
            BattleManager.instance.skillChoicePanel.SetActive(false);
            BattleManager.instance.OpenTargetMenu(skillName);
            BattleManager.instance.GetCurrentActiveCharacter().currentSP -= skillCost;
        }

        else
        {
            BattleManager.instance.battleNotice.SetText("You do not have enough stamina!");
            BattleManager.instance.battleNotice.Activate();

            BattleManager.instance.skillChoicePanel.SetActive(false);
        }
    }
}
