using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using AyaMod.Core.Globals;
using AyaMod.Helpers;
using AyaMod.Core.Attributes;
using AyaMod.Core.ModPlayers;
namespace AyaMod.Content.Items.Accessories
{
    [PlayerEffect]
    public class TriggerShutter : BaseAccessories
    {
        public override void Load()
        {
            GlobalCamera.PreAIHook += Shutter;
            CameraPlayer.PostUpdateHook += ShutterUpdate;
        }
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(gold: 1));
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TriggerShutter>();
        }
        public static void ShutterUpdate(Player player)
        {
            if (player.HasEffect<TriggerShutter>())
            {
                if (player.Camera().HoldNonCameraCounter > 90)
                {
                    player.Camera().FreeSnap = true;
                }
            }
        }
        public static bool Shutter(Player player, BaseCameraProj proj)
        {
            if (player.HasEffect<TriggerShutter>() && player.Camera().FreeSnap)
            {
                if (player.ItemAnimationJustStarted)
                {
                    while (!proj.CheckCanDamage())
                    {
                        player.itemTime--;
                        player.itemAnimation--;
                    }
                    player.Camera().FreeSnap = false;
                }
            }
            return true;
        }
    }
}
