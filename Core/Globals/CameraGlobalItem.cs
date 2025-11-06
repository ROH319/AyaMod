using AyaMod.Content.Prefixes.CameraPrefixes;
using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using static AyaMod.Core.ModPlayers.ModPlayerEvents;

namespace AyaMod.Core.Globals
{
    public class CameraGlobalItem : GlobalItem
    {
        public static LocalizedText StunTimeTooltip { get; private set; }

        public override void Load()
        {
            StunTimeTooltip = Mod.GetLocalization($"Items.Extras.{nameof(StunTimeTooltip)}");
        }

        public float SizeMult = 1f;

        public float StunTimeMult = 1f;

        public delegate bool ItemChecker(Item item);
        public static event ItemChecker OnCanRightClick = (i) => false;
        public override bool CanRightClick(Item item)
        {
            bool result = base.CanRightClick(item);

            foreach (ItemChecker checker in OnCanRightClick.GetInvocationList())
            {
                result |= checker(item);
            }

            return result;
        }

        public static event ModPlayerEvents.PlayerItemDelegate OnRightClick = (p, i) => { };
        public override void RightClick(Item item, Player player)
        {
            foreach(PlayerItemDelegate g in OnRightClick.GetInvocationList())
            {
                g(player, item);
            }
        }

        public delegate bool AmmoConsumer(Item weapon, Item ammo, Player player);
        public static event AmmoConsumer CanConsumeAmmoChecker = (w, a, p) => true;
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            foreach(AmmoConsumer checker in CanConsumeAmmoChecker.GetInvocationList())
            {
                if (!checker(weapon, ammo, player))
                    return false;
            }
            return base.CanConsumeAmmo(weapon, ammo, player);
        }

        public override int ChoosePrefix(Item item, UnifiedRandom rand)
        {
            if (item.DamageType != ReporterDamage.Instance)
                return base.ChoosePrefix(item, rand);

            int prefix = 0;
            var wr = new WeightedRandom<int>(rand);

            void AddCategory()
            {
                IReadOnlyList<ModPrefix> list = PrefixLoader.GetPrefixesInCategory(PrefixCategory.Custom);
                foreach (ModPrefix modPrefix in list.Where(x => x.CanRoll(item)))
                {
                    wr.Add(modPrefix.Type, modPrefix.RollChance(item));
                }
            }

            AddCategory();

            for (int i = 0; i < 50; i++)
            {
                prefix = wr.Get();
            }

            return prefix;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.DamageType == ReporterDamage.Instance)
            {
                int index = tooltips.FindIndex(line => line.Name == "PrefixSpeed");
                if (index != -1)
                {
                    tooltips.RemoveAt(index);
                }
                int kbindex = tooltips.FindIndex(line => line.Name == "Knockback");
                if(kbindex != -1)
                {
                    tooltips.RemoveAt(kbindex);
                    int stuntime = Main.LocalPlayer.TryGetModPlayer<CameraPlayer>(out var camPlayer) ? (int)(camPlayer.GetStunTime(item.ModItem as BaseCamera)) : 0;
                    if (stuntime > 0 && stuntime < 1000)
                    {
                        float stuntimeInSeconds = MathF.Round(stuntime / 60f, 2);
                        tooltips.Insert(kbindex, new TooltipLine(Mod, "StunTime", StunTimeTooltip.WithFormatArgs(stuntime, stuntimeInSeconds).Value));
                    }
                }
            }
        }


        public override bool InstancePerEntity => true;
    }
}
