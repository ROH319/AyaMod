using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using AyaMod.Core.Attributes;
using AyaMod.Helpers;

namespace AyaMod.Content.Items.Accessories
{
    [PlayerEffect]
    public class TranquilPupil : BaseAccessories
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(gold: 5));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TranquilPupil>();
        }

        public static void ModifySteathDamage(Player player, Item item, ref StatModifier damage)
        {
            var ayaPlayer = player.Aya();
            float damageMult = Utils.Remap(ayaPlayer.NotUsingCameraTimer, 0, 150, 1f, 3f);
            damage *= damageMult;
        }
    }
}
