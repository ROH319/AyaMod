using AyaMod.Core.Attributes;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Films
{
    public class AdvancedCameraFilm : BaseFilm
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus);
        public static int DamageBonus = 15;
        public override StatModifier DamageModifier => base.DamageModifier + DamageBonus / 100f;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(copper: 6));
        }
        public override void AddRecipes()
        {
            CreateRecipe(200)
                .AddIngredient(ItemID.MythrilOre)
                .AddIngredient(ItemID.Gel, 3)
                .AddTile(TileID.AdamantiteForge)
                .Register();
            CreateRecipe(200)
                .AddIngredient(ItemID.OrichalcumOre)
                .AddIngredient(ItemID.Gel, 3)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }
    }
}
