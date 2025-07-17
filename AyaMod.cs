global using Microsoft.Xna.Framework;
global using Terraria.ModLoader;
global using static Terraria.ModLoader.ModContent;
using AyaMod.Compat.ImproveGame;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class AyaMod : Mod
	{
		private static AyaMod _instance;

		public static AyaMod Instance
		{
			get
			{
				_instance ??= (AyaMod)ModLoader.GetMod("AyaMod");
				return _instance;
			}
			set
			{
				_instance = value;
			}
		}

        public override void Load()
        {
            Instance = this;
			AssetDirectory.LoadAsset();
        }

        public override void PostSetupContent()
        {
			ImproveGameCalls.CallImproveGame();
            base.PostSetupContent();
        }

        public override void Unload()
        {
			Instance = null;
        }
    }
}
