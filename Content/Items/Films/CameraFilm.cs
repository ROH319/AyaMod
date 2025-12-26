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
    public class CameraFilm : BaseFilm
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus);
        public static int DamageBonus = 10;
        public override StatModifier DamageModifier => base.DamageModifier + DamageBonus / 100f;
        public override void AddRecipes()
        {
            CreateRecipe(150)
                .AddIngredient(ItemID.SilverOre)
                .AddIngredient(ItemID.Gel, 2)
                .AddTile(TileID.Furnaces)
                .Register();
            CreateRecipe(150)
                .AddIngredient(ItemID.TungstenOre)
                .AddIngredient(ItemID.Gel, 2)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
