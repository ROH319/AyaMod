using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Ephemeral() : BaseCameraPrefix(focusSpeedMult:0.85f,stunMult:1.05f,valueMult:1.22f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
