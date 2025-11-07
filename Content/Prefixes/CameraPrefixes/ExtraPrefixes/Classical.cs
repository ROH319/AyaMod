using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using Terraria;

namespace AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes
{
    public class Classical() : ExtraCameraPrefix(damageMult: 1.15f, focusSpeedMult: 0.9f, critBonus: 10, sizeMult: 1.15f, valueMult: 1.5f)
    {

        public override void Load()
        {
            CameraPlayer.AutoSnapCheckerEvent += ClassicalSnap;
        }

        public bool ClassicalSnap(Player player, BaseCamera camera) => camera.Item.prefix != PrefixType<Classical>();

    }
}
