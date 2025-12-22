using AyaMod.Core;
using AyaMod.Core.Prefabs;
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

namespace AyaMod.Content.Items.Cameras
{
    public class Luminvore : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 30;

            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<LuminvoreProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(0, 0, 72, 0));
            SetCameraStats(0.04f, 104, 2f);
            SetCaptureStats(1000, 60);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(ItemID.ShadowScale, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class LuminvoreProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(66, 0, 181);
        public override Color innerFrameColor => new Color(0,191,51) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(73,57,90) * 0.5f;

        public override void OnSnapInSight()
        {
            int dustamount = 36;
            float speed = 10f;
            //for (int i = 0; i < dustamount; i++)
            //{
            //    float distFactor = Main.rand.NextFloat(0.3f, 1f);
            //    Vector2 dir = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
            //    Vector2 pos = Projectile.Center + dir * distFactor * 50;
            //    Vector2 vel = dir * speed * distFactor;
            //    var d = Dust.NewDustPerfect(pos, DustID.PurpleTorch, vel, 0, Scale: 2f);
            //    d.noGravity = true;
            //}
            var pos = AyaUtils.GetCameraRect(Projectile.Center, Projectile.rotation, Size * 0.75f, Size * 1.4f * 0.75f);

            for (int i = 0; i < dustamount; i++)
            {
                float distFactor = Main.rand.NextFloat(1f, 1f) * 0.65f;
                Vector2 p = pos[0];
                float factor = i / (float)dustamount;
                if (factor < 0.25f) p = Vector2.Lerp(pos[0], pos[1], Utils.Remap(factor, 0, 0.25f, 0f, 1f));
                else if (factor < 0.5f) p = Vector2.Lerp(pos[1], pos[3], Utils.Remap(factor, 0.25f, 0.5f, 0f, 1f));
                else if (factor < 0.75f) p = Vector2.Lerp(pos[3], pos[2], Utils.Remap(factor, 0.5f, 0.75f, 0f, 1f));
                else p = Vector2.Lerp(pos[2], pos[0], Utils.Remap(factor, 0.75f, 1f, 0f, 1f));
                var d = Dust.NewDustPerfect(p, DustID.PurpleTorch, Projectile.DirectionToSafe(p) * speed * distFactor, Scale: 2.5f);
                d.noGravity = true;
            }

            if (!Projectile.MyClient()) return;

            if (++EffectCounter >= 3)
            {
                int damage = (int)(Projectile.damage * 0.4f);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LuminvoreVeil>(), damage, 0, Projectile.owner);
                EffectCounter = 0;
            }
        }
    }

    public class LuminvoreVeil : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Mist";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(20);
            Projectile.timeLeft = 60 * 3;
            Projectile.scale = 1.3f;
            Projectile.ArmorPenetration = 20;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = AyaUtils.RandAngle;
        }
        public override void AI()
        {
            float factor = Projectile.TimeleftFactorPositive();

            Projectile.Opacity = 
                Utils.Remap(factor, 0, 0.125f, 0f, 1f)
                * Utils.Remap(factor, 0.125f, 1f, 1f, 0f);
            Projectile.rotation += 0.02f;

            int dustamount = (int)(7 * Projectile.scale);
            float alphafactor = Utils.Remap(factor, 0, 1f, 100, 255);
            float lengthfactor = Utils.Remap(factor, 0, 1f, 1.3f, 0.5f);
            for(int i = 0;i< dustamount; i++)
            {
                Vector2 pos = Projectile.Center + (MathHelper.TwoPi / dustamount * i).ToRotationVector2().RotateRandom(0.5f) * Main.rand.NextFloat(30,90) * lengthfactor * Projectile.scale;
                Vector2 vel = pos.DirectionToSafe(Projectile.Center).RotateRandom(0.3f) * Main.rand.NextFloat(1f,2.4f);
                var d = Dust.NewDustPerfect(pos, DustID.PurpleTorch, vel, (int)alphafactor,Scale:1.2f);
                d.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Color color = Color.White * 1f * Projectile.Opacity;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, RenderHelper.ReverseSubtract, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);

            int drawcount = 10;
            float radius = 30;
            for(int i = 0; i < drawcount; i++)
            {
                Vector2 offset = (MathHelper.TwoPi / drawcount * i).ToRotationVector2() * radius;

                Main.spriteBatch.Draw(texture, Projectile.Center + offset - Main.screenPosition, null, color * (4f / drawcount), Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);

            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);


            return false;
        }
    }
}
