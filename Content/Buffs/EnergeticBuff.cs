using AyaMod.Core;
using AyaMod.Core.ModPlayers;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;

namespace AyaMod.Content.Buffs
{
    public class EnergeticBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override LocalizedText Description => base.Description.WithFormatArgs(RegenBonus, DefenseBonus, MovementBonus);
        public static int RegenBonus = 8;
        public static int DefenseBonus = 6;
        public static int MovementBonus = 20;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            CameraPlayer cp = player.Camera();
            cp.CanReceiveAutoSnapModifier = false;
            player.lifeRegen += RegenBonus;
            player.statDefense += DefenseBonus;
            player.moveSpeed += MovementBonus / 100f;
        }
    }
}
