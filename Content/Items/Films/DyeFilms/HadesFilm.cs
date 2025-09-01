using AyaMod.Core;
using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class HadesFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";

        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
    }

}
