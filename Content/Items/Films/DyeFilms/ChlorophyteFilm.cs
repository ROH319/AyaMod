using AyaMod.Content.Buffs.Films;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class ChlorophyteFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FlourshingDotDmg / 2);

        public override int DyeID => 2883;

        public override float EffectChance => 20;
        public static int FlourshingDotDmg = 80;
        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CheckEffect(projectile.player))
            {
                target.AddBuff(BuffType<FlourishingPoisonBuff>(), 2 * 60);
            }
        }

        public static void SpawnSpore(NPC npc)
        {
            if(Main.GameUpdateCount % 30 == 0)
            {
                int count = 4;
                for (int i = 0; i < count; i++)
                {
                    int damage = 50;
                    Vector2 vel = npc.velocity.RotateRandom(0.2f) * 0.8f;
                    if (npc.velocity.Length() < 3f)
                    {
                        vel += Main.rand.NextVector2Unit() * Main.rand.NextFloat(3, 5) * 0.8f;
                    }
                    int type = ProjectileType<SporeCloud>();

                    Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, vel, type, damage, 3f, Main.myPlayer);
                }
            }
        }

    }

    public class SporeCloud : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Cloud";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height =108;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }
        public override void AI()
        {
            //Projectile.FrameLooping(6, 5);

            Projectile.rotation += 0.005f;
            if (Projectile.Opacity > 0.5f) Projectile.velocity *= 0.96f;
            else Projectile.velocity *= 0.98f;
            Projectile.Opacity -= 0.008f;
            if (Projectile.Opacity < 0.088f) Projectile.Kill();

            {
                
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 128,Scale:0.5f);
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Color drawcolor = new Color(20, 211, 49).AdditiveColor() * Projectile.Opacity * 0.5f;
            float scale = Projectile.scale;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, drawcolor, Projectile.rotation, texture.Size() / 2, scale, 0, 0);

            //int width = texture.Width;
            //int heightTotal = texture.Height;
            //int height = heightTotal / 5;
            //Rectangle rect = new Rectangle(0, height * Projectile.frame, width, height);
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, rect, lightColor * Projectile.Opacity, Projectile.rotation, rect.Size() / 2, Projectile.scale, 0, 0);


            return false;
        }
    }
}
