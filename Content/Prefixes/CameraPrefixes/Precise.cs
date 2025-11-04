using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Precise() : BaseCameraPrefix(damageMult:1.4f,sizeMult:0.8f,valueMult:1.75f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
