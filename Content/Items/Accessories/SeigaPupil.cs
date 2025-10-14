using AyaMod.Core.Attributes;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Accessories
{
    [PlayerEffect]
    public class SeigaPupil : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ChaseSpeedBonus);
        public override void Load()
        {
            CameraPlayer.CheckSnapThrouthWallEvent += SeigaSight;
        }
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.LightPurple6, Item.sellPrice(gold: 4));
        }

        public static bool SeigaSight(Player player, BaseCameraProj cameraProj)
        {
            if (player.HasEffect<SeigaPupil>() &&
                player.Distance(cameraProj.Projectile.Center) < SpiritSnapDistance) return true;
            return false;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Camera().ChaseSpeedModifier += ChaseSpeedBonus / 100f;
            player.AddEffect<SeigaPupil>();
        }
        public static int SpiritSnapDistance = 35 * 16;
        public static int ChaseSpeedBonus = 25;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Nazar)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient<IntelligentGyroscope>()
                .AddIngredient<SpiritHeart>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
