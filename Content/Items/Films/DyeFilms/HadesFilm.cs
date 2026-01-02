using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class HadesFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3038;
        public override float EffectChance => 20;
        public static float ChainedExplosionChance = 20;
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
                bool devEffect = Main.player[projectile.Projectile.owner].DevEffect();
                Vector2 pos = Vector2.Lerp(projectile.Projectile.Center, target.Center, 0.5f);
                int damage = (int)(damageDone * 0.7f);
                Projectile.NewProjectileDirect(projectile.Projectile.GetSource_FromAI(), pos, Vector2.Zero,
                    ProjectileType<HadesExplosion>(), damage, projectile.Projectile.knockBack, projectile.Projectile.owner,devEffect ? 1 : 0);
            }
        }

    }

    public class HadesExplosion : ModProjectile
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
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = AyaUtils.RandAngle;
        }
        public override void AI()
        {
            if(Projectile.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                Projectile.soundDelay = -1;
            }
            Projectile.FrameLooping(frameRate, Main.projFrames[Type]);

            bool num205 = WorldGen.SolidTile(Framing.GetTileSafely((int)Projectile.position.X / 16, (int)Projectile.position.Y / 16));
            Dust dust22 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex);
            //dust22.position = Projectile.Center;
            dust22.velocity = Vector2.Zero;
            dust22.noGravity = true;
            if (num205)
                dust22.noLight = true;

        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] > 0 && Main.rand.Next(100) < HadesFilm.ChainedExplosionChance)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.Next(0, 100);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero,
                    Type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0]);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int sizeY = texture.Height / Main.projFrames[Type];
            int frameY = Projectile.frame * sizeY;
            Rectangle rec = new(0, frameY, texture.Width, sizeY);
            Vector2 origin = rec.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rec, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, 0);

            return false;
        }
    }
}
