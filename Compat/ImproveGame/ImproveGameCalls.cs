using AyaMod.Core;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AyaMod.Compat.ImproveGame
{
    public static class ImproveGameCalls
    {
        public static void CallImproveGame()
        {
            if (ModLoader.TryGetMod("ImproveGame",out Mod qot))
            {
                string category = "AyaMod.Reporter";
                Texture2D texture = Request<Texture2D>(AssetDirectory.Accessories + "ReporterEmblem",ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                string nameKey = "Mods.AyaMod.UI.PlayerStats.Reporter";
                Texture2D modIcon = Request<Texture2D>("AyaMod/icon_small", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                qot.Call("AddStatCategory", category, texture, nameKey, modIcon);

                string damageNameKey = "Mods.ImproveGame.UI.PlayerStats.Damage";
                qot.Call("AddStat", category, damageNameKey,
                    () => BonusSyntax(ReporterDamage(),true));

                string critNameKey = "Mods.ImproveGame.UI.PlayerStats.Crit";
                qot.Call("AddStat", category, critNameKey,
                    () => BonusSyntax(ReporterCrit(), true));

                string speedNameKey = "Mods.AyaMod.UI.PlayerStats.FocusSpeed";
                qot.Call("AddStat", category, speedNameKey,
                    () => BonusSyntax(ReporterAttackSpeed(), true));

            }
        }
        public static float ReporterDamage()
        {
            var dmg = Main.LocalPlayer.GetTotalDamage<ReporterDamage>();
            return (dmg.Additive * dmg.Multiplicative - 1) * 100f;
        }

        public static float ReporterCrit() => Main.LocalPlayer.GetTotalCritChance<ReporterDamage>();

        public static float ReporterAttackSpeed() => Main.LocalPlayer.GetTotalAttackSpeed<ReporterDamage>() * 100f - 100f;

        /// <summary>
        /// 对于传入的value，以加成格式显示 (四舍五入并保留两位小数) <br/>
        /// 不带符号：value% <br/>
        /// 带符号：+value%(正值) 或 -value%(负值) 或 -0%(0) <br/>
        /// </summary>
        public static string BonusSyntax(float value, bool sign = false)
        {
            float roundedValue = MathF.Round(value, 2);
            if (!sign)
                return $"{roundedValue}%";
            return roundedValue > 0 ? $"+{roundedValue}%" : $"{roundedValue}%";
        }
    }
}
