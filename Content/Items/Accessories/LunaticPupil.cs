using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Accessories
{
    public class LunaticPupil : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SpeedIncreaseVisual, SizeIncrease, SpeedIncrease, SizeDecrease);
        public static LocalizedText VisualOn { get; private set; }
        public static LocalizedText VisualOff { get; private set; }

        public static int Visual = 0;
        public override void SetStaticDefaults()
        {
            VisualOn = this.GetLocalization(nameof(VisualOn));
            VisualOff = this.GetLocalization(nameof(VisualOff));
        }
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Cyan9, Item.sellPrice(0,15));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            int speed = hideVisual ? SpeedIncrease : SpeedIncreaseVisual;
            if (hideVisual)
            {
                player.Camera().SizeBonus -= (float)SizeDecrease / 100f;
                Visual = -1;
                //player.Camera().SingleTargetMultiplier += (float)SingleTargetIncrease / 100f;
            }
            else
            {
                player.Camera().SizeBonus += (float)SizeIncrease / 100f;
                Visual = 1;
            }
            player.GetAttackSpeed<ReporterDamage>() += (float)speed / 100f;
        }

        public static int SpeedIncreaseVisual = 20;
        public static int SpeedIncrease = 30;
        public static int SizeIncrease = 25;
        public static int SizeDecrease = 12;
        public static int SingleTargetIncrease = 24;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (index != -1)
            {
                var visualon = VisualOn.WithFormatArgs(SpeedIncreaseVisual, SizeIncrease);
                var visualoff = VisualOff.WithFormatArgs(SpeedIncrease, SizeDecrease);
                if(Visual == 1)
                {
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "LunaticPupil_ExtraTooltip", visualon.Value)
                    {
                        OverrideColor = new Microsoft.Xna.Framework.Color(150, 150, 255)
                    });
                }
                else if(Visual == -1)
                {
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "LunaticPupil_ExtraTooltip", visualoff.Value)
                    {
                        OverrideColor = new Microsoft.Xna.Framework.Color(255, 150, 255)
                    });
                }
                else
                {
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "LunaticPupil_ExtraTooltip", visualoff.Value)
                    {
                        OverrideColor = new Microsoft.Xna.Framework.Color(255, 150, 255)
                    });
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "LunaticPupil_ExtraTooltip", visualon.Value)
                    {
                        OverrideColor = new Microsoft.Xna.Framework.Color(150, 150, 255)
                    });
                }
            }

            Visual = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<SpazmatismLens>())
                .AddIngredient(ItemType<RetinazerLens>())
                .AddIngredient(ItemID.IceQueenTrophy)
                .AddIngredient(ItemID.PumpkingTrophy)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
