using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace AyaMod.Content.Items.Films
{
    public class CameraFilm : BaseFilm
    {
        public override void AddRecipes()
        {
            CreateRecipe(150)
                .AddIngredient(ItemID.SilverOre)
                .AddIngredient(ItemID.Gel, 5)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
