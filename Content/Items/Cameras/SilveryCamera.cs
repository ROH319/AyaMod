using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class SilveryCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 38;
            Item.height = 30;

            Item.damage = 10;

            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<SilveryCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.03f, 80, 2f);
            SetCaptureStats(1000, 60);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SilverBar, 15)
                .AddTile(TileID.Anvils)
                .Register(); 
            CreateRecipe()
                .AddIngredient(ItemID.TungstenBar, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SilveryCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(200, 200, 200);
        public override Color innerFrameColor => new Color(150, 150, 150) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(220, 220, 220).AdditiveColor() * 0.5f;
        public override void OnSnapInSight()
        {
            int damage = (int)(Projectile.damage * 0.8f);
            ScarletCameraProj.SpawnKnives(Projectile.Center, 1, 200, 8, damage, Projectile.knockBack, Projectile.GetSource_FromThis(), Projectile.owner, 0.15f);
        }
    }
}
