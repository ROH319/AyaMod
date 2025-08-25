using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.Localization;
using Terraria;
using AyaMod.Helpers;

namespace AyaMod.Content.Items.Accessories
{
    public class CrimsonPupil : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SizeDecrease);

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(gold: 1));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Camera().SizeBonus -= (float)SizeDecrease / 100f;
        }

        public static int SizeDecrease = 12;
    }
}
