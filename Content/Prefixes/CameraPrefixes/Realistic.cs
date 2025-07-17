using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Realistic() : BaseCameraPrefix(damageMult:1.1f,stunMult:1.1f,valueMult:1.26f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
