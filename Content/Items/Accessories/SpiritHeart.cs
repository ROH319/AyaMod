using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using AyaMod.Helpers;
using AyaMod.Core.Attributes;
using AyaMod.Core.ModPlayers;

namespace AyaMod.Content.Items.Accessories
{
    [PlayerEffect]
    public class SpiritHeart : BaseAccessories
    {
        public override void Load()
        {
            CameraPlayer.CheckSnapThrouthWallEvent += SpiritDele;
        }
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(gold: 1));
        }

        public static bool SpiritDele(Player player, BaseCameraProj cameraProj)
        {
            if (player.HasEffect<SpiritHeart>() && 
                player.velocity.X == 0 && 
                player.velocity.Y == 0 && 
                player.Distance(cameraProj.Projectile.Center) < SpiritSnapDistance) return true;
            return false;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SpiritHeart>();
        }

        public static int SpiritSnapDistance = 20 * 16;
    }
}
