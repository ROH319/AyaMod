using AyaMod.Content.Items.Films;
using AyaMod.Content.Items.Lens;
using AyaMod.Content.Items.Testing;
using AyaMod.Content.Prefixes.CameraPrefixes;
using AyaMod.Content.UI;
using AyaMod.Core.BuilderToggles;
using AyaMod.Core.Configs;
using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
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
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = true;
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
        public override bool CanShoot(Player player)
        {
            //不使用原版的弹药消耗
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override bool CanRightClick() => Main.mouseItem.IsAir;
        public override void RightClick(Player player)
        {
            base.RightClick(player);
            if (FilmContainerUI.Instance.Visible && FilmContainerUI.Instance.OpenTimer.AnyPositive)
                FilmContainerUI.Instance.Close();
            else
                FilmContainerUI.Instance.Open(player.Camera().GetCameraSlot(CameraStats.FilmSlot));
        }
        public override bool ConsumeItem(Player player) => false;

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

            if (!player.GetModPlayer<CameraPlayer>().CanAutoSnap(this))
            {
                if (!player.controlUseItem && player.itemTime > 2)
                    player.itemTime = player.itemAnimation = 0;
                if (player.itemTime == 2)
                    if (!player.releaseUseItem)
                        player.itemTime = player.itemAnimation = 3;
                if(player.Aya().itemTimeLastFrame == 4 && player.itemTime == 3)
                {
                    Helper.PlayPitched("FocusReady", ClientConfig.Instance.ManualSnapVolume, 0.3f, position: player.Center);

                }
            }
            if(player.itemTime == 1)
            {
                player.Aya().NotUsingCameraTimer = 0;
            }
            //Main.NewText($"{player.itemTime} {player.itemAnimation}");
        }

        public virtual void ShootCameraProj(Player player, EntitySource_ItemUse_WithAmmo source, int damage, float knockback)
        {
            var p = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, Item.shoot, damage, knockback, Main.myPlayer);
           
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return player.GetModPlayer<CameraPlayer>().CanAutoSnap(this);
        }
        public override bool? UseItem(Player player)
        {
            PickFilm(player, out bool canshoot);
            return canshoot;
        }

        /// <summary>
        /// 从玩家胶卷槽和物品栏选择胶卷
        /// </summary>
        /// <param name="player"></param>
        /// <param name="canShoot"></param>
        /// <param name="consume">是否消耗</param>
        /// <returns></returns>
        public virtual List<Item> PickFilm(Player player, out bool canShoot, bool consume = true, int defaultSlot = -1)
        {
            if (!ItemLoader.NeedsAmmo(Item, player))
                consume = false;

            var slot = defaultSlot <= 0 ? player.Camera().GetCameraSlot(CameraStats.FilmSlot) : defaultSlot;
            var list = player.ChooseFilmsFromVault(Item, slot);
            int leftSlot = slot - list.Count;
            if (leftSlot > 0)
                list.AddRange(player.ChooseFilmsFromInv(Item, leftSlot));

            canShoot = list.Count > 0 || !consume;

            if (!consume) return list;

            foreach (var ammo in list)
            {
                //DeathSickle是随便写的弹幕ID ，这个方法里没用到projtoshoot
                if (ammo.consumable && !player.IsAmmoFreeThisShot(Item, ammo, ProjectileID.DeathSickle))
                {
                    CombinedHooks.OnConsumeAmmo(player, Item, ammo);
                    ammo.stack--;
                    if(ammo.stack < 0)
                    {
                        ammo.active = false;
                        ammo.TurnToAir();
                    }
                }
            }

            return list;
        }

        public virtual float GetStunTime() => Item.knockBack * 2 * Item.GetGlobalItem<CameraGlobalItem>().StunTimeMult;

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            var films = PickFilm(player, out var _, false);
            foreach(var film in films)
            {
                if (film.ModItem is not BaseFilm) continue;
                BaseFilm f = film.ModItem as BaseFilm;
                damage = damage.CombineWith(f.DamageModifier);
            }

            if (player.HasEffect<NoDmgModifier>()) return;

            CameraPlayer cp = player.GetModPlayer<CameraPlayer>();
            if (cp.CanAutoSnap(this))
            {
                if (cp.CanReceiveAutoSnapModifier)
                    damage *= cp.GetCameraDamageModifier();
            }
            else
            {
                if (cp.CanReceiveManualSnapModifier)
                    damage *= cp.GetCameraDamageModifier(false);
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var films = PickFilm(Main.LocalPlayer, out var _, false);
            foreach(var film in films)
            {
                tooltips.Add(new TooltipLine(Mod, $"usingFilms:{film.ModItem.Name}",  "正在使用胶卷".WrapWithColorCode(Main.DiscoColor) + $"[i:{film.type}]" + $"{film.Name}".WrapWithColorCode(Main.DiscoColor)));
            }
        }
    }

    //public class CameraUseStyleItem : GlobalItem
    //{

    //    public static int CameraUseStyle;
    //    public override void Load()
    //    {
    //        CameraUseStyle = ItemLoader.RegisterUseStyle(Mod, "CameraUseStyle");
    //    }
    //    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    //    {
    //        return lateInstantiation && entity.useStyle == CameraUseStyle;
    //    }
    //    public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
    //    {
    //        //Texture2D tex = TextureAssets.Item[item.type].Value;
    //        //var origin = tex.Size() / 2;
    //        //var loc = new Vector2(0, tex.Height);
    //        //var toloc = (loc - origin) / 2f;
    //        //Vector2 tomouse = Main.MouseWorld - player.Center;
    //        //Vector2 itemCenter = player.Center + tomouse.SafeNormalize(Vector2.Zero) * 4;
    //        //player.itemRotation = tomouse.ToRotation();
    //        //Main.NewText($"{player.itemRotation}");
    //        //player.itemLocation.X = itemCenter.X + (float)(toloc.RotatedBy(player.itemRotation).X * player.direction);
    //        //player.itemLocation.Y = itemCenter.Y + (float)(toloc.RotatedBy(player.itemRotation).Y);
    //        //Vector2 vec = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / 56];
    //        //player.itemLocation += vec;
    //        //player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)Math.PI * -3f / 4f * (float)player.direction);
    //        //player.FlipItemLocationAndRotationForGravity();
    //    }
    //}
}
