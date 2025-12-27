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
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ID.ArmorIDs;

namespace AyaMod.Content.Items.Accessories.Movements
{
    [AutoloadEquip(EquipType.Wings)]
    [PlayerEffect]
    public class TenguWings1 : BaseAccessories, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Accessories + "Movements/" + Name;

        public static float FlySpeed = 8f;
        public static float Acceleration = 1.75f;
        public static float MaxAscentMultiplier = 2f;
        public static int FlyTime = 170;
        public static int GlideSpeedBonus = 20;
        public static int GlideGravDecrease = 90;
        public override LocalizedText Tooltip => 
            base.Tooltip.WithFormatArgs(FlySpeed, MaxAscentMultiplier, Acceleration, FlyTime, GlideSpeedBonus, GlideGravDecrease);

        public override void Load()
        {
            AyaPlayer.PreUpdateMovementHook += GlideMovement;
        }
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
            player.AddEffect<TenguWings1>();
        }
        public static void GlideMovement(Player player)
        {
            if (!player.HeldCamera() || !player.HasEffect<TenguWings1>()) return;
            if (!player.controlJump && player.TryingToHoverUp)
            {
                float gravMult = 1f - GlideGravDecrease / 100f;
                if (player.gravDir == 1f)
                {
                    if (player.velocity.Y > player.maxFallSpeed * 0.2f)
                        player.velocity.Y = player.maxFallSpeed * gravMult;
                }
                else
                {
                    if (player.velocity.Y < (0f - player.maxFallSpeed) * 0.2f)
                        player.velocity.Y = (0f - player.maxFallSpeed) * gravMult;
                }
            }
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 0.75f;
            maxAscentMultiplier = MaxAscentMultiplier;
            constantAscend = 0.1f;

            if (player.HeldCamera())
            {
                if (player.controlJump)
                {
                    ascentWhenFalling = 0.75f;
                    ascentWhenRising = 0.2f;
                    maxAscentMultiplier = 2.3f;
                    constantAscend = 0.125f;
                }

                bool inUse = player.wingsLogic > 0 && player.controlJump && player.velocity.Y != 0f;
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

                //不飞行的状态下按住up可以进行滑翔
                bool inUse = player.wingsLogic > 0 && player.controlJump && player.velocity.Y != 0f;

                if (!player.controlJump && player.TryingToHoverUp)
                {
                    speed *= 1f + GlideSpeedBonus / 100f;
                }
            }
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
