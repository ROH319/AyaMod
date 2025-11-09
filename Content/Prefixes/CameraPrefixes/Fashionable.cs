using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Fashionable() : BaseCameraPrefix(damageMult:1.05f,focusSpeedMult:0.9f,critBonus: 15,sizeMult:1.1f, stunMult:1.2f,valueMult:1.75f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
