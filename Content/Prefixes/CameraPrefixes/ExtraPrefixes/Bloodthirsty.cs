using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public override void Load()
        {
            GlobalCamera.SnapHook += BloodSnap;
            AyaGlobalProjectile.OnProjectileHitNPC += OnKillNPC;
            AyaGlobalProjectile.OnProjectileSpawn += OnSpawn;
        }

        public static void OnSpawn(Projectile projectile, IEntitySource source)
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
            if (camera.player.TryGetHeldModItem(out ModItem moditem) && moditem is BaseCamera && moditem.Item.prefix == PrefixType<Bloodthirsty>())
            {
                projectile.AddEffect<Bloodthirsty>();
            }
        }

        public static void OnKillNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life > 0 || target.lifeMax == 5 || target.friendly) return;
            if (projectile.HasEffect<Bloodthirsty>())
            {
                Item.NewItem(target.GetSource_Death(), target.getRect(), ItemID.Heart);
            }
        }

        public static void BloodSnap(Core.Prefabs.BaseCameraProj projectile)
        {
            if(projectile.player.TryGetHeldModItem(out ModItem moditem) && moditem is BaseCamera && moditem.Item.prefix == PrefixType<Bloodthirsty>())
            {
                Player player = projectile.player;
                player.statLife -= BloodSnapCost;
                CombatText.NewText(new Rectangle((int)player.position.X,(int)player.position.Y,player.width,player.height), CombatText.DamagedFriendly, $"-{BloodSnapCost}", false, false);
            }
        }

        public static int BloodSnapCost = 4;
    }
}
