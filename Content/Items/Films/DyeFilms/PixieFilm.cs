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

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class PixieFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 2879;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritDmgBonus);
        public override LocalizedText DevTooltip => base.DevTooltip.WithFormatArgs(CritDmgBonusDev);
        public static float CritDmgBonus = 20;
        public static float CritDmgBonusDev = 30;
        public override void SetDefaults()
        {
            base.SetDefaults();
            FilmArgs = [("critdmg", CritDmgBonus.ToString(), CritDmgBonusDev.ToString())];
        }
        public override void ModifyHitNPCFilm(BaseCameraProj projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            bool deveffect = Main.player[projectile.Projectile.owner].DevEffect();
            modifiers.CritDamage += (deveffect ? CritDmgBonusDev : CritDmgBonus) / 100f;
        }
    }
}
