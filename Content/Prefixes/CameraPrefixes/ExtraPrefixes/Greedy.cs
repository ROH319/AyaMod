using AyaMod.Core.Globals;
using AyaMod.Helpers;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class Greedy() : ExtraCameraPrefix(damageMult:1.07f,focusSpeedMult:0.93f,critBonus:7,sizeMult:1.07f,stunMult:1.07f,valueMult:1.07f)
    {
        public override void Load()
        {
            AyaGlobalNPC.OnNPCKill += GreedyLoot;
        }

        public static void GreedyLoot(NPC npc)
        {
            Player player = Main.player[npc.lastInteraction];
            if (player == null || !player.Alive()) return;
            if(player.HeldItem.prefix != PrefixType<Greedy>()) return;
            if (Main.rand.Next(100) > ExtraLootChance) return;
            int extraLoots = npc.boss ? ExtraLootBoss : ExtraLootCount;
            for (int i = 0; i < extraLoots; i++)
                npc.NPCLoot_DropItems();
        }
        public static int ExtraLootChance = 7;
        public static int ExtraLootCount = 13;
        public static int ExtraLootBoss = 6;
    }
}
