using AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace AyaMod.Core.ItemDropRules
{
    public class NecromanticGhostDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && !info.npc.GetGlobalNPC<GhostNerfed>().nerfed;

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => Language.GetTextValue("Mods.AyaMod.Conditions.NecromanticGhost");
    }
}
