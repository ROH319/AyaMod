using AyaMod.Content.Prefixes.CameraPrefixes;
using AyaMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AyaMod.Core.Globals
{
    public class CameraGlobalItem : GlobalItem
    {
        public float SizeMult = 1f;

        public float StunTimeMult = 1f;

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
            }
        }


        public override bool InstancePerEntity => true;
    }
}
