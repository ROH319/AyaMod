using AyaMod.Common.Easer;
using AyaMod.Content.Projectiles.Auras;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;
using Terraria.ID;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class GelFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3561;
        public override void OnSnap(BaseCameraProj projectile)
        {
            if (CheckEffect(projectile.player))
            {
                Color innerColor = new Color(146,210,225) * 0.1f;
                Color edgeColor = new Color(42, 73, 152) * 1f;
                float radius = MathHelper.Clamp(projectile.size * 1.5f, 0, 200f);
                var aura = BaseBuffAura.Spawn<GelAura>(projectile.Projectile.GetSource_FromAI(), projectile.Projectile.Center,
                    3 * 60, 0, 0, radius, innerColor, edgeColor, projectile.Projectile.owner);
                aura.DisableRadiusFadeout();
                aura.SetRadiusFadein(0.4f, Ease.OutCubic);
                aura.SetAlphaFadeout(0.5f, Ease.OutSine);
                aura.SetSlowFactor(Main.player[projectile.Projectile.owner].DevEffect() ? SlowFactorDev : SlowFactor);
                aura.SetDust(56, 4, 16, 1f, 0.8f, 1f);
                aura.DistortIntensity = 20f;

                //bool deveffect = Main.player[projectile.Projectile.owner].DevEffect();
                //Projectile.NewProjectileDirect(projectile.Projectile.GetSource_FromAI(),projectile.Projectile.Center,Vector2.Zero,
                //    ProjectileType<StickyGel>(),0,projectile.Projectile.knockBack,projectile.Projectile.owner, deveffect ? 1 : 0);
            }

        }
        public override float EffectChance => 30;
        public static int SlowFactor = 20;
        public static int SlowFactorDev = 30;
    }
    public class GelAura : BaseBuffAura
    {
        public float SlowFactor;
        public void SetSlowFactor(float factor)
        {
            SlowFactor = factor;
        }
        public override void ApplyBuff()
        {
            foreach(var npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(Projectile, true)) continue;
                if (npc.Distance(Projectile.Center) > Radius * 0.45f) continue;

                npc.Aya().SpeedModifier -= SlowFactor;
            }
        }
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
