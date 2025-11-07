using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Dazzling() : BaseCameraPrefix(damageMult:1.20f,focusSpeedMult:1.1f,critBonus:15, stunMult:2f,valueMult:1.65f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
