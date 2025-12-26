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
    [PlayerEffect]
    public class AdvancedCameraFilm : BaseFilm
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus);
        public static int DamageBonus = 15;
        public override void Load()
        {
            AyaPlayer.ModifyWeaponDamageHook += ModifyCameraDamage;
        }
        public override void Unload()
        {
            AyaPlayer.ModifyWeaponDamageHook -= ModifyCameraDamage;
        }
        public static void ModifyCameraDamage(Player player, Item item, ref StatModifier modifier)
        {
            if (!player.HasEffect<AdvancedCameraFilm>()) return;
            modifier *= 1 + DamageBonus / 100f;
        }
        public override void PreAI(BaseCameraProj projectile)
        {
            projectile.player.AddEffect<AdvancedCameraFilm>();
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
