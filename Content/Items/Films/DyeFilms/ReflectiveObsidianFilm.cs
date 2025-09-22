using AyaMod.Content.Buffs.Films;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class ReflectiveObsidianFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3554;

        public override void PostClearProjectile(BaseCameraProj projectile, int capturecount)
        {
            projectile.player.AddBuff(BuffType<ReflectiveObsidianBuff>(), 2);
        }

        public static void SpawnObsidianShard(Player player, ref Player.HurtInfo hurtInfo)
        {
            int count = 6;
            int damage = (int)(hurtInfo.Damage * 0.75f);
            for (int i = 0; i < count; i++)
            {
                float speed = Main.rand.NextFloat(6, 12);
                Vector2 vel = Main.rand.NextVector2Unit() * speed;
                int type = ProjectileType<ObsidianShard>();
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, vel, type, damage, 7f, player.whoAmI);
            }
        }

        public static int DefenseBonus = 20;
        public static int DefenseBonusDev = 24;
        public static int DRBonus = 20;
        public static int DRBonusDev = 24;
        public static int thornBonus = 200;
        public static int thornBonusDev = 350;
    }

    public class ObsidianShard : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(4, 15);
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 28;
            Projectile.friendly = true;
            
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(3);
        }
        public override void AI()
        {

            Projectile.Chase(1500, 28, 0.05f);
            Projectile.rotation = Projectile.velocity.ToRotation();

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle rect = new Rectangle(24 * Projectile.frame, 0, 24, 28);

            float scale = Projectile.scale;
            for(int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float factor = i / (float)Projectile.oldPos.Length;
                Vector2 trailpos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                float rot = Projectile.oldRot[i] + MathHelper.PiOver4;
                Color trailColor = Color.White * Projectile.Opacity * Utils.Remap(factor, 0, 1f, 1f, 0f);
                Main.spriteBatch.Draw(texture, trailpos, rect, trailColor, rot, rect.Size() / 2, scale, 0, 0);
            }

            RenderHelper.DrawBloom(6, 4, texture, Projectile.Center - Main.screenPosition, rect, Color.DarkViolet.AdditiveColor() * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver4, rect.Size() / 2, scale);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, rect, Color.White * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver4, rect.Size() / 2, scale, 0, 0);

            return false;
        }
    }
}
