using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Content.Items.Films
{
    public class BloodyFilm : BaseFilm
    {
        public static int HitHeal = 1;
        public static int HitHealDev = 2;
        public static int SnapProjHeal = 4;
        public static int SnapProjHealDev = 5;
        public override void SetDefaults()
        {
            base.SetDefaults();
            FilmArgs = [("hitheal", HitHeal.ToString(), HitHealDev.ToString()), 
                ("projheal", SnapProjHeal.ToString(), SnapProjHealDev.ToString())];
        }
        public override void OnSnap(BaseCameraProj projectile)
        {
            int healamount = projectile.player.DevEffect() ? HitHealDev : HitHeal;
            projectile.player.Heal(healamount);
        }
        public override void OnClearProjectile(BaseCameraProj projectile)
        {
            int healamount = projectile.player.DevEffect() ? SnapProjHealDev : SnapProjHeal;
            projectile.player.Heal(healamount);
        }
    }
}
