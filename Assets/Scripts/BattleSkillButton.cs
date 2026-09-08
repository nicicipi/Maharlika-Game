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
    }
}
