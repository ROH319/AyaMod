using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    [ProjectileEffect]
    public class Bloodthirsty() : ExtraCameraPrefix(damageMult: 1.1f, focusSpeedMult: 0.95f, valueMult: 1.5f)
    {
        public override LocalizedText PrefixExtraTooltip => base.PrefixExtraTooltip.WithFormatArgs(BloodSnapCost);
        public override void GlobalProjectile_Spawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.hostile) return;
            BaseCameraProj camera = null;
            //如果它自己是拍摄框弹幕
            if (projectile.TryGetCameraProj(out BaseCameraProj c))
                camera = c;
            //如果它的生成源是拍摄框弹幕
            else if (source is EntitySource_Parent parent && parent.Entity is Projectile parentProj
                && parentProj.active && parentProj.TryGetCameraProj(out BaseCameraProj ca))
                camera = ca;

            if (camera == null) return;

            projectile.AddEffect<Bloodthirsty>();
        }
        public override void GlobalProjectile_OnHit(Projectile projectile, NPC target, NPC.HitInfo info, int damageDone)
        {
            if (target.life > 0 || target.lifeMax == 5 || target.friendly) return;
            if (projectile.HasEffect<Bloodthirsty>())
            {
                Item.NewItem(target.GetSource_Death(), target.getRect(), ItemID.Heart);
            }
        }
        public override void Camera_OnSnap(BaseCameraProj projectile)
        {
            Player player = projectile.player;
            player.statLife -= BloodSnapCost;
            CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.DamagedFriendly, $"-{BloodSnapCost}", false, false);
        }

        public static int BloodSnapCost = 4;
    }
}
