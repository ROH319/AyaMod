using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class MineCamera : BaseCamera, IPlaceholderItem
    {
        public override void SetOtherDefaults()
        {
            Item.width = Item.height = 32;
            Item.damage = 5;

            Item.useTime = Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<MineCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.03f, 80, 2f);
            SetCaptureStats(1000, 60);
        }
    }

    public class MineCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(255, 222, 111);
        public override Color innerFrameColor => new Color(181, 194, 217) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(220, 220, 220).AdditiveColor() * 0.5f;

        /// <summary>
        /// 设为1表示没有大小浮动
        /// </summary>
        /// <returns></returns>
        public override float FloatingFactor() => 1f;

        public override void OnSnap()
        {
            var pos = AyaUtils.GetCameraRect(Projectile.Center, Projectile.rotation, size, size * 1.4f);
            (Vector2 min, Vector2 max) = Helper.GetBoundingBox(pos);
            var topleft = min.ToTileCoordinates();
            var bottomright = max.ToTileCoordinates();

            int minx = topleft.X, maxx = bottomright.X;
            int miny = topleft.Y, maxy = bottomright.Y;

            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                {
                    Tile tile = Main.tile[i, j];

                    if (tile == null) continue;
                    var coord = new Vector2(i, j).ToWorldCoordinates();
                    var tilerect = new Rectangle((int)coord.X, (int)coord.Y, 16, 16);
                    if (!lens.Colliding(Projectile.Center, size, Projectile.rotation, tilerect)) continue;
                    if (tile.HasTile && player.HasEnoughPickPowerToHurtTile(i, j))
                    {

                        if (TileID.Sets.Grass[tile.TileType] || TileID.Sets.GrassSpecial[tile.TileType] || Main.tileMoss[tile.TileType] || TileID.Sets.tileMossBrick[tile.TileType])
                        {
                            player.PickTile(i, j, 10000);
                        }
                        player.PickTile(i, j, 10000);
                    }
                }
            }
        }
    }
}
