using AyaMod.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.Conditions
{
    public static class AyaConditions
    {
        public static Condition DownedGoblinSummoner = new Condition("Mods.AyaMod.Conditions.DownedGoblinSummoner", () => AyaWorld.downedGoblinSummoner);
    }
}
