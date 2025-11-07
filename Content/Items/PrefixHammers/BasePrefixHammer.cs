using AyaMod.Content.Prefixes.CameraPrefixes;
using AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes;
using AyaMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.PrefixHammers
{
    public abstract class BasePrefixHammer : ModItem
    {
        public override string Texture => AssetDirectory.PrefixHammers + Name;

        public virtual ModPrefix PrefixToForge { get; set; }

        public override void SetDefaults()
        {
            Item.width = Item.height = 28;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Green;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
        }

        /// <summary>
        /// 锻锤“重铸”时触发，原方法里会将物品SetDefault并赋予<see cref="PrefixToForge"/>
        /// </summary>
        /// <param name="item"></param>
        public virtual void OnReforge(Item item)
        {
            item.SetDefaults(item.type);
            item.Prefix((int)(PrefixToForge?.Type));
            SoundEngine.PlaySound(SoundID.Item37);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if(PrefixToForge == null) return;
            if(PrefixToForge is BaseCameraPrefix cameraPrefix)
            {
                Color color = new Color(120, 190, 120) * (Main.mouseTextColor / 255f);
                Color badColor = new Color(190,120,120) * (Main.mouseTextColor / 255f);

                string prefixName = Language.GetTextValue($"Mods.{Mod.Name}.Prefixes.{cameraPrefix.Name}.DisplayName");
                tooltips.Add(new TooltipLine(Mod, "PrefixHammer: PrefixName", prefixName)
                {
                    OverrideColor = Main.DiscoColor
                });

                if (cameraPrefix.damageMult != 1)
                {
                    float damageStat = MathF.Round((cameraPrefix.damageMult - 1f) * 100, 2);
                    string damageText = (damageStat > 0 ? "+" : "") + $"{damageStat}" + Lang.tip[39].Value;
                    tooltips.Add(new TooltipLine(Mod, "PrefixHammer: PrefixDamage", damageText)
                    {
                        OverrideColor = damageStat > 0 ? color : badColor
                    });
                }

                if (cameraPrefix.focusSpeedMult != 1)
                {
                    float focusSpeedStat = MathF.Round((1f - cameraPrefix.focusSpeedMult) * 100, 2);
                    string value = (focusSpeedStat > 0 ? "+" : "") + $"{focusSpeedStat}";
                    string speedText = BaseCameraPrefix.PrefixFocusSpeed.WithFormatArgs(value).Value;
                    tooltips.Add(new TooltipLine(Mod, "PrefixHammer: PrefixSpeed", speedText)
                    {
                        OverrideColor = focusSpeedStat > 0 ? color : badColor,
                    });
                }

                if(cameraPrefix.critBonus != 0)
                {
                    int critStat = cameraPrefix.critBonus;
                    string critText = (critStat > 0 ? "+" : "") + $"{critStat}" + Lang.tip[41].Value;
                    tooltips.Add(new TooltipLine(Mod, "PrefixHammer: PrefixCrit", critText)
                    {
                        OverrideColor = critStat > 0 ? color : badColor
                    });
                }

                if (cameraPrefix.sizeMult != 1)
                {
                    float sizeStat = MathF.Round((cameraPrefix.sizeMult - 1f) * 100, 2);
                    string value = (sizeStat > 0 ? "+" : "") + $"{sizeStat}";
                    string sizeText = BaseCameraPrefix.SizeTooltip.WithFormatArgs(value).Value;
                    tooltips.Add(new TooltipLine(Mod, "PrefixHammer: PrefixSize", sizeText)
                    {
                        OverrideColor = sizeStat > 0 ? color : badColor
                    });
                }

                if (cameraPrefix.stunMult != 1)
                {
                    float stunStat = MathF.Round((cameraPrefix.stunMult - 1f) * 100, 2);
                    string value = (stunStat > 0 ? "+" : "") + $"{stunStat}";
                    string stunText = BaseCameraPrefix.StunTooltip.WithFormatArgs(value).Value;
                    tooltips.Add(new TooltipLine(Mod, "PrefixHammer: PrefixStun", stunText)
                    {
                        OverrideColor = stunStat > 0 ? color : badColor
                    });
                }

                if(cameraPrefix is ExtraCameraPrefix extraCameraPrefix)
                {
                    string extraTooltip = extraCameraPrefix.PrefixExtraTooltip.Value;
                    if (!string.IsNullOrEmpty(extraTooltip))
                    {
                        tooltips.Add(new TooltipLine(Mod, "PrefixHammer: PrefixExtra", extraTooltip));
                    }
                }

            }

            base.ModifyTooltips(tooltips);
        }
    }
}
