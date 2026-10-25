using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUtils
{

    [System.Serializable]
    public class EnemyData
    {
        public CharacterDataPreset Enemy;
        public BattleRewardData rewards;
    }

    [System.Serializable]
    public class BattleRewardData
    {
        public int rewardXP;
        public ItemData[] rewardItems;
        public int scrapsReward;
    }
}
