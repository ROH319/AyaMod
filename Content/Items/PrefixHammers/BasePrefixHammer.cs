using AyaMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;

namespace AyaMod.Content.Items.PrefixHammers
{
    public abstract class BasePrefixHammer : ModItem
    {
        public override string Texture => AssetDirectory.PrefixHammers + Name;

        public abstract int PrefixTypeToForge { get; }

        public override void SetDefaults()
        {
            Item.width = Item.height = 28;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Green;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
        }

    }
}
