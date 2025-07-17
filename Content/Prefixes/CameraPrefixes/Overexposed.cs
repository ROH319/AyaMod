using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Overexposed() : BaseCameraPrefix(critBonus:-10,sizeMult:0.9f,valueMult:0.76f)
    {
        public override float RollChance(Item item)
        {
            return 0.66f;
        }
    }
}
