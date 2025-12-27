using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.ModPlayers;
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
using Terraria.WorldBuilding;

namespace AyaMod.Content.Items.Accessories
{
    [PlayerEffect]
    public class FalsePHDJ : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageIncrease, CritIncrease, HurtIncrease);
        public static int DamageIncrease = 15;
        public static int CritIncrease = 10;
        public static int HurtIncrease = 10;
        public override void Load()
        {
            AyaPlayer.ModifyHitByBothHook += FalseHurt;
        }
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Lime7, Item.sellPrice(gold: 6));
        }
        
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ReporterDamage>() += DamageIncrease / 100f;
            player.GetCritChance<ReporterDamage>() += CritIncrease;
            player.AddEffect<FalsePHDJ>();
        }
        public static void FalseHurt(Player player, ref Player.HurtModifiers modifiers)
        {
            if (player.HasEffect<FalsePHDJ>())
            {
                modifiers.FinalDamage *= 1f + HurtIncrease / 100f;
            }
        } 
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReporterEmblem>()
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
