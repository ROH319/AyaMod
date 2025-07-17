using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class PixieFilm : BaseDyeFilm
    {
        public override void ModifyHitNPCFilm(BaseCameraProj projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            
            float critDamageBonus = 0.2f;
            modifiers.CritDamage += critDamageBonus;
        }
    }
}
