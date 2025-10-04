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
    public class DigitalCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 30;

            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<DigitalCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.White0, Item.sellPrice(0, 0, 98, 0));
            SetCameraStats(0.04f, 114, 1.7f);
            SetCaptureStats(1000, 60);
        }
    }

    public class DigitalCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(128, 128, 128);
        public override Color innerFrameColor => new Color(181, 194, 217) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(119, 136, 153).AdditiveColor() * 0.5f;

        public override bool PreAI()
        {
            AddDamagePoint(0.6f);
            AddDamagePoint(0.8f);
            return base.PreAI();
        }
    }
}
