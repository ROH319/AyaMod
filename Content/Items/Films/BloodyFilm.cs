using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;

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
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(copper: 3));
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
