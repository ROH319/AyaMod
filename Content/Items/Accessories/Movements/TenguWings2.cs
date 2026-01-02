using AyaMod.Content.Items.Materials;
using AyaMod.Core;
using AyaMod.Core.Loaders;
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
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ID.ArmorIDs;

namespace AyaMod.Content.Items.Accessories.Movements
{
    [AutoloadEquip(EquipType.Wings)]
    public class TenguWings2 : BaseAccessories, IPlaceholderItem
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
            player.GetModPlayer<TenguWingPlayer>().CanFreeMove = true;
            player.noFallDmg = true;
            player.wingTime = player.wingTimeMax;
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
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<TenguWings1>()
                .AddIngredient(ItemID.EmpressFlightBooster)
                .AddIngredient<WindyStarFragment>()
                .Register();
        }
    }
    public class TenguWingPlayer : ModPlayer
    {
        public bool CanFreeMove;
        public bool freeMove;
        public bool SlowMove;
        public float wingSpeedMax => SlowMove ? 4f : 8f;
        public float wingSpeed;
        public override void ResetEffects()
        {
            wingSpeed = wingSpeedMax;
            if (freeMove)
            {
                flight();
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (CanFreeMove && AyaKeybindLoader.SpecialMove.JustPressed)
            {
                freeMove = !freeMove;
                return;
            }
        }
        public void flight()
        {
            UpdateSlowMove();
            Player.gravity = 0f;
            Player.noFallDmg = true;
            Player.maxFallSpeed *= 4f;
            Player.wingTime = -1f;
            //Main.NewText($"{SlowMove} {wingSpeedMax} {Player.velocity.X}");
            bool up = (Player.controlUp || Player.controlJump) && !Player.controlDown;
            bool down = Player.controlDown && !Player.controlJump && !Player.controlUp;
            bool left = Player.controlLeft && !Player.controlRight;
            bool right = Player.controlRight && !Player.controlLeft;
            bool movingVer = up || down;
            bool movingHor = left || right;
            float accelerationSpeed = wingSpeed / 3f;
            float speedMult = 1.2f;
            if (up || down)
            {
                float dir = down.ToDirectionInt();
                float speed = accelerationSpeed * dir;
                Player.velocity.Y += speed;
            }
            if (left || right)
            {
                float dir = right.ToDirectionInt();
                float speed = accelerationSpeed * dir;
                Player.velocity.X += speed;
            }
            Player.velocity.X = MathHelper.Clamp(Player.velocity.X, -wingSpeed * speedMult, wingSpeed * speedMult);
            Player.velocity.Y = MathHelper.Clamp(Player.velocity.Y, -wingSpeed * speedMult, wingSpeed * speedMult);
            if (!movingVer)
                Player.velocity.Y *= 0.8f;
            if (!movingHor)
                Player.velocity.X *= 0.8f;
        }
        public void UpdateSlowMove()
        {
            if(Player.whoAmI == Main.myPlayer)
            {
                SlowMove = AyaKeybindLoader.SlowMove.Current;
            }
        }
    }
}
