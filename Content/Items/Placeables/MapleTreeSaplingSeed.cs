using AyaMod.Content.Tiles;
using AyaMod.Core;
using Terraria;

namespace AyaMod.Content.Items.Placeables
{
    public class MapleTreeSaplingSeed : ModItem
    {
        public override string Texture => AssetDirectory.Placeables + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<MapleTreeSapling>());
            Item.value = Item.buyPrice(gold: 1);
        }
    }
}
