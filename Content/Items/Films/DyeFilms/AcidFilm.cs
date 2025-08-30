using AyaMod.Content.Buffs;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class AcidFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AcidBuff>(), 60);
        }
    }
}
