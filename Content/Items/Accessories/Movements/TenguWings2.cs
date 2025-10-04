using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using static Terraria.ID.ArmorIDs;

namespace AyaMod.Content.Items.Accessories.Movements
{
    [AutoloadEquip(EquipType.Wings)]
    public class TenguWings2 : BaseAccessories
    {
        public override string Texture => AssetDirectory.Accessories + "Movements/" + Name;

        public static float FlySpeed = 9;
        public static float Acceleration = 2.5f;
        public static float MaxAscentMultiplier = 3f;
        public static int FlyTime = 180;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FlySpeed, MaxAscentMultiplier, Acceleration, FlyTime);

        public override void SetStaticDefaults()
        {
            //flySpeedOverride最大水平速度：9f->46mph
            //accelerationMultiplier水平加速：250%
            Wing.Sets.Stats[Item.wingSlot] = new WingStats(FlyTime, FlySpeed, Acceleration);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.StrongRed10, Item.sellPrice(gold: 10));

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.jumpSpeedBoost += 1.8f;
            player.moveSpeed += 0.075f;
            player.Aya().AccSpeedModifier += 0.5f;
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.6f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = MaxAscentMultiplier;
            constantAscend = 0.125f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 9f;
            acceleration *= 2.5f;
        }
    }
}
