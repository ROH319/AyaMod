using AyaMod.Core;
using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Accessories
{
    public class ReporterBadge : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageIncrease, CritIncrease);

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(gold: 6));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ReporterDamage>() += (float)DamageIncrease / 100f;
            player.GetCritChance<ReporterDamage>() += CritIncrease;
        }

        public static readonly int DamageIncrease = 7;
        public static readonly int CritIncrease = 7;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ReporterCard>())
                .AddIngredient(ItemID.SoulofMight)
                .AddIngredient(ItemID.SoulofFright)
                .AddIngredient(ItemID.SoulofSight)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
