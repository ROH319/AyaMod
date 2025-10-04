using AyaMod.Core;
using AyaMod.Core.Attributes;
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
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ID.ArmorIDs;

namespace AyaMod.Content.Items.Accessories.Movements
{
    [AutoloadEquip(EquipType.Wings)]
    [PlayerEffect]
    public class TenguWings1 : BaseAccessories
    {
        public override string Texture => AssetDirectory.Accessories + "Movements/" + Name;

        public static float FlySpeed = 8f;
        public static float Acceleration = 1.75f;
        public static float MaxAscentMultiplier = 2f;
        public static int FlyTime = 170;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FlySpeed, MaxAscentMultiplier, Acceleration, FlyTime);

        public override void SetStaticDefaults()
        {
            Wing.Sets.Stats[Item.wingSlot] = new WingStats(FlyTime, FlySpeed, Acceleration, false, 9f, 9f);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Yellow8, Item.sellPrice(gold: 6));

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.HeldCamera() && player.wingTime == player.wingTimeMax)
                player.Aya().FreeFlyFrame += 15;
            player.AddEffect<TenguWings1>();
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 0.75f;
            maxAscentMultiplier = MaxAscentMultiplier;
            constantAscend = 0.1f;

            if (player.controlJump && player.HeldCamera())
            {
                ascentWhenFalling = 0.75f;
                ascentWhenRising = 0.2f;
                maxAscentMultiplier = 2.3f;
                constantAscend = 0.125f;
            }
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 8f;
            acceleration *= 1.75f;

            if (player.HeldCamera())
            {
                //手持相机时可以悬停
                var wingStats = player.GetWingStats(player.wingsLogic);
                bool flag = player.TryingToHoverDown && player.controlJump && player.wingTime > 0f;
                if (flag)
                {
                    player.accRunSpeed = wingStats.DownHoverSpeedOverride;
                    player.runAcceleration *= wingStats.DownHoverAccelerationMult;
                }

                player.Aya().AccSpeedModifier += 0.25f;
            }
        }
        public override bool WingUpdate(Player player, bool inUse)
        {
            return base.WingUpdate(player, inUse);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<TenguWings>())
                .AddIngredient(ItemID.BlackFairyDust, 3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
