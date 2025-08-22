using AyaMod.Core.Attributes;
using ReLogic.Utilities;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Core.Loaders
{
    public class PlayerEffectLoader : ILoader
    {
        public static Dictionary<string, int> effects = new Dictionary<string, int>();

        public static int EffectCount { get; private set; }
        public static FrozenDictionary<string, int> Effects { get; private set; }

        public void SetUp(Mod mod, Type type)
        {
            PlayerEffectAttribute attribute = type.GetAttribute<PlayerEffectAttribute>();
            if(attribute != null)
            {
                string name = type.Name;
                if(!string.IsNullOrEmpty(attribute.OverrideEffectName))
                    name = attribute.OverrideEffectName;
                effects.Add(name, EffectCount);
                EffectCount++;

                if(attribute.ExtraEffectNames != null)
                    foreach(var name2 in attribute.ExtraEffectNames)
                    {
                        effects.Add(name2, EffectCount);
                        EffectCount++;
                    }
            }
        }

        public void PostSetUp(Mod mod)
        {
            Effects = effects.ToFrozenDictionary();
            //effects = null;
        }

        public void PreUnload(Mod mod)
        {
            Effects = null;
        }
    }
}
