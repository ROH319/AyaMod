using AyaMod.Content.Buffs;
using AyaMod.Content.Items.Materials;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class IzanagiCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {

            Item.width = 52;
            Item.height = 48;

            Item.damage = 150;

            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<IzanagiCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(0, 0, 78, 0));
            SetCameraStats(0.06f, 180, 1.6f, 0.5f, 3);
            SetCaptureStats(1000, 60);
        }
        public override void HoldItem(Player player)
        {
            player.AddBuff(BuffType<DevelopingBuff>(), 2);
            player.Camera().FilmEffectChanceModifier *= 1.5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FlawlessCamera>()
                .AddIngredient<IzanagiObject>(15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class IzanagiCameraProj : BaseCameraProj
    {

        public override Color outerFrameColor => new Color(250, 232, 136);
        public override Color innerFrameColor => new Color(232, 79, 39) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(249, 239, 189).AdditiveColor() * 0.5f;

    }
}
