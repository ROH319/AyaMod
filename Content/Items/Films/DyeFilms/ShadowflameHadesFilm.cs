using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class ShadowflameHadesFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3600;

        public override float EffectChance => 20;
        public override float GetTotalChance(Player player)
        {
            float totalChance = base.GetTotalChance(player);
            if (player.DevEffect()) totalChance *= 1.5f;
            return totalChance;
        }
        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CheckEffect(projectile.player))
            {
                int type = ProjectileType<ShadowflameExplosion>();
                int damage = (int)(damageDone * 0.5f);
                Vector2 pos = Vector2.Lerp(projectile.Projectile.Center, target.Center, 0.5f);
                Projectile.NewProjectileDirect(projectile.Projectile.GetSource_FromAI(), pos, Vector2.Zero, type, damage, projectile.Projectile.knockBack, projectile.Projectile.owner);
            }
        }
    }
    public class ShadowflameExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Films + Name;
        public static int frameRate = 4;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Main.projFrames[Type] * frameRate;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.SetImmune(-1);
            Projectile.scale = 1.5f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.rotation = AyaUtils.RandAngle;

            float speed = 4f;
            int type = ProjectileID.ShadowFlame;
            int dmg = (int)(Projectile.damage * 0.2f);
            for (int i = 0; i < 3; i++)
            {
                float num17 = Main.rand.Next(10, 80) * 0.001f;
                if (Main.rand.Next(2) == 0)
                    num17 *= -1f;

                float num18 = Main.rand.Next(10, 80) * 0.001f;
                if (Main.rand.Next(2) == 0)
                    num18 *= -1f;
                Vector2 vel = (MathHelper.TwoPi / 3 * i).ToRotationVector2() * speed;
                var p = Projectile.NewProjectileDirect(source, Projectile.Center, vel, type, dmg, Projectile.knockBack, Projectile.owner, num17, num18);
                p.DamageType = ReporterDamage.Instance;
            }
        }
        public override void AI()
        {
            Projectile.FrameLooping(frameRate, 7);
            if (Projectile.frame >= Main.projFrames[Type]) 
                Projectile.Kill();
            Projectile.velocity = Vector2.Zero;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int sizeY = texture.Height / Main.projFrames[Type];
            int frameY = Projectile.frame * sizeY;
            Rectangle rect = new(0, frameY, texture.Width, sizeY);
            Vector2 origin = rect.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, 0);
            return false;
        }
    }
}
