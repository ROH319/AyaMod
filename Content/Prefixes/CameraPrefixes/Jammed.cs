using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Jammed() : BaseCameraPrefix(focusSpeedMult:1.15f,stunMult:0.9f,valueMult:0.9f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
