using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using AyaMod.Core;
using Microsoft.Xna.Framework.Audio;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;

namespace AyaMod.Content.Items.Cameras
{
    public class LightRipper : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;
            Item.damage = 22;

            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.shoot = ModContent.ProjectileType<LightRipperProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 0, 24, 0));
            SetCameraStats(0.03f, 92, 2f);
            SetCaptureStats(100, 5);

            base.SetOtherDefaults();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(ItemID.Amethyst, 3)
                .AddTile(TileID.Anvils)
                .Register();
                
        }
    }

    public class LightRipperProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(120,69,208);
        public override Color innerFrameColor => new Color(172,124,255) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(210,183,255);
        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;

            float rot = AyaUtils.RandAngle;
            Vector2 dir = rot.ToRotationVector2();
            Vector2 ndir = dir.RotatedBy(MathHelper.PiOver2);
            int rotdir = Main.rand.NextBool() ? 1 : -1;
            float dmgmult = 0.33f;
            for(int i = -1; i < 2; i++)
            {
                Vector2 pos = Projectile.Center + ndir * i * 40 - dir * 100;
                float scale = 1.4f;
                if (i != 0) scale *= 0.5f;
                var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, dir * 7, ModContent.ProjectileType<LightRipperSlash>(), (int)(Projectile.damage * dmgmult), Projectile.knockBack, Projectile.owner, scale, rotdir * 0.02f);
                p.timeLeft = 20;
            }
        }
    }

    public class LightRipperSlash : ModProjectile
    {
        public override string Texture => AssetDirectory.VanillaProjPath(974);

        public static BlendState _multiplyBlendState;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 30;
            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[2] = Projectile.velocity.Length();
            Projectile.ai[2] = Projectile.timeLeft;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = -1;
                SoundEngine.PlaySound((SoundStyle?)SoundID.Item60.WithVolumeScale(2f * Projectile.ai[0]), Projectile.Center);
                //if (soundEffectInstance != null)
                //    soundEffectInstance.Volume *= 0.15f * Projectile.ai[0];
            }

            Projectile.localAI[0] += 1f;

            float factor = Projectile.localAI[0] / Projectile.ai[2];

            Projectile.localAI[1] = Utils.Remap(factor, 0, 1f, 2.5f, 0.1f);
            float scaleFactor = Utils.Remap(factor, 0, 1f, 1f, 0.3f);

            Projectile.scale = Projectile.ai[0] * scaleFactor;
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 12)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Projectile.velocity = Projectile.velocity.Length(Projectile.localAI[2] * Projectile.localAI[1]).RotatedBy(Projectile.ai[1]);
            Projectile.rotation = Projectile.velocity.ToRotation();
            
            float rot = Projectile.rotation;
            float num = 46f * Projectile.scale;
            Vector2 vector = rot.ToRotationVector2();
            float num2 = Projectile.localAI[0] / 10f;
            if (num2 >= 0f && num2 <= 1f)
            {
                var pos = Vector2.Lerp(Projectile.Center - vector * num, Projectile.Center + vector * num, Projectile.localAI[0] / 36f);
                var vel = vector.RotatedBy((float)Math.PI * 2f * Main.rand.NextFloatDirection() * 0.02f) * 8f * Main.rand.NextFloat();
                Dust dust = Dust.NewDustPerfect(pos, 278, vel, 0, new Color(60, 0, 150), 0.7f * num2);
                dust.noGravity = true;
                dust.noLight = (dust.noLightEmittence = true);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();


            _multiplyBlendState ??= new BlendState
            {
                ColorBlendFunction = BlendFunction.ReverseSubtract,
                ColorDestinationBlend = Blend.One,
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaBlendFunction = BlendFunction.ReverseSubtract,
                AlphaDestinationBlend = Blend.One,
                AlphaSourceBlend = Blend.SourceAlpha
            };

            BlendState multiplyBlendState = _multiplyBlendState;
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, multiplyBlendState, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            Vector2 position = Projectile.Center - Main.screenPosition;
            //LoadProjectile(proj.type);
            Asset<Texture2D> asset = TextureAssets.Projectile[Projectile.type];
            Rectangle rectangle = asset.Frame(1, 13, 0, Projectile.frame);
            Vector2 vector = rectangle.Size() / 2f;
            Vector2 vector2 = new Vector2(0.7f, 0.7f) * Projectile.scale;
            float num = Utils.Remap(Projectile.frame, 0f, 3f, 0f, 1f) * Utils.Remap(Projectile.frame, 4f, 12f, 1f, 0f);
            Rectangle value = asset.Frame(1, 13, 0, 12);
            Vector2 origin = vector + new Vector2(0f, 0f);

            float factor = Projectile.localAI[0] / Projectile.ai[2];
            float alphaFadeout = Utils.Remap(factor, 0.5f, 1f, 1f, 0f);

            Main.spriteBatch.Draw(asset.Value, position, value, Color.White * 0.3f * num * alphaFadeout, Projectile.rotation, origin, new Vector2(1f, 6f) * vector2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(asset.Value, position, value, Color.White * 0.3f * num * alphaFadeout, Projectile.rotation, origin, new Vector2(2f, 2f) * vector2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(asset.Value, position, rectangle, Color.White * alphaFadeout, Projectile.rotation, vector, vector2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(asset.Value, position, rectangle, Color.White * alphaFadeout, Projectile.rotation, vector, vector2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(asset.Value, position, rectangle, Color.White * alphaFadeout, Projectile.rotation, vector, vector2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(asset.Value, position, rectangle, Color.White * alphaFadeout, Projectile.rotation, vector, vector2, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            Main.spriteBatch.Draw(asset.Value, position, rectangle, Color.Magenta * alphaFadeout, Projectile.rotation, vector, vector2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(asset.Value, position, rectangle, Color.Magenta * alphaFadeout, Projectile.rotation, vector, vector2, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            return false;
        }
    }
}
