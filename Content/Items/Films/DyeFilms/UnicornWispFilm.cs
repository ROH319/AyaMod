using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;
using Terraria.ID;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class UnicornWispFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 2884;
        public static float WispDmgRegen = 200 / 60;
        public static int WispDmgMax = 800;
        public static int ExtraAttackChance = 20;
        public override void OnSnap(BaseCameraProj projectile)
        {
            if(projectile.player.Aya().UnicornWispDmg > projectile.Projectile.damage)
            {
                int damage = projectile.Projectile.damage;
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2, 4);
                int type = ProjectileType<UnicornWispProjectile>();
                float devEffect = projectile.player.DevEffect() ? 1 : 0;
                var p = Projectile.NewProjectileDirect(projectile.Projectile.GetSource_FromAI(), projectile.Projectile.Center, vel, type, damage, projectile.Projectile.knockBack, projectile.Projectile.owner, devEffect);
                p.DamageType = ReporterDamage.Instance;
                projectile.player.Aya().UnicornWispDmg -= damage;
            }
        }
    }
    public class UnicornWispProjectile : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float Timer => ref Projectile.ai[0];
        public ref float DevEffect => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            Projectile.friendly = true;
        }
        public override bool? CanDamage() => Timer > 60;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (DevEffect > 0 && Main.rand.Next(100) < UnicornWispFilm.ExtraAttackChance)
            {
                int damage = (int)(damageDone * 0.5f);
                int type = ProjectileID.PrincessWeapon;
                Vector2 pos = Vector2.Lerp(Projectile.Center, target.Center, 0.5f);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, type, damage, Projectile.knockBack, Projectile.owner);
            }
        }
        public override void AI()
        {
            Timer++;
            if (Timer > 60)
            {
                Projectile.Chase(2500, 25f, 0.05f);
            }


            for (int num508 = 0; num508 < 3; num508++)
            {
                float num509 = Projectile.velocity.X * 0.2f * num508;
                float num510 = (0f - Projectile.velocity.Y * 0.2f) * num508;
                int num511 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.SpectreStaff, 0f, 0f, 100, default, 1.3f);
                Main.dust[num511].noGravity = true;
                Dust dust2 = Main.dust[num511];
                dust2.velocity *= 0f;
                Main.dust[num511].position.X -= num509;
                Main.dust[num511].position.Y -= num510;
            }

            for (int num508 = 0; num508 < 2; num508++)
            {
                float num509 = Projectile.velocity.X * 0.2f * num508;
                float num510 = (0f - Projectile.velocity.Y * 0.2f) * num508;
                int num511 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Gastropod, 0f, 0f, 100, default(Color), 1.3f);
                Main.dust[num511].noGravity = true;
                Dust dust2 = Main.dust[num511];
                dust2.velocity *= 0f;
                Main.dust[num511].position.X -= num509;
                Main.dust[num511].position.Y -= num510;
            }
        }
    }
}
