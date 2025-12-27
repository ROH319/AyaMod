using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using Terraria.Localization;
using AyaMod.Helpers;
using AyaMod.Core;
using Terraria.ID;

namespace AyaMod.Content.Items.Accessories
{
    public class CorruptPupil : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SpeedIncrease, SizeIncrease);

        public static int SpeedIncrease = 8;
        public static int SizeIncrease = 8;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(gold: 1));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed<ReporterDamage>() += SpeedIncrease / 100f;
            player.Camera().SizeModifier += SizeIncrease / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShadowScale, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
