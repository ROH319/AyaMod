using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace AyaMod.Content.Items.Cameras
{
    public class HandmadeCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 20;

            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<HandmadeCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.White0, Item.sellPrice(0, 0, 12, 0));
            SetCameraStats(0.025f, 72, 2f);
            SetCaptureStats(100, 5);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IronBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.LeadBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class HandmadeCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(169, 169, 169);
        public override Color innerFrameColor => new Color(128, 128, 128) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(211, 211, 211).AdditiveColor() * 0.5f;
    }
}
