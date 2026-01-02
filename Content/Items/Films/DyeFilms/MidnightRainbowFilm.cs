using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class MidnightRainbowFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3556;
        public override float EffectChance => 20;
        public override void OnSnap(BaseCameraProj projectile)
        {
            if (!CheckEffect(projectile.player)) return;

            int maxShot = 4;
            int shot = 0;
            float range = 300;
            int damage = (int)(projectile.Projectile.damage * 0.5f);
            foreach(var npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy() || npc.Distance(projectile.Projectile.Center) > range) continue;

                SpawnRainbow(projectile, npc.Center, damage);
                shot++;
                if (shot >= maxShot) break;
            }
            if (shot < maxShot)
            {
                int left = maxShot - shot;
                for (int i = 0; i < left; i++)
                {
                    Vector2 pos = projectile.Projectile.Center + AyaUtils.RandAngle.ToRotationVector2() * range;
                    SpawnRainbow(projectile, pos, damage);
                }
            }
        }
        public void SpawnRainbow(BaseCameraProj projectile, Vector2 targetPos, int damage)
        {
            int type = 644;
            float hue = Main.rgbToHsl(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB)).X;
            Projectile.NewProjectileDirect(projectile.Projectile.GetSource_FromAI(), targetPos, Vector2.Zero, type, damage, projectile.Projectile.knockBack, projectile.player.whoAmI, hue, projectile.Projectile.whoAmI);
        }
    }
}
