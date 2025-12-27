using AyaMod.Content.Items.Materials;
using AyaMod.Content.Items.Placeables;
using AyaMod.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Tiles
{
    public class MapleTree : ModTree
    {
        public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };
        public Asset<Texture2D> texture;
        public Asset<Texture2D> branchesTexture;
        public Asset<Texture2D> topsTexture;
        public override void SetStaticDefaults()
        {
            //暂时用泥作为生长土壤
            GrowsOnTileId = [TileID.Mud];
            texture = Request<Texture2D>(AssetDirectory.Tiles + "MapleTree");
            branchesTexture = Request<Texture2D>(AssetDirectory.Tiles + "MapleTree_Branches");
            topsTexture = Request<Texture2D>(AssetDirectory.Tiles + "MapleTree_Top");
        }
        public override int DropWood() => Main.rand.NextBool(10) ? ItemType<MapleTreeSaplingSeed>() : ItemType<MapleLeaf>();

        public override Asset<Texture2D> GetBranchTextures() => branchesTexture;

        public override Asset<Texture2D> GetTexture() => texture;
        public override Asset<Texture2D> GetTopTextures() => topsTexture;

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
        }
        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return TileType<MapleTreeSapling>();
        }
    }
}
