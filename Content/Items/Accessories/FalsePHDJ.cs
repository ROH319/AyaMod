using AyaMod.Core;
using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using AyaMod.Core.Attributes;
using AyaMod.Helpers;

namespace AyaMod.Content.Items.Accessories
{
    [PlayerEffect]
    public class FalsePHDJ : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageIncrease, CritIncrease, HurtIncrease);
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Lime7, Item.sellPrice(gold: 6));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ReporterDamage>() += (float)DamageIncrease / 100f;
            player.GetCritChance<ReporterDamage>() += CritIncrease;
            var b = player.AddEffect<FalsePHDJ>();
        }

        public static int DamageIncrease = 15;
        public static int CritIncrease = 10;
        public static int HurtIncrease = 10;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ReporterEmblem>())
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
