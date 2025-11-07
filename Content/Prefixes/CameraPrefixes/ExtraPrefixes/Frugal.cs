using AyaMod.Core;
using AyaMod.Core.Globals;
using Terraria;
using Terraria.Localization;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class Frugal() : ExtraCameraPrefix(damageMult: 1.08f, focusSpeedMult: 0.92f, critBonus: 8, sizeMult: 1.1f, valueMult: 1.5f)
    {
        public override LocalizedText PrefixExtraTooltip => base.PrefixExtraTooltip.WithFormatArgs(AmmoSaveRate);
        public override void Load()
        {
            CameraGlobalItem.CanConsumeAmmoChecker += ReduceAmmoConsumption;
        }

        public static bool ReduceAmmoConsumption(Item weapon, Item ammo, Player player)
        {
            if (weapon.DamageType != ReporterDamage.Instance || weapon.prefix != PrefixType<Frugal>())
                return true;
            return Main.rand.NextBool(AmmoSaveRate, 100);
        }
        public static int AmmoSaveRate = 30;
    }
}
