using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.ID;
using Terraria;
using AyaMod.Helpers;
using Terraria.DataStructures;
using AyaMod.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using ReLogic.Content;
using AyaMod.Common.Easer;

namespace AyaMod.Content.Items.Cameras
{
    public class SoulCamera : BaseCamera
    {
        public override bool IsLoadingEnabled(Mod mod)
        {

            return false;
        }
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 50;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<SoulCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(0, 0, 78, 0));
            SetCameraStats(0.06f, 156, 1.6f, 0.5f);
            SetCaptureStats(1000, 60);
        }
    }

    public class SoulCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(100, 250, 136);
        public override Color innerFrameColor => new Color(39, 232, 79) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(189, 249, 139).AdditiveColor() * 0.5f;

        public override void OnSpawn(IEntitySource source)
        {
            for(int i = 0; i < 4; i++)
            {
                
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SoulLantern>(), 0, 0, Projectile.owner, Projectile.whoAmI, i);
            }
        }

        public override void OnSnapInSight()
        {
        }
    }

    public class SoulLantern : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;
        public ref float Owner => ref Projectile.ai[0];
        public ref float Offset => ref Projectile.ai[1];
        public ref float OrbitRot => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            
        }
        public override void AI()
        {
            Projectile.timeLeft++;

            float radius = 250f;

            Projectile camera = Main.projectile[(int)Owner];
            if (camera.TypeAlive(ProjectileType<SoulCameraProj>()))
            {
                var player = (camera.ModProjectile as BaseCameraProj).player;

                if (player.AliveCheck(Projectile.Center, 3000))
                {

                }

                Vector2 targetPos = player.Center + (MathHelper.TwoPi / 4f * Offset + OrbitRot).ToRotationVector2() * radius;
                Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.9f);
            }
            else Projectile.Kill();
            OrbitRot += 0.01f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Texture2D ball = Request<Texture2D>(AssetDirectory.Extras + "Ball",AssetRequestMode.ImmediateLoad).Value;

            Color drawColor = new Color(31, 236, 239) * Projectile.Opacity;
            int drawcount = 24;
            Vector2 posOffset = Vector2.UnitY * 5;
            for(int i = 0; i < drawcount; i++)
            {
                
                float factor = (float)i / drawcount;
                if (factor > 0.2f && factor < 0.6f) continue;
                Color color = drawColor.AdditiveColor() * EaseManager.Evaluate(Ease.InCirc, factor, 1f);

                float scale = Projectile.scale * Utils.Remap(factor, 0, 1f, 1.5f, 0.1f);
                Main.EntitySpriteDraw(ball, Projectile.Center + posOffset - Main.screenPosition, null, color, 0f, ball.Size() / 2, scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(ball, Projectile.Center + posOffset - Main.screenPosition, null, drawColor.AdditiveColor() * 0.1f, Projectile.rotation, ball.Size() / 2, Projectile.scale * 1f, 0, 0);
            Main.EntitySpriteDraw(ball, Projectile.Center + posOffset - Main.screenPosition, null, drawColor.AdditiveColor() * 0.4f, Projectile.rotation, ball.Size() / 2, Projectile.scale * 0.4f, 0, 0);
            return false;
        }
    }
}
