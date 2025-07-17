using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Global() : BaseCameraPrefix(critBonus:5,sizeMult:1.15f,valueMult:1.1f)
    {
        public override float RollChance(Item item)
        {
            return 0.66f;
        }
    }
}
