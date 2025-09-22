using AyaMod.Content.Items.Films.DyeFilms;
using AyaMod.Core;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace AyaMod.Content.Buffs.Films
{
    public class ReflectiveObsidianBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs_Films + Name;
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            bool dev = player.DevEffect();
            player.statDefense += dev ? ReflectiveObsidianFilm.DefenseBonusDev : ReflectiveObsidianFilm.DefenseBonus;
            float dr = dev ? ReflectiveObsidianFilm.DRBonusDev : ReflectiveObsidianFilm.DRBonus;
            player.endurance += dr / 100f;
            float thorn = dev ? ReflectiveObsidianFilm.thornBonusDev : ReflectiveObsidianFilm.thornBonus;
            player.thorns += thorn;
        }
    }
}
