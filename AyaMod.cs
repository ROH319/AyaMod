global using Microsoft.Xna.Framework;
global using Terraria.ModLoader;
global using static Terraria.ModLoader.ModContent;
using AyaMod.Compat.ImproveGame;
using AyaMod.Core;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

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
			Main.QueueMainThreadAction(RenderHelper.CreateRender);
            Terraria.Main.OnResolutionChanged += Main_OnResolutionChanged;
        }

        private void Main_OnResolutionChanged(Vector2 obj)
        {
            Main.QueueMainThreadAction(RenderHelper.CreateRender);
        }

        public override void PostSetupContent()
        {
			ImproveGameCalls.CallImproveGame();
            base.PostSetupContent();
        }

        public override void Unload()
        {
			Instance = null;
			AssetDirectory.UnloadAsset();
            Terraria.Main.OnResolutionChanged -= Main_OnResolutionChanged;

        }
    }
}
