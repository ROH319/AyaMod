using AyaMod.Common.Easer;
using AyaMod.Content.Buffs.Films;
using AyaMod.Content.Projectiles.Auras;
using AyaMod.Core;
using AyaMod.Core.Globals;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using Terraria.ID;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class MartianFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 2864;
        public static int ElectrifiedDotDmg = 300;
        public static int ElectrifiedDotDmgDev = 360;
        public static int VulnerableDamage = 10;
        public override void Load()
        {
            AyaGlobalNPC.ModifyHitHook += VulnerableHit;
        }

        public static void VulnerableHit(Terraria.NPC npc, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (npc.Aya().MartainElectrified2)
                modifiers.FinalDamage *= (1 + VulnerableDamage / 100f);
        }

        public override void OnSnap(BaseCameraProj projectile)
        {
            int bufftime = 2 * 60;
            int buffType = BuffType<MartianElectrifiedBuff>();
            Color innerColor = new Color(210, 255, 250) * 0.1f;
            Color edgeColor = new Color(62, 236, 255);
            if (projectile.player.DevEffect())
            {
                buffType = BuffType<MartianElectrifiedBuff2>();
                innerColor = new Color(248, 255, 186) * 0.15f;
                edgeColor = new Color(104, 56, 8);
            }

            float radius = MathF.Max(200, projectile.size * 2f);
            int auratime = 60;
            var aura = BaseBuffAura.Spawn<AuraFriendly>(projectile.Projectile.GetSource_FromAI(), projectile.Projectile.Center, auratime, buffType, bufftime, radius, innerColor, edgeColor, projectile.player.whoAmI);
            aura.DisableRadiusFadeout();
            aura.SetRadiusFadein(0.4f, Ease.OutCubic);
            aura.SetAlphaFadeout(0.5f, Ease.OutSine);
            aura.SetDust(DustID.Electric, 2, 3, 1f, 0.8f, 1f);
            aura.DistortIntensity = 4f;
        }
    }
}
