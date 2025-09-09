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
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SpeedIncrease);
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Cyan9, Item.sellPrice(0,15));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed<ReporterDamage>() += (float)SpeedIncrease / 100f;
            if (hideVisual)
            {
                player.Camera().SizeBonus -= (float)SizeDecrease / 100f;
                player.Camera().SingleTargetMultiplier += (float)SingleTargetIncrease / 100f;
            }
            else
            {
                player.Camera().SizeBonus += (float)SizeIncrease / 100f;
            }
        }

        public static int SpeedIncrease = 20;
        public static int SizeIncrease = 40;
        public static int SizeDecrease = 30;
        public static int SingleTargetIncrease = 24;

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
