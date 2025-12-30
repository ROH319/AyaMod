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
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AyaMod.Content.Items.Cameras
{
    public class GoldenCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 18;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<GoldenCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.03f, 80, 2f);
            SetCaptureStats(1000, 60);
            base.SetOtherDefaults();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 10)
                .AddIngredient(ItemID.Lens, 3)
                .AddIngredient(ItemID.FallenStar,3)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.PlatinumBar, 10)
                .AddIngredient(ItemID.Lens, 3)
                .AddIngredient(ItemID.FallenStar,3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GoldenCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(255,222,111);
        public override Color innerFrameColor => new Color(181,194,217) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(255,251,179).AdditiveColor() * 0.5f;
        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;


            var pos = AyaUtils.GetCameraRect(Projectile.Center, Projectile.rotation, Size * 0.75f, Size * 1.4f * 0.75f);

            int dustamount = 36;
            for (int i = 0; i < dustamount; i++)
            {
                float distFactor = Main.rand.NextFloat(1f, 1f) * 0.65f;
                Vector2 p = pos[0];
                float factor = i / (float)dustamount;
                if (factor < 0.25f) p = Vector2.Lerp(pos[0], pos[1], Utils.Remap(factor, 0, 0.25f, 0f, 1f));
                else if (factor < 0.5f) p = Vector2.Lerp(pos[1], pos[3], Utils.Remap(factor, 0.25f, 0.5f, 0f, 1f));
                else if (factor < 0.75f) p = Vector2.Lerp(pos[3], pos[2], Utils.Remap(factor, 0.5f, 0.75f, 0f, 1f));
                else p = Vector2.Lerp(pos[2], pos[0], Utils.Remap(factor, 0.75f, 1f, 0f, 1f));
                var d = Dust.NewDustPerfect(p, DustID.GoldFlame, Projectile.DirectionToSafe(p) * 6 * distFactor, Scale: 2f);
                d.noGravity = true;
            }

            int damage = (int)(Projectile.damage * 0.6f);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GoldenFlash>(), damage, Projectile.knockBack,Projectile.owner);
        }
    }

    public class GoldenFlash : ModProjectile
    {
        public override string Texture => AssetDirectory.StarTexturePass;
        
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(20);
            Projectile.timeLeft = 20;
            
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = Projectile.timeLeft;
            Projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
        }

        public override void AI()
        {
            Projectile.rotation += 0.05f;
            float factor = Utils.Remap(Projectile.timeLeft, 0, Projectile.localAI[0], 1, 0);
            Projectile.scale = MathF.Sin(factor * MathHelper.Pi) * 1.2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            for(int i = 0; i < 2; i++)
            {
                Color color = Color.White.AdditiveColor() * 0.7f;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation + MathHelper.PiOver2 * i, texture.Size() / 2, new Vector2(Projectile.scale * 0.8f,Projectile.scale * 1.5f), 0, 0);
            }
            return false;
        }
    }
}
