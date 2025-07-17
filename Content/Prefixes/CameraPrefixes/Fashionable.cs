using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Fashionable() : BaseCameraPrefix(damageMult:1.1f,focusSpeedMult:0.95f,stunMult:1.05f,valueMult:1.35f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
