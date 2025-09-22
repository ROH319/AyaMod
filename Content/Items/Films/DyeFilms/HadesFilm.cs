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
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class HadesFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3038;
        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(CheckEffect())
            {
                bool devEffect = Main.player[projectile.Projectile.owner].DevEffect();
                Projectile.NewProjectileDirect(projectile.Projectile.GetSource_FromAI(), target.Center, Vector2.Zero,
                    ProjectileType<HadesExplosion>(), projectile.Projectile.damage, projectile.Projectile.knockBack, projectile.Projectile.owner,devEffect ? 1 : 0);
            }
        }

        public override float EffectChance => 20;
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
            Projectile.timeLeft = 3 * 1 * 7;
        }

        public override void AI()
        {
            if(Projectile.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                Projectile.soundDelay = -1;
            }
            Projectile.FrameLooping(3 * 1, 7);

            bool num205 = WorldGen.SolidTile(Framing.GetTileSafely((int)Projectile.position.X / 16, (int)Projectile.position.Y / 16));
            Dust dust22 = Main.dust[Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 229)];
            dust22.position = Projectile.Center;
            dust22.velocity = Vector2.Zero;
            dust22.noGravity = true;
            if (num205)
                dust22.noLight = true;

        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] > 0 && Main.rand.Next(100) <= ModContent.GetInstance<HadesFilm>().EffectChance)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.Next(0, 100);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero,
                    Type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0]);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle rec = new Rectangle(0, 98 * Projectile.frame, 98, 98);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rec, Color.White * Projectile.Opacity, Projectile.rotation, new Vector2(49, 49), Projectile.scale, 0, 0);

            return false;
        }
    }
}
