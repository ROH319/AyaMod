using AyaMod.Core;
using AyaMod.Core.Prefabs;
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

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritDamageBonus);
        public override void ModifyHitNPCFilm(BaseCameraProj projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += CritDamageBonus / 100f;
        }
        public static float CritDamageBonus = 20;
    }
}
