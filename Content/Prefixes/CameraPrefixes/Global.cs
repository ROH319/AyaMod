using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Global() : BaseCameraPrefix(damageMult:1.05f,focusSpeedMult:1.05f, critBonus:5,sizeMult:1.3f,valueMult:1.75f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
