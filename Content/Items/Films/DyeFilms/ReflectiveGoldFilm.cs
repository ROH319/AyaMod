﻿using AyaMod.Content.Buffs.Films;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using Terraria.Localization;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class ReflectiveGoldFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DefenceBonusMin, DefenceBonusMax, DRBonusMin, DRBonusMax);        public override int DyeID => 3027;

        public override void PostClearProjectile(BaseCameraProj projectile, int capturecount)
        {
            projectile.player.AddBuff(BuffType<ReflectiveGoldBuff>(), capturecount);
        }

        public static int DefenceBonusMin = 12;
        public static int DefenceBonusDevMin = 18;
        public static int DefenceBonusMax = 18;
        public static int DefenceBonusDevMax = 24;
        public static int DRBonusMin = 12;
        public static int DRBonusDevMin = 18;
        public static int DRBonusMax = 18;
        public static int DRBonusDevMax = 24;
    }
}
