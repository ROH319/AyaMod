using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Focused() : BaseCameraPrefix(damageMult:1.45f,focusSpeedMult:1.1f,sizeMult:1.15f,valueMult:1.65f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
