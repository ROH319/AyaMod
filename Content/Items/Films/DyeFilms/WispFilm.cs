using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;
using Terraria.ID;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class WispFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 2878;
        public override void OnSnapInSight(BaseCameraProj projectile)
        {
            if(projectile.player.Aya().WispDmg > projectile.Projectile.damage)
            {
                int damage = projectile.Projectile.damage;
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2, 4);
                Projectile.NewProjectileDirect(projectile.Projectile.GetSource_FromAI(), projectile.Projectile.Center, vel, ProjectileID.SpectreWrath, damage, projectile.Projectile.knockBack, projectile.Projectile.owner, -1);

                projectile.player.Aya().WispDmg -= projectile.Projectile.damage;

                
            }
        }

        public static float WispDmgRegen = 80f / 60;
        public static float WispDmgRegenDev = 125 / 60;
        public static int WispDmgMax = 320;
        public static int WispDmgMaxDev = 500;
    }
}
