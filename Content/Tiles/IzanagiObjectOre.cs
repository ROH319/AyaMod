using AyaMod.Core;
using AyaMod.Core.Globals;
using System.Threading;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Tiles
{
    public class IzanagiObjectOre : ModTile
    {
        public override string Texture => AssetDirectory.Tiles + Name;
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
            Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
            Main.tileOreFinderPriority[Type] = 410; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
            Main.tileShine2[Type] = true; // Modifies the draw color slightly.
            Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(152, 171, 198), name);

            DustType = DustID.Platinum;
            VanillaFallbackOnModDeletion = TileID.Silver;
            HitSound = SoundID.Tink;
        }
        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = Main.DiscoColor;
            return true;
        }
    }

    public class IzanagiOreSystem : ModSystem
    {
        public static LocalizedText BlessWithIzanagiOreMessage { get; set; }

        public override void Load()
        {
            AyaGlobalNPC.OnNPCKill += MechBossBless;
        }
        public override void Unload()
        {
            AyaGlobalNPC.OnNPCKill -= MechBossBless;
        }
        public static void MechBossBless(NPC npc)
        {
            if (npc.type is NPCID.TheDestroyer or NPCID.Retinazer or NPCID.Spazmatism or NPCID.SkeletronPrime && !NPC.downedMechBossAny)
                GetInstance<IzanagiOreSystem>().BlessWorldWithIzanagiOre();
        }

        public override void SetStaticDefaults()
        {
            BlessWithIzanagiOreMessage = Mod.GetLocalization($"WorldGen.{nameof(BlessWithIzanagiOreMessage)}");
        }

        public void BlessWorldWithIzanagiOre()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(BlessWithIzanagiOreMessage.Value, 50, 255, 130);
                else if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(BlessWithIzanagiOreMessage.ToNetworkText(), new Color(50,255,130));

                int splotches = (int)(100 * (Main.maxTilesX / 4200f));
                int highestY = (int)Utils.Lerp(Main.rockLayer, Main.UnderworldLayer, 0.5);
                for (int iteration = 0; iteration < splotches; iteration++)
                {
                    // Find a point in the lower half of the rock layer but above the underworld depth.
                    int i = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                    int j = WorldGen.genRand.Next(highestY, Main.UnderworldLayer);

                    WorldGen.OreRunner(i, j, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), (ushort)TileType<IzanagiObjectOre>());
                }
            });
        }
    }
}
