using AyaMod.Content.Items.Cameras;
using AyaMod.Content.Items.Lens;
using AyaMod.Core.BuilderToggles;
using AyaMod.Core.Configs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AyaMod.Core.ModPlayers
{
    public class CameraPlayer : ModPlayer
    {
        public Vector2 CameraPosition = Vector2.Zero;
        public Vector2 MouseWorld;

        public ILens CurrentLens;
        public static ILens DefaultLens;

        public StatModifier SizeBonus;

        public float FlashTimer;

        public float FlashTimerMax = 15;

        public int CameraAltCooldown;

        public override void Load()
        {
            DefaultLens = new DefaultLens();
        }
        public override void Unload()
        {
            DefaultLens = null;
        }

        public override void OnEnterWorld()
        {
            CameraPosition = Player.Center;
        }

        public override void ResetEffects()
        {
            CurrentLens = null;

            SizeBonus = new StatModifier();
        }

        public override void PreUpdate()
        {
            if (Main.myPlayer == Player.whoAmI)
                MouseWorld = Main.MouseWorld;
        }

        public override void PostUpdate()
        {
            if (Main.myPlayer == Player.whoAmI)
                CameraPosition = Vector2.Lerp(CameraPosition, Player.Center, 0.1f);
            if (FlashTimer > 0) FlashTimer--;
            FlashTimer = Math.Clamp(FlashTimer, 0, FlashTimerMax);

            if (CameraAltCooldown > 0) CameraAltCooldown--;
        }

        public override void PostUpdateMiscEffects()
        {

        }

        public void OnSnap()
        {
            FlashTimer = FlashTimerMax;

            Helper.PlayPitched("Snap", ClientConfig.Instance.SnapVolume, position: Player.Center);
        }

        public ILens GetLens()
        {
            if (CurrentLens == null)
                return DefaultLens;
            else
                return CurrentLens;
        }

        public bool CanSnapThroughWall() => false;

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (attempt.inLava || attempt.inHoney) return;

            if (attempt.legendary)
            {
                if (Main.rand.NextBool(3))
                    itemDrop = ItemType<WaveCamera>();
            }

        }

        public static float CameraAutoSnapDamageModifier = 0.8f;

        public static float CameraManualSnapDamageModifier = 1.1f;
    }
}
