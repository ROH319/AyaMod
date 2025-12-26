using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace AyaMod.Content.Items.Films
{
    public class DoomFilm : BaseFilm
    {
        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool ichor = WorldGen.crimson;
            bool curse = !WorldGen.crimson;
            if (projectile.player.DevEffect())
                ichor = curse = true;
            if (ichor)
                target.AddBuff(BuffID.Ichor, 5 * 60);
            if (curse)
                target.AddBuff(BuffID.CursedInferno, 5 * 60);
        }
        public override void AddRecipes()
        {
            CreateRecipe(200)
                .AddIngredient<CameraFilm>(200)
                .AddIngredient(ItemID.Ichor)
                .AddTile(TileID.Anvils)
                .Register(); 
            CreateRecipe(200)
                .AddIngredient<CameraFilm>(200)
                .AddIngredient(ItemID.CursedFlame)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
