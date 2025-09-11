using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class GelFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override void OnSnapInSight(BaseCameraProj projectile)
        {
            if(Main.rand.Next(100) <= EffectChance)
            {
                bool deveffect = Main.player[projectile.Projectile.owner].DevEffect();
                Projectile.NewProjectileDirect(projectile.Projectile.GetSource_FromAI(),projectile.Projectile.Center,Vector2.Zero,
                    ProjectileType<StickyGel>(),0,projectile.Projectile.knockBack,projectile.Projectile.owner, deveffect ? 1 : 0);
            }
            
        }
        public static int EffectChance = 20;
        public static int SlowFactor = 20;
        public static int SlowFactorDev = 30;
    }

    public class StickyGel : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Films + Name;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.timeLeft = 3 * 60;
        }

        public override void AI()
        {
            foreach(var npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(Projectile, true)) continue;
                if (Projectile.Colliding(Projectile.GetHitbox(), npc.getRect()))
                {
                    npc.Aya().SpeedModifier *= (1f - (Projectile.ai[0] > 0 ? GelFilm.SlowFactorDev : GelFilm.SlowFactor) / 100f);
                }
            }
            Projectile.Opacity = Projectile.TimeleftFactor();
        }
    }
}
