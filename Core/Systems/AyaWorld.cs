using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace AyaMod.Core.Systems
{
    public class AyaWorld : ModSystem
    {
        public static bool downedGoblinSummoner = false;

        public override void PreWorldGen()
        {
            downedGoblinSummoner = false;
        }
        public override void LoadWorldData(TagCompound tag)
        {
            if(tag.TryGet("downedGoblinSummoner", out bool downed))
            {
                downedGoblinSummoner = downed;
            }
        }
        public override void SaveWorldData(TagCompound tag)
        {
            if(downedGoblinSummoner)
            {
                tag.Add("downedGoblinSummoner", true);
            }
        }
    }
}
