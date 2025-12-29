using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace AyaMod.Core.Systems
{
    public class StrangePlantSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {
            GrowStrangePlants();
        }

        public static void GrowStrangePlants()
        {
            double worldUpdateRate = WorldGen.GetWorldUpdateRate();
            double num = 3E-05f * (float)worldUpdateRate;
            double num2 = 1.5E-05f * (float)worldUpdateRate;
            int num5 = 151;
            double num4 = (double)(Main.maxTilesX * Main.maxTilesY) * num;
            int num6 = (int)Utils.Lerp(num5, (double)num5 * 2.8, Utils.Clamp((double)Main.maxTilesX / 4200.0 - 1.0, 0.0, 1.0));

            for (int j = 0; (double)j < num4; j++)
            {

                int i2 = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int j2 = WorldGen.genRand.Next(10, (int)Main.worldSurface - 1);
                UpdateOvergroundTile(i2, j2);
            }

            for (int l = 0; (double)l < (double)(Main.maxTilesX * Main.maxTilesY) * num2; l++)
            {
                int i4 = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
                int j4 = WorldGen.genRand.Next((int)Main.worldSurface - 1, Main.maxTilesY - 20);
                UpdateUndergroundTile(i4, j4);
            }
        }
        public static void UpdateOvergroundTile(int i, int j)
        {

            if (Main.tile[i, j] == null)
                return;

            if (Main.tile[i, j].TileType != TileID.LilyPad && Main.tile[i, j].LiquidAmount <= 32 && Main.tile[i, j].HasUnactuatedTile)
            {
                //原next(15000)
                if ((i < Main.maxTilesX * 0.4 || i > Main.maxTilesX * 0.6) && Main.rand.NextBool(500))
                    WorldGen.plantDye(i, j, exoticPlant: true);

            }
        }
        public static void UpdateUndergroundTile(int i, int j)
        {

            if (Main.tile[i, j] == null)
                return;

            if (!Main.tileAlch[Main.tile[i,j].TileType] && Main.tile[i, j].HasUnactuatedTile)
            {
                //原next(15000)
                if ((i < Main.maxTilesX * 0.4 || i > Main.maxTilesX * 0.6) && Main.rand.NextBool(500))
                    WorldGen.plantDye(i, j, exoticPlant: true);
            }
        }
    }
}
