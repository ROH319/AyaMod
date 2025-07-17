using AyaMod.Core;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AyaMod.Content.Items.Lens
{
    public abstract class BaseLens : ModItem
    {
        public override string Texture => AssetDirectory.Lens + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateInventory(Player player)
        {
            var mplr = player.GetModPlayer<CameraPlayer>();
            if (mplr.CurrentLens == null)
                mplr.CurrentLens = (ILens)this;
        }
    }
}
