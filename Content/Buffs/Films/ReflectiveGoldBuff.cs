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
    public class ReflectiveGoldBuff : ModBuff
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
            bool dev = player.DevEffect();
            int defmin = dev ? ReflectiveGoldFilm.DefenceBonusDevMin : ReflectiveGoldFilm.DefenceBonusMin;
            int defmax = dev ? ReflectiveGoldFilm.DefenceBonusDevMax : ReflectiveGoldFilm.DefenceBonusMax;
            int defense = (int)Utils.Remap(time, 1, 8, defmin, defmax);
            player.statDefense += defense;

            int drmin = dev ? ReflectiveGoldFilm.DRBonusDevMin : ReflectiveGoldFilm.DRBonusMin;
            int drmax = dev ? ReflectiveGoldFilm.DRBonusDevMax : ReflectiveGoldFilm.DRBonusMax;
            int endurance = (int)Utils.Remap(time, 1, 8, drmin, drmax);
            player.endurance += endurance / 100f;
        }
    }
}
