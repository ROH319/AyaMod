using AyaMod.Content.Buffs.Films;
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
using Terraria.ModLoader;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class GrimFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 3598;
        public static int KillLine = 15;
        public static int KillLineDev = 20;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(KillLine);
        public override LocalizedText DevTooltip => base.DevTooltip.WithFormatArgs(KillLineDev);
        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            int bufftime = projectile.player.DevEffect() ? 4 * 60 : 2 * 60;
            target.AddBuff(BuffType<ScaredBuff>(), bufftime);
        }
    }
}
