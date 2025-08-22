using AyaMod.Core.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.ModPlayers
{
    public partial class AyaPlayer
    {
        public bool[] Effects { get; set; } = new bool[PlayerEffectLoader.EffectCount];

        public void ResetAyaEffects() 
        {
            if(Effects.Length != PlayerEffectLoader.EffectCount)
                Effects = new bool[PlayerEffectLoader.EffectCount];

            for (int i = 0; i < Effects.Length; i++)
                Effects[i] = false;
        }

        public bool HasEffect(string effectName)
        {
            if(PlayerEffectLoader.Effects.TryGetValue(effectName, out var index))
                return Effects.IndexInRange(index) && Effects[index];
            
            return false;
        }

        public bool AddEffect(string effectName)
        {
            if(PlayerEffectLoader.Effects.TryGetValue(effectName,out var index))
            {
                Effects[index] = true;
                return true;
            }
            return false;
        }
    }
}
