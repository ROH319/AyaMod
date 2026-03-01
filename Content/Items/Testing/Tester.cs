using AyaMod.Content.Items.Cameras;
using AyaMod.Content.Particles;
using AyaMod.Content.Projectiles.Auras;
using AyaMod.Core;
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

namespace AyaMod.Content.Items.Testing
{
    public class Tester : ModItem
    {
        public override string Texture => AssetDirectory.Testing + Name;
        public override void SetDefaults()
        {
            Item.width = Item.height = 38;
            Item.damage = 50;
            Item.knockBack = 5;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 8;
            Item.useTime = Item.useAnimation = 10;
            Item.DamageType = DamageClass.Melee;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            int index = 2;

            switch (index)
            {
                case 0:
                    var aura = AuraFriendly.Spawn(source, Main.MouseWorld, 2 * 60, BuffID.Ichor, 60, 400, new Color(20,20,20,128), new Color(168,74,69,255), player.whoAmI);
                    aura.SetRadiusFadein(0.4f, Common.Easer.Ease.OutCubic);
                    aura.SetRadiusFadeout(0.6f, Common.Easer.Ease.OutCubic);
                    break;
                case 1:
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(4f,5f);
                        Vector2 pos = Main.MouseWorld + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 30f);
                        float randscale = Main.rand.NextFloat(0.75f, 1.25f);
                        for (int i = 0; i < 2; i++)
                        {
                            var color = i == 0 ? new Color(49, 92, 219) : Color.White;
                            float scale = i == 0 ? 1f : 0.6f;
                            var p = SoulsParticle2.Spawn(source, pos, vel, color.AdditiveColor(), scale * .3f * randscale, 40);
                            p.alpha = 0.9f;
                            //var p = LightSpotParticle.Spawn(source, pos, vel, color, scale * .4f * randscale, 40);
                            //p.alpha = 0.5f;
                            //p.SetAlphaMult(0.86f);
                            //p.SetScaleMult(0.99f);
                            //p.UseAlpha = true;
                        }
                    }
                    break;
                case 2:

                    {
                        Projectile p = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, ProjectileType<TesterProj>(), damage, 0f, player.whoAmI);
                    }
                    break;
                default:break;
            }

            return false;
        }
    }
    public class TesterProj : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            Projectile.timeLeft++;
            base.AI();
        }
        public override void PostDraw(Color lightColor)
        {

            Texture2D texture = TextureAssets.MagicPixel.Value;
            float x = 4;
            Rectangle rect = new((int)(Projectile.Center.X - (x / 2) - Main.screenPosition.X), (int)(Projectile.Center.Y - x / 2 - Main.screenPosition.Y), (int)x, (int)x);
            Main.spriteBatch.Draw(texture, rect, null, Color.Red, 0f, texture.Size() / 2, 0, 0);

            float[] rots = new float[40];
            for (int i = 0; i < 40; i++) rots[i] = MathF.Sin((float)((Main.timeForVisualEffects * 0.75f + i * 4971) * 0.5f)) * MathF.Cos((float)((Main.GameUpdateCount + i * 225212) * 0.3f));
            for (int i = 0; i < 39; i++)
            {
                float dist = 4;
                Vector2 sp = Projectile.Center + new Vector2(-dist * (i + 1), 0);
                Vector2 ep = Projectile.Center + new Vector2(-dist * i, 0);
                //float sr = MathF.Sin((float)((Main.timeForVisualEffects) * 0.2f + (i + 1) * 2f));
                //float er = MathF.Sin((float)((Main.timeForVisualEffects) * 0.2f + i * 2f));
                float sr = rots[i + 1];
                float er = rots[i];
                float factor = Utils.Remap(MathF.Sin((i) * 5), -1f, 1f, 0.4f, 0.6f);
                //if (Main.GameUpdateCount % (i + 1) == Main.rand.Next(20)) factor += 0.2f;
                //factor = MathHelper.Clamp(factor, 0f, 1f);
                VitricSpark.DrawSegment(Color.SkyBlue, sp, ep, sr, er, factor, 1f, 0.75f);
            }
        }
    }
}
