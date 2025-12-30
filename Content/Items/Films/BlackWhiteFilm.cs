using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria.ID;
using Terraria;
using System.Collections.Generic;
using Humanizer;

namespace AyaMod.Content.Items.Films
{
    public class BlackWhiteFilm : BaseFilm
    {
        public static int ArmorPenBonus = 7;
        public static int ArmorPenBonusDev = 9;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(copper:3));
            FilmArgs = [("armorpen", ArmorPenBonus.ToString(), ArmorPenBonusDev.ToString())];
        }
        public override void ModifyHitNPCFilm(BaseCameraProj projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += projectile.player.DevEffect() ? ArmorPenBonusDev : ArmorPenBonus;
        }
        public override void AddRecipes()
        {
            CreateRecipe(450)
                .AddIngredient<CameraFilm>(450)
                .AddIngredient(ItemID.SilverDye)
                .AddIngredient(ItemID.BlackDye)
                .AddTile(TileID.DyeVat)
                .Register();
        }
    }
}
