using AyaMod.Core;
using AyaMod.Core.Prefabs;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader.Default;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class PrismaticFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 4778;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonusStep);
        public override LocalizedText DevTooltip => base.DevTooltip.WithFormatArgs(DamageBonusStepDev);
        public int ExtraDmg;
        public static int DamageBonusStep = 4;
        public static int DamageBonusStepDev = 6;
        public override StatModifier DamageModifier => base.DamageModifier + ExtraDmg / 100f;
        public override void PreAI(BaseCameraProj projectile)
        {
            var player = projectile.player;
            List<int> listedRarity = new();

            for (int m = 3; m < 10; m++)
            {
                if(player.IsItemSlotUnlockedAndUsable(m) && !listedRarity.Contains(player.armor[m].rare))
                {
                    listedRarity.Add(player.armor[m].rare);
                }
            }
            var slotPlayer = player.GetModPlayer<ModAccessorySlotPlayer>();
            var loader = LoaderManager.Get<AccessorySlotLoader>();
            for (int i = 0; i < slotPlayer.SlotCount; i++)
            {
                var slot = loader.Get(i, player);
                if (slot != null && slot.IsEnabled() && !listedRarity.Contains(slot.FunctionalItem.rare))
                {
                    listedRarity.Add(slot.FunctionalItem.rare);
                }
            }
            ExtraDmg = listedRarity.Count * DamageBonusStep;
        }
    }
}
