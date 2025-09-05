using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Enums;
using static AyaMod.Core.ModPlayers.AyaPlayer;

namespace AyaMod.Content.Items.Accessories.Movements
{
    [PlayerEffect]
    public class GaleGeta : BaseAccessories
    {
        public override string Texture => AssetDirectory.Accessories + "Movements/" + Name;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(gold: 1));

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accRunSpeed = 6.75f;
            player.rocketBoots = (player.vanityRocketBoots = 2);
            player.moveSpeed += 0.08f;
            player.jumpSpeedBoost += 1.4f;
            Player.jumpHeight += 8;
            player.autoJump = true;

            player.AddEffect<GaleGeta>();
        }

        public static void AddDash(Player player)
        {
            var modPlayer = player.Aya();
            modPlayer.AyaDash = DashType.GaleGeta0;
            modPlayer.HasDash = true;

        }

        public static void GetaDash0(Player player, int direction)
        {
            AyaPlayer modPlayer = player.Aya();

            float speed = 15f;
            float dashDirection;
            switch (direction)
            {
                case AyaPlayer.DashLeft:
                case AyaPlayer.DashRight:
                    {
                        dashDirection = direction == AyaPlayer.DashRight ? 1 : -1;
                        speed *= dashDirection;
                        break;
                    }
                default:
                    return;
            }
            //if (direction > 0 && player.velocity.X > speed) return;
            //if (direction < 0 && player.velocity.X < -speed) return;
            player.velocity.X = speed;
            player.velocity.Y = 0.000001f;

            player.direction = (int)dashDirection;
            modPlayer.DashDelay = 40;
            modPlayer.DashTimer = 10;

            Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, player.velocity, ModContent.ProjectileType<GaleGetaDash>(), 0, 0, player.whoAmI);
            //player.dashDelay = 30;
        }
    }

    public class GaleGetaDash : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(4, 25);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 80;
            Projectile.scale = 1.2f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.Alive())
                Projectile.Kill();
            //if (player.Aya().DashDelay > 0)
            //{
            //    Projectile.velocity = player.Center - Projectile.Center;
            //}
            float factor = Utils.Remap(player.Aya().DashDelay, 0, 20, 0.94f, 0.99f);
            Projectile.velocity *= factor;
            if (Projectile.timeLeft < 40)
                Projectile.Opacity -= 0.07f;
            if (Projectile.Opacity < 0.05f) Projectile.Kill();
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Request<Texture2D>(AssetDirectory.Extras + "spellBulletAa013", AssetRequestMode.ImmediateLoad).Value;
            Vector2 origin = texture.Size() / 2;
            Vector2 baseScale = new Vector2(0.5f, 0.5f);

            for(int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                float factor = 1f - (float)i / Projectile.oldPos.Length;
                float rot = (i == 0 ? Projectile.rotation : /*(Projectile.oldPos[i - 1] - Projectile.oldPos[i]).ToRotation()*/Projectile.oldRot[i]) 
                    + MathF.Sin(Main.GameUpdateCount * 0.5f) * (i % 2 == 0 ? 1 : -1);
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Color color = Color.White.AdditiveColor() * factor * 0.4f * Projectile.Opacity;

                float scaleFactor = Utils.Remap(factor, 0, 1f, 0.5f, 1f);
                var trailScale = baseScale * scaleFactor * Projectile.scale;

                Main.spriteBatch.Draw(texture, drawPos, null, color, rot, origin, trailScale, 0, 0);

            }
            return true;
        }
    }
}
