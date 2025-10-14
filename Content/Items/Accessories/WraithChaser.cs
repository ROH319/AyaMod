using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using AyaMod.Helpers;
using Terraria.Localization;
using AyaMod.Core.ModPlayers;
using Terraria.ID;

namespace AyaMod.Content.Items.Accessories
{
    public class WraithChaser : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ChaseSpeedBonus);

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Lime7, Item.sellPrice(gold: 6));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Camera().ChaseSpeedModifier += ChaseSpeedBonus / 100f;
            player.Camera().SpiritSnap = true;
            player.AddEffect<TriggerShutter>();
        }

        public static int ChaseSpeedBonus = 40;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<TriggerShutter>()
                .AddIngredient<SeigaPupil>()
                .AddIngredient(ItemID.Ectoplasm, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
