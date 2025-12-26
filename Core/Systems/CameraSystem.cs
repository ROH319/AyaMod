using AyaMod.Content.Items.Cameras;
using AyaMod.Content.Items.Materials;
using AyaMod.Content.Items.PrefixHammers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace AyaMod.Core.Systems
{
    public class CameraSystem : ModSystem
    {
        public static event Action OnPostSetupContent;
        public override void PostSetupContent()
        {
            if (OnPostSetupContent == null) return;
            foreach(Action a in OnPostSetupContent.GetInvocationList())
            {
                a();
            }
        }

        public override void PostWorldGen()
        {

            int maxWoodenCamera = 5;
            int maxShadowCamera = 4;
            int maxDesertCamera = 4;
            int woodenPlaced = 0;
            int shadowPlaced = 0;
            int desertPlaced = 0;


            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null) continue;
                if (woodenPlaced >= maxWoodenCamera
                    && shadowPlaced >= maxShadowCamera
                    && desertPlaced >= maxDesertCamera) break;

                Tile chestTile = Framing.GetTileSafely(chest.x, chest.y);

                switch (chestTile.TileType)
                {
                    case TileID.Containers:
                        switch (chestTile.TileFrameX)
                        {
                            default:
                            case 0://木箱子
                                if (woodenPlaced < maxWoodenCamera)
                                {
                                    //判断不是地牢里的木箱子
                                    if (chestTile.WallType is WallID.BlueDungeonUnsafe or WallID.GreenDungeonUnsafe or WallID.PinkDungeonUnsafe
                                                                               or WallID.BlueDungeonSlabUnsafe or WallID.GreenDungeonSlabUnsafe or WallID.PinkDungeonSlabUnsafe
                                                                               or WallID.BlueDungeonTileUnsafe or WallID.GreenDungeonTileUnsafe or WallID.PinkDungeonTileUnsafe) continue;
                                    //1/3概率
                                    if (WorldGen.genRand.NextBool(3) && woodenPlaced > 1) continue;
                                    chest.AddItem<ToyCamera>();
                                    woodenPlaced++;
                                }
                                break;
                            case 1 * 36://金箱子
                                {
                                    if(chest.y > Main.rockLayer)
                                    {
                                        if (WorldGen.genRand.NextBool(10, 100))
                                            chest.AddItem<ClassicalHammer>();
                                    }
                                }
                                break;
                            case 2 * 36://上锁金箱（地牢）
                                {
                                    if (WorldGen.genRand.NextBool(20, 100))
                                        chest.AddItem<NecromanticHammer>();
                                }
                                break;
                            case 4 * 36://暗影箱
                                if (shadowPlaced < maxShadowCamera)
                                {

                                    //1/3概率
                                    if (WorldGen.genRand.NextBool(3)) continue;
                                    chest.AddItem<ShadowCamera>();
                                    shadowPlaced++;
                                }
                                break;
                        }
                        break;
                    case TileID.Containers2:
                        switch (chestTile.TileFrameX)
                        {
                            default: break;
                            case 10 * 36:
                                if (desertPlaced < maxDesertCamera)
                                {

                                    if (WorldGen.genRand.NextBool(3)) continue;
                                    chest.AddItem<VitricLightning>();
                                    desertPlaced++;
                                }
                                break;
                        }
                        break;
                    default: break;
                }
            }
        }
        public override void PreUpdateEntities()
        {
            foreach (var projectile in Main.ActiveProjectiles)
            {
                projectile.Aya().SpeedModifier = StatModifier.Default;
            }
            //BlackHoleScreen.Active = false;
        }
        public override void PostUpdatePlayers()
        {
            foreach(var projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile != null && projectile.ModProjectile is BaseCameraProj)
                {
                    var camera = projectile.ModProjectile as BaseCameraProj;
                    camera.UpdateHeld();
                }
            }
        }
        public override void PreUpdateProjectiles()
        {
            //ISinkProjectile的实现
            for(int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.ModProjectile == null || projectile.ModProjectile is not ISinkProjectile || !projectile.active) continue;
                var sinkProjectile = projectile.ModProjectile as ISinkProjectile;

                for(int j = 0; j < i; j++)
                {
                    Projectile pre = Main.projectile[j];
                    if (pre.active)
                    {
                        if(pre.ModProjectile == null || pre.ModProjectile is not ISinkProjectile) continue;
                        var sinkPre = pre.ModProjectile as ISinkProjectile;
                        if (sinkPre.SinkDepth < sinkProjectile.SinkDepth)
                            SwapProjectile(j, i);
                    }
                    SwapProjectile(j, i);
                }
            }
        }
        public static void SwapProjectile(int index, int index2)
        {
            var temp = Main.projectile[index];
            Main.projectile[index] = Main.projectile[index2];
            Main.projectile[index2] = temp;

            Main.projectile[index2].whoAmI = index;
            Main.projectile[index].whoAmI = index2;
        }
        public override void PostUpdateEverything()
        {
            foreach(var projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile != null && projectile.ModProjectile is BaseCameraProj)
                {
                    var camera = projectile.ModProjectile as BaseCameraProj;
                    camera.films.Clear();
                    camera.HighestHealthTarget = -1;
                }
            }
            foreach(var player in Main.ActivePlayers)
            {
                player.Aya().itemTimeLastFrame = player.itemTime;
            }
        }

        public override void PostDrawTiles()
        {
            //BlackHoleScreen.Active = false;
        }

    }
}
