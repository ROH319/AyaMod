using AyaMod.Content.Items.Cameras;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

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
            

            for(int chestIndex = 0;chestIndex<Main.maxChests;chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null) continue;
                if (woodenPlaced >= maxWoodenCamera 
                    && shadowPlaced >= maxShadowCamera
                    && desertPlaced >= maxDesertCamera) break;

                Tile chestTile = Main.tile[chest.x, chest.y];
                //4是上锁暗影箱的帧图位置，36是每帧x
                if (chestTile.TileType == TileID.Containers)
                {
                    if (chestTile.TileFrameX == 4 * 36 && shadowPlaced < maxShadowCamera) 
                    {
                        //1/3概率
                        if (WorldGen.genRand.NextBool(3)) continue;
                        chest.AddItem<ShadowCamera>();
                        shadowPlaced++;
                    }
                    else if (chestTile.TileFrameX == 0 && woodenPlaced < maxWoodenCamera)
                    {
                        if (chestTile.WallType is WallID.BlueDungeonUnsafe or WallID.GreenDungeonUnsafe or WallID.PinkDungeonUnsafe
                                                                   or WallID.BlueDungeonSlabUnsafe or WallID.GreenDungeonSlabUnsafe or WallID.PinkDungeonSlabUnsafe
                                                                   or WallID.BlueDungeonTileUnsafe or WallID.GreenDungeonTileUnsafe or WallID.PinkDungeonTileUnsafe) continue;
                        //1/3概率
                        if (WorldGen.genRand.NextBool(3) && woodenPlaced > 1) continue;
                        chest.AddItem<ToyCamera>();
                        woodenPlaced++;
                    }
                }

                if (chestTile.TileType == TileID.Containers2)
                {
                    if (chestTile.TileFrameX == 10 * 36 && desertPlaced < maxDesertCamera)
                    {
                        if (WorldGen.genRand.NextBool(3)) continue;
                        chest.AddItem<VitricLightning>();
                        desertPlaced++;
                    }
                }
            }

            base.PostWorldGen();
        }
        public override void PreUpdateEntities()
        {
            foreach (var projectile in Main.ActiveProjectiles)
            {
                projectile.Aya().SpeedModifier = StatModifier.Default;
            }
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
    }
}
