using AyaMod.Content.Items.Lens;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.Loaders
{
    public class LensLoader : ILoader
    {
        private static Dictionary<string, ILens> lens = new();
        public static FrozenDictionary<string, ILens> Lens { get; private set; }

        public void SetUp(Mod mod, Type type)
        {
            if (Main.dedServ) return;

            if (type == typeof(ILens)) return;

            Type[] i = type.GetInterfaces();
            if (!i.Contains(typeof(ILens))) return;

            string name = type.Name;
            if(!lens.ContainsKey(name))
                lens.Add(name, (ILens)Activator.CreateInstance(type));
        }

        public static ILens GetLens(string key)
        {
            if(!lens.ContainsKey(key)) return null;
            return lens[key];
        }
        public static ILens GetLens<T>() where T : ILens
        {
            return GetLens(typeof(T).Name);
        }

        public void PostSetUp(Mod mod)
        {
            Lens = lens.ToFrozenDictionary();
        }
        public void PreUnload(Mod mod)
        {
            Lens = null;
        }
    }
}
