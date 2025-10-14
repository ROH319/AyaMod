using AyaMod.Content.Items.Cameras;
using AyaMod.Content.Items.Lens;
using AyaMod.Core.BuilderToggles;
using AyaMod.Core.Configs;
using AyaMod.Core.Prefabs;
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

        public StatModifier ChaseSpeedModifier = StatModifier.Default;
        public StatModifier SizeModifier = StatModifier.Default;

        public float SingleTargetMultiplier = 0f;

        public float FlashTimer;

        public float FlashTimerMax = 15;

        public int HoldCameraCounter = 0;
        public int HoldNonCameraCounter = 0;

        public int CameraAltCooldown;

        /// <summary>
        /// 是否有显影液效果
        /// </summary>
        public bool Developing;

        /// <summary>
        /// 是否可以穿墙拍摄
        /// </summary>
        public bool SpiritSnap;

        public bool FreeSnap;

        public override void Load()
        {
            DefaultLens = new DefaultLens();
        }
        public override void Unload()
        {
            DefaultLens = null;

            CheckSnapThrouthWallEvent = null;
            PostUpdateHook = null;
        }

        public override void OnEnterWorld()
        {
            CameraPosition = Player.Center;
        }

        public override void ResetEffects()
        {
            CurrentLens = null;

            ChaseSpeedModifier = StatModifier.Default;
            SizeModifier = StatModifier.Default;

            SingleTargetMultiplier = 0f;

            Developing = false;

            SpiritSnap = false;
        }

        public override void PreUpdate()
        {
            if (Main.myPlayer == Player.whoAmI)
                MouseWorld = Main.MouseWorld;
        }

        public static event ModPlayerEvents.PostUpdateDelegate PostUpdateHook;
        public override void PostUpdate()
        {
            if (Main.myPlayer == Player.whoAmI)
                CameraPosition = Vector2.Lerp(CameraPosition, Player.Center, 0.1f);
            if (FlashTimer > 0) FlashTimer--;
            FlashTimer = Math.Clamp(FlashTimer, 0, FlashTimerMax);

            if (CameraAltCooldown > 0) CameraAltCooldown--;

            if (Player.HeldCamera())
            {
                HoldCameraCounter++;
                HoldNonCameraCounter = 0;
            }
            else
            {
                HoldNonCameraCounter++;
                HoldCameraCounter = 0;
            }

            PostUpdateHook.Invoke(Player);
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

        public delegate bool CheckSnapThrouthWallDelegate(Player player, BaseCameraProj proj);
        public static event CheckSnapThrouthWallDelegate CheckSnapThrouthWallEvent;
        public bool CanSnapThroughWall(BaseCameraProj projectile)
        {
            bool result = false;

            if (SpiritSnap) result = true;

            if (CheckSnapThrouthWallEvent is null) return result;

            foreach(CheckSnapThrouthWallDelegate del in CheckSnapThrouthWallEvent.GetInvocationList())
            {
                result |= del.Invoke(Player, projectile);
            }
            return result;
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (attempt.inLava || attempt.inHoney) return;

            if (attempt.legendary)
            {
                if (Main.rand.NextBool(3))
                    itemDrop = ItemType<WaveCamera>();
            }

        }

        public static float CameraAutoSnapDamageModifier = 0.9f;

        public static float CameraManualSnapDamageModifier = 1.1f;
    }
}
