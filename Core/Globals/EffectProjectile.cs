using AyaMod.Core.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.Globals
{
    public partial class AyaGlobalProjectile
    {
        public bool[] Effects { get; set; } = new bool[ProjectileEffectLoader.EffectCount];

        public bool HasEffect(string effectName)
        {
            if(ProjectileEffectLoader.Effects.TryGetValue(effectName, out int index))
            {
                return Effects.IndexInRange(index) && Effects[index];
            }
            return false;
        }

        public bool AddEffect(string effectName)
        {
            if (ProjectileEffectLoader.Effects.TryGetValue(effectName, out int index))
            {
                Effects[index] = true;
                return true;
            }
            return false;
        }
    }
}
