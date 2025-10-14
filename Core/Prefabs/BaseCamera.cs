using AyaMod.Content.Items.Films;
using AyaMod.Content.Items.Lens;
using AyaMod.Content.Items.Testing;
using AyaMod.Content.Prefixes.CameraPrefixes;
using AyaMod.Core.BuilderToggles;
using AyaMod.Core.Configs;
using AyaMod.Core.ModPlayers;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AyaMod.Core.Prefabs
{
    public abstract class BaseCamera : ModItem
    {
        public override string Texture => AssetDirectory.Cameras + Name;

        public CameraStat CameraStats;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = true;
        }

        public sealed override void SetDefaults()
        {
            Item.DamageType = ReporterDamage.Instance;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAmmo = ModContent.ItemType<CameraFilm>();
            SetOtherDefaults();
        }

        public virtual void SetOtherDefaults() { }
        public void SetCameraStats(float chasefactor, float size, float maxFocusScale, float slowFactor = 0.3f, int filmSlot = 1)
        {
            CameraStats.ChaseFactor = chasefactor;
            CameraStats.Size = size;
            CameraStats.MaxFocusScale = maxFocusScale;
            CameraStats.FilmSlot = filmSlot;
            CameraStats.SlowFactor = slowFactor;
        }

        public void SetCaptureStats(int maxCaptureDamage, int captureCooldown)
        {
            CameraStats.CameraDamage = maxCaptureDamage;
            CameraStats.CaptureCooldown = captureCooldown;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override bool AltFunctionUse(Player player) => player.Camera().CameraAltCooldown <= 0;

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] <= 0)
            {
                ShootCameraProj(player, new EntitySource_ItemUse_WithAmmo(player, Item, ModContent.ItemType<CameraFilm>()), player.GetWeaponDamage(Item), Item.knockBack);
            }
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.type == Item.shoot) projectile.timeLeft = 2;
            }

            if (!CameraToggle.AutoSnapEnabled)
            {
                if (!player.controlUseItem && player.itemTime > 2)
                    player.itemTime = 0;
                if (player.itemTime == 2)
                    if (!player.releaseUseItem)
                        player.itemTime = 3;
                if(player.Aya().itemTimeLastFrame == 4 && player.itemTime == 3)
                {
                    Helper.PlayPitched("FocusReady", ClientConfig.Instance.ManualSnapVolume, 0.3f, position: player.Center);

                }
            }
            if(player.itemTime == 1)
            {
                player.Aya().NotUsingCameraTimer = 0;
            }
            //Main.NewText($"{player.itemTime}");
        }

        public virtual void ShootCameraProj(Player player, EntitySource_ItemUse_WithAmmo source, int damage, float knockback)
        {
            var p = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, Item.shoot, damage, knockback, Main.myPlayer);
           
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return CameraToggle.AutoSnapEnabled;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.HasEffect<NoDmgModifier>()) return;

            if (CameraToggle.AutoSnapEnabled)
            {
                damage *= CameraPlayer.CameraAutoSnapDamageModifier;
            }
            else
            {
                damage *= CameraPlayer.CameraManualSnapDamageModifier;
            }
        }
    }
}
