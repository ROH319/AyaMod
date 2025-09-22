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
    public class ReflectiveSilverBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs_Films + Name;
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            int time = player.buffTime[buffIndex];
            int max = player.DevEffect() ? ReflectiveSilverFilm.DefenceBonusDevMax : ReflectiveSilverFilm.DefenceBonusMax;
            int defense = (int)Utils.Remap(time, 1, 8, ReflectiveSilverFilm.DefenceBonusMin, max);
            player.statDefense += defense;
            if (player.DevEffect())
            {
                int endurance = (int)Utils.Remap(time, 1, 8, ReflectiveSilverFilm.DRBonusMin, ReflectiveSilverFilm.DRBonusMax);
                player.endurance += endurance / 100f;
            }
        }
    }
}
