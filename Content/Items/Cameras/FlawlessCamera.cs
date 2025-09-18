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

namespace AyaMod.Content.Items.Cameras
{
    public class FlawlessCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 20;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;

            Item.shootSpeed = 8;
            Item.knockBack = 8f;
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 1, 0, 0));
            SetCameraStats(0.04f, 98, 2f, filmSlot: 2);
            SetCaptureStats(100, 5);
        }
    }

    public class FlawlessCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(255, 222, 111);
        public override Color innerFrameColor => new Color(181, 194, 217) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(255, 251, 179).AdditiveColor() * 0.5f;
    }
}
