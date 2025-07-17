using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes
{
    public class Peerless() : BaseCameraPrefix(damageMult:1.15f,focusSpeedMult:0.9f,5,sizeMult:1.1f,stunMult:1.15f,1.75f)
    {
        public override float RollChance(Item item)
        {
            return 0.9f;
        }
    }
}
