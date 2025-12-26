using AyaMod.Content.Buffs;
using AyaMod.Content.Items.Placeables;
using AyaMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ObjectData;

namespace AyaMod.Content.Tiles
{
    public class TripodTile : ModTile
    {
        public override string Texture => AssetDirectory.Tiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(30, 30, 140), null);
            DustType = DustID.Stone;
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = (fail ? 15 : 3);
        }
        public override bool RightClick(int i, int j)
        {
            Player localplayer = Main.LocalPlayer;
            SoundEngine.PlaySound(SoundID.Item4, localplayer.position);
            localplayer.AddBuff(BuffType<TripodBuff>(), 108000);
            return true;
        }
        public override void MouseOver(int i, int j)
        {
            Player localPlayer = Main.LocalPlayer;
            localPlayer.cursorItemIconID = ItemType<Tripod>();
            localPlayer.cursorItemIconText = "";
            localPlayer.cursorItemIconEnabled = true;
        }
    }
}
