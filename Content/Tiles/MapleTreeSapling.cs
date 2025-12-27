using AyaMod.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;
using static AyaMod.Core.ModPlayers.AyaPlayer;

namespace AyaMod.Content.Tiles
{
    public class MapleTreeSapling : ModTile
    {
        public override string Texture => AssetDirectory.Tiles + Name;

        public override void SetStaticDefaults()
        {
            //Main.tileFrameImportant[Type] = true;
            //Main.tileNoAttach[Type] = true;
            //Main.tileLavaDeath[Type] = true;

            //TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            //TileObjectData.newTile.Origin = new Point16(0, 1);
            //TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            //TileObjectData.newTile.UsesCustomCanPlace = true;
            //TileObjectData.newTile.CoordinateHeights = [16, 18];
            //TileObjectData.newTile.CoordinateWidth = 16;
            //TileObjectData.newTile.CoordinatePadding = 2;
            ////TileObjectData.newTile.DrawYOffset = -2;
            //TileObjectData.newTile.AnchorValidTiles = [TileID.Mud];
            //TileObjectData.newTile.StyleHorizontal = true;
            //TileObjectData.newTile.DrawFlipHorizontal = true;
            //TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            //TileObjectData.newTile.LavaDeath = true;
            //TileObjectData.newTile.RandomStyleRange = 3;
            //TileObjectData.newTile.StyleMultiplier = 3;

            //TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            //TileObjectData.newTile.Origin = new Point16(0, 1);
            //TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            //TileObjectData.newTile.UsesCustomCanPlace = true;
            //TileObjectData.newTile.CoordinateHeights = [16, 18];
            //TileObjectData.newTile.CoordinateWidth = 16;
            //TileObjectData.newTile.DrawYOffset = 16 - 18;
            //TileObjectData.newTile.AnchorValidTiles = [TileID.Mud];
            //TileObjectData.newTile.StyleHorizontal = true;
            //TileObjectData.newTile.DrawFlipHorizontal = true;
            //TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            //TileObjectData.newTile.LavaDeath = true;
            //TileObjectData.newTile.RandomStyleRange = 3;
            //TileObjectData.newTile.StyleMultiplier = 3;

            //TileObjectData.addTile(Type);

            //AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Sapling"));

            //TileID.Sets.CommonSapling[Type] = true;
            //TileID.Sets.TreeSapling[Type] = true;
            //TileID.Sets.SwaysInWindBasic[Type] = true;
            //TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

            //DustType = DustID.Dirt;
            //AdjTiles = [TileID.Saplings];


            //int width = 16;
            //int Bottomheight = 18;
            //int[] AnchorValidTiles = [TileID.Mud];
            //Main.tileFrameImportant[Type] = true;
            //Main.tileNoAttach[Type] = true;
            //Main.tileLavaDeath[Type] = true;

            //TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            //TileObjectData.newTile.Origin = new Point16(0, 1);
            //TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            //TileObjectData.newTile.UsesCustomCanPlace = true;
            //TileObjectData.newTile.CoordinateHeights = [16, Bottomheight];
            //TileObjectData.newTile.CoordinateWidth = width;
            //TileObjectData.newTile.DrawYOffset = 16 - Bottomheight;
            //TileObjectData.newTile.AnchorValidTiles = AnchorValidTiles;
            //TileObjectData.newTile.StyleHorizontal = true;
            //TileObjectData.newTile.DrawFlipHorizontal = true;
            //TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            //TileObjectData.newTile.LavaDeath = true;
            //TileObjectData.newTile.RandomStyleRange = 3;
            //TileObjectData.newTile.StyleMultiplier = 3;
            //TileObjectData.addTile(Type);

            //AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Sapling"));

            //TileID.Sets.TreeSapling[Type] = true;
            //TileID.Sets.CommonSapling[Type] = true;
            //TileID.Sets.SwaysInWindBasic[Type] = true;
            //TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]); // Make this tile interact with golf balls in the same way other plants do

            //DustType = DustID.Dirt;
            //AdjTiles = [TileID.Saplings];


            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorValidTiles = [TileID.Mud];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawFlipHorizontal = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.StyleMultiplier = 3;

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Sapling"));

            TileID.Sets.TreeSapling[Type] = true;
            TileID.Sets.CommonSapling[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]); // Make this tile interact with golf balls in the same way other plants do

            DustType = DustID.Dirt;

            AdjTiles = [TileID.Saplings];
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.NextBool(10))
            {
                bool isPlayerNear = WorldGen.PlayerLOS(i, j);
                bool success = WorldGen.GrowTree(i, j);
                if (success && isPlayerNear)
                {
                    WorldGen.TreeGrowFXCheck(i, j);
                }
            }
        }
        public override void SetSpriteEffects(int i, int j, ref SpriteEffects effects)
        {
            if (i % 2 == 0)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
        }
        
    }
}
