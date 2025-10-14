using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace AyaMod.Common.ItemDropRules
{
    public class DownedSkeletron : IItemDropRuleCondition
    {
        bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info)
        {
            if (!info.IsInSimulation)
                return NPC.downedBoss3;
            return false;
        }

        bool IItemDropRuleCondition.CanShowItemDropInUI() => true;

        string IProvideItemConditionDescription.GetConditionDescription() => AyaUtils.GetText("DropConditions.PostSkeletron");
    }
}
