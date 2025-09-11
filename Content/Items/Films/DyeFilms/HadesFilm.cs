using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class HadesFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";

        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(Main.rand.Next(100) <= EffectChance)
            {
                bool devEffect = Main.player[projectile.Projectile.owner].DevEffect();
                Projectile.NewProjectileDirect(projectile.Projectile.GetSource_FromAI(), target.Center, Vector2.Zero,
                    ProjectileType<HadesExplosion>(), projectile.Projectile.damage, projectile.Projectile.knockBack, projectile.Projectile.owner,devEffect ? 1 : 0);
            }
        }

        public static int EffectChance = 20;
    }

    public class HadesExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Films + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 140;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(20);
            Projectile.timeLeft = 3 * 5 * 7;
        }

        public override void AI()
        {
            Projectile.FrameLooping(3 * 5, 7);

        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] > 0 && Main.rand.Next(100) <= HadesFilm.EffectChance)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.Next(0, 100);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero,
                    Type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0]);
            }
        }
    }
}
