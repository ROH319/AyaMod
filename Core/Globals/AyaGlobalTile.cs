using AyaMod.Content.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace AyaMod.Core.Globals
{
    public class AyaGlobalTile : GlobalTile
    {

        public override void Drop(int i, int j, int type)
        {
            if (type == 596)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 24, 24), ItemType<SakuraPetal>(), 3);

                if (Main.rand.NextBool(8))
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 24, 24), ItemID.VanityTreeSakuraSeed);
                return;
            }
            base.Drop(i, j, type);
        }
    }
}
