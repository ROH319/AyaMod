using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using AyaMod.Core.Attributes;
using AyaMod.Helpers;

namespace AyaMod.Content.Items.Accessories
{
    [PlayerEffect]
    public class ManicPupil : BaseAccessories
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(gold: 5));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ManicPupil>();
        }

        public static void CalManicAttackSpeed(Player player, Item item)
        {
            if (item.ModItem is not BaseCamera) return;
            var ayaPlayer = player.Aya();
            if (ayaPlayer.ManicStack < 10) ayaPlayer.ManicStack++;
            if (ayaPlayer.NotUsingCameraTimer > 2 * 60) ayaPlayer.ManicStack = 0;
            ayaPlayer.AttackSpeed += ayaPlayer.ManicStack * SpeedIncrease / 100f;
        }

        public static int SpeedIncrease = 2;
        public static int MaxSpeedIncrease = 20;
    }
}
