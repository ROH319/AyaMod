using AyaMod.Core.Prefabs;
using Terraria;
using Terraria.ID;
namespace AyaMod.Content.Items.Films
{
    public class MiasmaFilm : BaseFilm
    {
        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 3 * 60);
        }
    }
}
