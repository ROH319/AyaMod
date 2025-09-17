using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Items.Testing
{
    [PlayerEffect]
    public class NoDmgModifier : ModItem
    {
        public override string Texture => AssetDirectory.Testing + Name;
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            
        }

        public override void UpdateInventory(Player player)
        { 
            player.AddEffect<NoDmgModifier>();
        }
    }
}
