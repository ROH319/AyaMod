using AyaMod.Core;
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
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class YukaCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 90;

            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<YukaCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Yellow8, Item.sellPrice(0, 0, 78, 0));
            SetCameraStats(0.05f, 132, 1.4f, 0.5f);
            SetCaptureStats(100, 5);
        }
    }

    public class YukaCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(55, 86, 178);
        public override Color innerFrameColor => new Color(115, 209, 234) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(187, 206, 245).AdditiveColor() * 0.5f;
        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;
            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(),Projectile.Center,Vector2.Zero,
                ProjectileType<YukaFlowerMedium>(),Projectile.damage,Projectile.knockBack,Projectile.owner);
        }
    }

    public class YukaFlowerMedium : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name + "_0";
        public ref float Radius => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.SetImmune(20);
            Projectile.scale = 1f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            
        }
        public override void AI()
        {
            Radius = MathHelper.Lerp(Radius, 20, 0.2f);
            //Projectile.UseGravity(1f, 0.15f, 20);
            Projectile.rotation += 0.015f;
        }

        public override void OnKill(int timeLeft)
        {
            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D flower1 = Request<Texture2D>(AssetDirectory.Projectiles + Name + "_1_1", AssetRequestMode.ImmediateLoad).Value;
            Texture2D flower0 = TextureAssets.Projectile[Type].Value;

            //RenderHelper.DrawRing(64, Projectile.Center, Radius * 1.15f, new Color(30, 84, 1).AdditiveColor(), Projectile.rotation, new Vector2(0.2f, 0.3f) * Projectile.scale);
            Texture2D star = TextureAssets.Extra[98].Value;
            Texture2D ball = TextureAssets.Projectile[921].Value;
            Texture2D ball4 = Request<Texture2D>(AssetDirectory.Extras + "Ball4", AssetRequestMode.ImmediateLoad).Value;
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    float dir = i * MathHelper.Pi / 3 + Projectile.rotation;
                    Vector2 pos = Projectile.Center - Main.screenPosition;
                    Main.spriteBatch.Draw(star, pos, null, new Color(30, 84, 1).AdditiveColor(), dir, star.Size() / 2, Projectile.scale * new Vector2(2f, 1.3f), 0, 0);
                }
            }

            Color color = Color.White * Projectile.Opacity;
            for(int i = 0; i < 3; i++)
            {
                float dir = i * MathHelper.TwoPi / 3 + MathHelper.TwoPi / 6;
                Vector2 pos = Projectile.Center + (dir + Projectile.rotation).ToRotationVector2() * Radius * Projectile.scale - Main.screenPosition;

                Main.spriteBatch.Draw(flower1, pos, null, color, dir + Projectile.rotation - MathHelper.Pi / 4, flower1.Size() / 2, Projectile.scale, 0, 0);
            }
            for (int i = 0; i < 3; i++)
            {
                float dir = i * MathHelper.TwoPi / 3;
                Vector2 pos = Projectile.Center + (dir + Projectile.rotation).ToRotationVector2() * Radius * Projectile.scale - Main.screenPosition;

                Main.spriteBatch.Draw(flower0, pos, null, color, dir + Projectile.rotation - MathHelper.Pi / 4, flower0.Size() / 2, Projectile.scale, 0, 0);
            }
            Main.spriteBatch.Draw(ball4, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.2f, Projectile.rotation, ball4.Size() / 2, Projectile.scale * 0.3f, 0, 0);
            //Main.spriteBatch.Draw(ball, Projectile.Center - Main.screenPosition, null, Color.Orchid, Projectile.rotation, ball.Size() / 2, Projectile.scale * 1f, 0, 0);
            return false;
        }
    }
}
