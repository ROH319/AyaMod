using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Deceptive() : BaseCameraPrefix(damageMult:0.85f,sizeMult:0.9f,stunMult:0.85f,valueMult:0.7f)
    {
        public override float RollChance(Item item)
        {
            return 0.66f;
        }
    }
}
