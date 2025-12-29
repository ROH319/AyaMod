using AyaMod.Content.Items.Cameras;
using AyaMod.Content.Items.Films;
using AyaMod.Content.Items.Lens;
using AyaMod.Core.BuilderToggles;
using AyaMod.Core.Configs;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using MonoMod.Core.Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AyaMod.Core.ModPlayers
{
    public class CameraPlayer : ModPlayer
    {
        public delegate bool PlayerCameraChecker(Player player, BaseCamera camera);
        public delegate bool PlayerCameraProjChecker(Player player, BaseCameraProj proj);

        public Vector2 CameraPosition = Vector2.Zero;
        public Vector2 MouseWorld;

        public ILens CurrentLens;
        public static ILens DefaultLens;

        public StatModifier FilmEffectChanceModifier = StatModifier.Default;
        public StatModifier AmmoCostModifier = StatModifier.Default;
        public StatModifier FilmCostModifier = StatModifier.Default;
        public StatModifier ChaseSpeedModifier = StatModifier.Default;
        public StatModifier SizeModifier = StatModifier.Default;
        public StatModifier StunTimeModifier = StatModifier.Default;
        public StatModifier AutoSnapDamageModifier = StatModifier.Default;
        public StatModifier ManualSnapDamageModifier = StatModifier.Default;

        public float SingleTargetMultiplier = 0f;

        public float FlashTimer;

        public float FlashTimerMax = 15;

        /// <summary>
        /// 手持相机计时器
        /// </summary>
        public int HoldCameraCounter = 0;
        /// <summary>
        /// 手持非相机计时器
        /// </summary>
        public int HoldNonCameraCounter = 0;

        /// <summary>
        /// 全局拍摄计数器，每次拍摄递增
        /// </summary>
        public int GlobalSnapCounter = 0;

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

        public bool CanReceiveAutoSnapModifier;
        public bool CanReceiveManualSnapModifier;

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

            FilmEffectChanceModifier = StatModifier.Default;
            AmmoCostModifier = StatModifier.Default;
            FilmCostModifier = StatModifier.Default;
            ChaseSpeedModifier = StatModifier.Default;
            SizeModifier = StatModifier.Default;
            StunTimeModifier = StatModifier.Default;
            AutoSnapDamageModifier = StatModifier.Default;
            ManualSnapDamageModifier = StatModifier.Default;

            SingleTargetMultiplier = 0f;

            Developing = false;

            SpiritSnap = false;

            CanReceiveAutoSnapModifier = false;
            CanReceiveManualSnapModifier = false;
        }

        public override void PreUpdate()
        {
            if (Main.myPlayer == Player.whoAmI)
                MouseWorld = Main.MouseWorld;
        }

        public static event ModPlayerEvents.PlayerDelegate PostUpdateHook;
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

            //Main.NewText($"{Player.TryingToHoverUp}");
            //Tile tile = Main.tile[(int)(MouseWorld.X / 16f), (int)(MouseWorld.Y / 16f)];
            //Main.NewText($"{tile.TileType} {tile.TileType > TileID.Count}");
            //Main.NewText($"{Player.velocity.X} {Player.accRunSpeed}");
            PostUpdateHook.Invoke(Player);
        }

        public override void PostUpdateMiscEffects()
        {

        }

        public void OnSnap()
        {
            FlashTimer = FlashTimerMax;
            GlobalSnapCounter++;
            Helper.PlayPitched("Snap", ClientConfig.Instance.SnapVolume, position: Player.Center);
        }

        public ILens GetLens()
        {
            if (CurrentLens == null)
                return DefaultLens;
            else
                return CurrentLens;
        }

        public static event PlayerCameraProjChecker CheckSnapThrouthWallEvent;
        public bool CanSnapThroughWall(BaseCameraProj projectile)
        {
            bool result = false;

            if (SpiritSnap) result = true;

            if (CheckSnapThrouthWallEvent is null) return result;

            foreach(PlayerCameraProjChecker del in CheckSnapThrouthWallEvent.GetInvocationList())
            {
                result |= del.Invoke(Player, projectile);
            }
            return result;
        }

        public static event PlayerCameraChecker AutoSnapCheckerEvent;
        public bool CanAutoSnap(BaseCamera camera)
        {
            bool result = CameraToggle.AutoSnapEnabled;

            if(AutoSnapCheckerEvent is null) return result;

            foreach (PlayerCameraChecker del in AutoSnapCheckerEvent.GetInvocationList())
            {
                result &= del.Invoke(Player, camera);
            }

            return result;
        }

        public float GetStunTime(BaseCamera camera)
        {
            if (camera == null) return 0;
            StatModifier modifier = StunTimeModifier;
            var baseValue = camera.GetStunTime();
            var value = Math.Max(0, modifier.ApplyTo(baseValue));
            return value;
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

        public float GetCameraDamageModifier(bool autoSnap = true)
        {
            if (autoSnap)
                return AutoSnapDamageModifier.ApplyTo(CameraAutoSnapDamageModifier);
            else
                return ManualSnapDamageModifier.ApplyTo(CameraManualSnapDamageModifier);
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            if (ammo.ammo == ItemType<CameraFilm>() && Main.rand.NextFloat() > FilmCostModifier.ApplyTo(1f))
                return false;
            if (Main.rand.NextFloat() > AmmoCostModifier.ApplyTo(1f))
                return false;
            return base.CanConsumeAmmo(weapon, ammo);
        }

        public static float CameraAutoSnapDamageModifier = 0.9f;

        public static float CameraManualSnapDamageModifier = 1.1f;
    }
}
