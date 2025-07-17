using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Blurred() : BaseCameraPrefix(damageMult:0.85f,critBonus:-10,valueMult:0.85f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
