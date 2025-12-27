using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    [ProjectileEffect]
    public class Festive() : ExtraCameraPrefix(focusSpeedMult:1.1f,sizeMult:1.1f)
    {
        public override void GlobalProjectile_Spawn(Projectile projectile, IEntitySource source)
        {
            if (ProjectileID.Sets.CultistIsResistantTo[projectile.type]) return;
            if (projectile.hostile) return;
            BaseCameraProj camera = null;
            if (!projectile.CameraSourcedProj(out camera)) return;
            if (projectile.ModProjectile != null && !ProjectileLoader.ShouldUpdatePosition(projectile)) return;
            projectile.AddEffect<Festive>();
        }
        public override void Camera_PostAI(Player player, BaseCameraProj projectile)
        {
            if (projectile.Projectile.HasEffect<Festive>())
            {
                projectile.Projectile.GetGlobalProjectile<AyaGlobalProjectile>().SpeedModifier -= 0.2f;
            }
        }
        public override void Camera_OnSnap(BaseCameraProj projectile)
        {
            var mplr = projectile.player.GetModPlayer<FestivePlayer>();
            mplr.FestiveCounter++;
            if (mplr.FestiveCounter >= 5)
            {
                float damageMult = 1.6f;
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.hostile || proj.owner != projectile.player.whoAmI) continue;
                    if (proj.ModProjectile != null && proj.ModProjectile.CanDamage().HasValue && !(bool)proj.ModProjectile.CanDamage()) continue;
                    if (projectile.Projectile == proj) continue;
                    int width = proj.width;
                    int height = proj.height;
                    Projectile.NewProjectileDirect(proj.GetSource_FromThis(), proj.Center, Vector2.Zero, ModContent.ProjectileType<FestiveExplosion>(), (int)(proj.damage * damageMult), 0f, proj.owner, width, height, (proj.whoAmI % 3) * 2);
                    //Main.NewText("!!!");
                    proj.Kill();
                }
                mplr.FestiveCounter = 0;
            }
        }
    }
    public class FestiveExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10;
            Projectile.SetImmune(-1);
        }
        public override void OnSpawn(IEntitySource source)
        {
            int width = (int)(Projectile.ai[0] * 3f);
            int height = (int)(Projectile.ai[1] * 3f);
            Vector2 center = Projectile.Center;
            Projectile.width = width;
            Projectile.height = height;
            Projectile.Center = center;
            Projectile.timeLeft = (int)Projectile.ai[2];
            //for (int i = 0; i < 20; i++)
            //{
            //    Vector2 vel = Main.rand.NextVector2Circular(5f, 5f);
            //    int dusttype = Main.rand.NextFromList(DustID.Firework_Blue,DustID.Firework_Green,DustID.Firework_Pink,DustID.Firework_Red,DustID.Firework_Yellow);
            //    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dusttype, vel.X, vel.Y, 150, default, 1.5f);
            //    Main.dust[dust].noGravity = true;
            //}
        }
        public override void OnKill(int timeLeft)
        {
            int width = Projectile.width;
            int height = Projectile.height;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14 with { MaxInstances = 20, Pitch = 0.5f }, Projectile.Center);
            {
                for (int k = 0; k < 4; k++)
                {
                    Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), width, height, 31, 0f, 0f, 100, default(Color), 1.5f);
                }

                for (int l = 0; l < 20; l++)
                {
                    int dusttype = Main.rand.NextFromList(DustID.Firework_Blue, DustID.Firework_Green, DustID.Firework_Pink, DustID.Firework_Red, DustID.Firework_Yellow);
                    int num5 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), width, height, dusttype, 0f, 0f, 200, default(Color), 1.2f);
                    Main.dust[num5].noGravity = true;
                    Main.dust[num5].velocity *= 3f;
                    num5 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), width, height, dusttype, 0f, 0f, 200, default(Color), 0.5f);
                    Main.dust[num5].velocity *= 1.2f;
                    Main.dust[num5].noGravity = true;
                }

                for (int m = 0; m < 1; m++)
                {
                    int num6 = Gore.NewGore(Projectile.GetSource_Death(), Projectile.position + new Vector2((float)(width * Main.rand.Next(100)) / 100f, (float)(height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num6].velocity *= 0.3f;
                    Main.gore[num6].velocity.X += (float)Main.rand.Next(-10, 11) * 0.05f;
                    Main.gore[num6].velocity.Y += (float)Main.rand.Next(-10, 11) * 0.05f;
                }
            }

        }
    }
    public class FestivePlayer : ModPlayer
    {
        public int FestiveCounter = 0;
    }
}
