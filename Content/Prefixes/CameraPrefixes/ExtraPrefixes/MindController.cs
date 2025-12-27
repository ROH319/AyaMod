using AyaMod.Core.Globals;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using Terraria;
using Terraria.Localization;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class MindController() : ExtraCameraPrefix(focusSpeedMult:0.9f,critBonus:15,sizeMult:1.1f)
    {
        public override LocalizedText PrefixExtraTooltip => base.PrefixExtraTooltip.WithFormatArgs(MindBonusPerDebuff);
        public override void Camera_ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            int debuffcount = 0;
            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                if (target.buffTime[i] <= 0 || !Main.debuff[target.buffType[i]]) continue;
                debuffcount++;
            }
            modifiers.FinalDamage += MindBonusPerDebuff / 100f * debuffcount;
        }
        public static int MindBonusPerDebuff = 10;
    }
}
