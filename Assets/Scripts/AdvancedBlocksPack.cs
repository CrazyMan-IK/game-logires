using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Utils;
using CrazyGames.Logires.UI;

namespace CrazyGames.Logires
{
    [CreateAssetMenu(fileName = "AdvancedPack", menuName = "Logires/Advanced BlocksPack", order = 1001)]
    public class AdvancedBlocksPack : BlocksPack
    {
        public override bool IsActivated()
        {
#if UNITY_EDITOR
            return true;
#else
            var endTime = EncryptedGlobalPreferences.GetPrimitive("advanced_set_end_time", DateTime.Now);       

            return DateTime.Now < endTime || base.IsActivated();
#endif
        }
    }
}