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
using Terraria.Localization;

namespace AyaMod.Content.Items.Accessories
{
    [PlayerEffect]
    public class TranquilPupil : BaseAccessories
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxChargeTime, MaxDamageMult);
        public override void Load()
        {
            AyaPlayer.ModifyWeaponDamageHook += ModifySteathDamage;
        }
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
            if(!player.HasEffect<TranquilPupil>()) return;
            var ayaPlayer = player.Aya();
            float damageMult = 1 + Utils.Remap(ayaPlayer.NotUsingCameraTimer, 0, MaxChargeTime * 60, 0f, MaxDamageMult / 100f);
            damage *= damageMult;
        }

        public static float MaxChargeTime = 2.5f;
        public static int MaxDamageMult = 200;
    }
}
