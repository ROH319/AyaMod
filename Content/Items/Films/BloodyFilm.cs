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
        public override void OnSnap(BaseCameraProj projectile)
        {
            int healamount = 1;
            if (projectile.player.DevEffect()) healamount = 2;
            projectile.player.Heal(healamount);
        }
        public override void OnClearProjectile(BaseCameraProj projectile)
        {
            int healamount = 4;
            if (projectile.player.DevEffect()) healamount = 5;
            projectile.player.Heal(healamount);
        }
    }
}
