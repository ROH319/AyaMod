using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using Terraria.Localization;
using AyaMod.Core;
using Terraria.ID;

namespace AyaMod.Content.Items.Accessories
{
    public class PHDJ : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageIncrease, CritIncrease);
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Lime7, Item.sellPrice(gold: 6));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ReporterDamage>() += (float)DamageIncrease / 100f;
            player.GetCritChance<ReporterDamage>() += CritIncrease;
        }

        public static readonly int DamageIncrease = 15;
        public static readonly int CritIncrease = 10;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ReporterEmblem>())
                .AddIngredient(ModContent.ItemType<ReporterBadge>())
                .AddIngredient(ItemID.EyeoftheGolem)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
