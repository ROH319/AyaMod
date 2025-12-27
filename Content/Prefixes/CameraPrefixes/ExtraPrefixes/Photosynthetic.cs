using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class Photosynthetic() : ExtraCameraPrefix(damageMult:1.1f,focusSpeedMult:0.9f,critBonus:5,stunMult:1.15f)
    {
        public override LocalizedText PrefixExtraTooltip => base.PrefixExtraTooltip.WithFormatArgs(SizeBonusMax, AttackSpeedBonusMax);
        public static int SizeBonusMax = 20;
        public static int AttackSpeedBonusMax = 15;
        public static int MaxDuration = 5 * 60;
        public override void Camera_PostAI(Player player, BaseCameraProj projectile)
        {
            Point pos = player.Center.ToTileCoordinates();
            if (pos.X > 0 && pos.X > 0 && pos.X < Main.maxTilesX && pos.Y < Main.maxTilesY)
            {
                float lightStrength = Lighting.GetColor(pos).ToVector3().Length();
                float ratio = lightStrength / 1.732f;
                if (ratio > 0.5f)
                    player.GetModPlayer<PhotosyntheticPlayer>().PhotosyntheticTimer++;
                else
                    player.GetModPlayer<PhotosyntheticPlayer>().PhotosyntheticTimer--;
            }
        }
    }

    public class PhotosyntheticPlayer : ModPlayer
    {
        public float PhotosyntheticTimer;
        public override void PostUpdateMiscEffects()
        {
            Player.Camera().SizeModifier += Utils.Remap(PhotosyntheticTimer, 0, Photosynthetic.MaxDuration, 0, Photosynthetic.SizeBonusMax);
            Player.GetAttackSpeed<ReporterDamage>() += Utils.Remap(PhotosyntheticTimer, 0, Photosynthetic.MaxDuration, 0, Photosynthetic.AttackSpeedBonusMax);
        }
    }
}
