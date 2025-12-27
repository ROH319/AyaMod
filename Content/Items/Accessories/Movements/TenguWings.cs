using AyaMod.Core;
using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using static Terraria.ID.ArmorIDs;
using Terraria.DataStructures;
using AyaMod.Helpers;
using Terraria.ID;
using Terraria.Localization;
using AyaMod.Content.Items.Materials;
using AyaMod.Core.Attributes;
using AyaMod.Core.ModPlayers;

namespace AyaMod.Content.Items.Accessories.Movements
{
    [AutoloadEquip(EquipType.Wings)]
    [PlayerEffect]
    public class TenguWings : BaseAccessories, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Accessories + "Movements/" + Name;

        public static float FlySpeed = 7.5f;
        public static float Acceleration = 1.2f;
        public static float MaxAscentMultiplier = 1.5f;
        public static int FlyTime = 150;
        public static int GlideSpeedBonus = 20;
        public static int GlideGravDecrease = 75;

        public override LocalizedText Tooltip => 
            base.Tooltip.WithFormatArgs(FlySpeed, MaxAscentMultiplier, Acceleration, FlyTime, GlideSpeedBonus, GlideGravDecrease);

        public override void Load()
        {
            AyaPlayer.PreUpdateMovementHook += GlideMovement;
        }
        public override void SetStaticDefaults()
        {
            //flySpeedOverride最大水平速度：7.5f->38mph
            //accelerationMultiplier水平加速：120%
            Wing.Sets.Stats[Item.wingSlot] = new WingStats(FlyTime, FlySpeed, Acceleration);
        }
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(gold: 1));

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TenguWings>();
        }
        public static void GlideMovement(Player player)
        {
            if (!player.HeldCamera() || !player.HasEffect<TenguWings>()) return;
            if (!player.controlJump && player.TryingToHoverUp)
            {
                float gravMult = 1f - GlideGravDecrease / 100f;
                if (player.gravDir == 1f)
                {
                    if (player.velocity.Y > player.maxFallSpeed * gravMult)
                        player.velocity.Y = player.maxFallSpeed * gravMult;
                }
                else
                {
                    if (player.velocity.Y < (0f - player.maxFallSpeed) * gravMult)
                        player.velocity.Y = (0f - player.maxFallSpeed) * gravMult;
                }
            }
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 0.5f;
            maxAscentMultiplier = MaxAscentMultiplier;
            constantAscend = 0.1f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 7.5f;
            acceleration *= 1.5f;

            if (player.HeldCamera())
            {
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
                .AddIngredient(ItemID.HarpyWings)
                .AddIngredient(ItemID.SoulofFlight, 25)
                .AddIngredient(ItemID.SoulofMight,10)
                .AddIngredient(ItemID.Feather, 20)
                .AddIngredient<MapleLeaf>(45)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
