using AyaMod.Core;
using AyaMod.Core.Globals;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Buffs
{
    public class MadnessBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public static int DPS = 24;
        public override void Load()
        {
            AyaGlobalNPC.PostAIHook += MadnessAI;
        }

        public static void MadnessAI(NPC npc)
        {
            if(!npc.HasBuff(BuffType<MadnessBuff>())) return;
            if ((Main.GameUpdateCount + npc.whoAmI) % 20 != 0) return;
            foreach (var n in Main.ActiveNPCs)
            {
                if(n == npc || !n.CanBeChasedBy()) continue;
                Rectangle rect = n.getRect();
                Rectangle rectangle = npc.getRect();
                if(rect.Intersects(rectangle))
                {
                    int damage = DPS / 60;
                    n.SimpleStrikeNPC(npc.damage / 2, (npc.Center.X < n.Center.X).ToDirectionInt());
                }
            }
        }

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Aya().Madness = true;
        }
    }
}
