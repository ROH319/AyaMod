using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.Loaders
{
    public class AyaKeybindLoader : ILoader
    {
        public static ModKeybind UltraMove;
        public static ModKeybind SpecialMove;

        public void Load(Mod mod, Type type)
        {
            if (Main.dedServ)
                return;

            UltraMove = KeybindLoader.RegisterKeybind(mod, "UltraMove", Microsoft.Xna.Framework.Input.Keys.LeftControl);
            SpecialMove = KeybindLoader.RegisterKeybind(mod, "SpecialMove", Microsoft.Xna.Framework.Input.Keys.V);
        }

        public void Unload(Mod mod, Type type)
        {
            UltraMove = null;
            SpecialMove = null;
        }
    }
}
