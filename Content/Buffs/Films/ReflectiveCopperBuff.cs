using AyaMod.Content.Items.Films.DyeFilms;
using AyaMod.Core;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Buffs.Films
{
    public class ReflectiveCopperBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs_Films + Name;
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex]++;

            player.statDefense += player.DevEffect() ? ReflectiveCopperFilm.DefenceBonusDev : ReflectiveCopperFilm.DefenceBonus;
            if (player.DevEffect())
            {
                player.endurance += ReflectiveCopperFilm.EnduranceBonus;
            }
        }
    }
}
