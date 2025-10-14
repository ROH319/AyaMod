using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.ID;
using Terraria;
using AyaMod.Helpers;
using Terraria.ModLoader;

namespace AyaMod.Content.Items.Cameras
{
    public class ToyCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 15;

            Item.useTime = Item.useAnimation = 55;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<ToyCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.White0, Item.sellPrice(0, 0, 12, 0));
            SetCameraStats(0.025f, 72, 2f);
            SetCaptureStats(1000, 60);
        }
        public override bool NeedsAmmo(Player player) => false;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 15)
                .AddIngredient(ItemID.CopperBar, 10)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 15)
                .AddIngredient(ItemID.TinBar, 10)
                .Register();
        }
    }

    public class ToyCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(255, 85, 43);
        public override Color innerFrameColor => new Color(176, 215, 96) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(139, 233, 167).AdditiveColor() * 0.5f;
    }
}
