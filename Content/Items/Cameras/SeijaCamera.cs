using AyaMod.Common.Easer;
using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Core.Systems;
using AyaMod.Core.Systems.Trails;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using static Terraria.Utilities.NPCUtils;

namespace AyaMod.Content.Items.Cameras
{
    public class SeijaCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 20;

            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<SeijaCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 0, 78, 0));
            SetCameraStats(0.05f, 132, 1.4f, 0.5f);
            SetCaptureStats(1000, 60);
        }
        public override bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            float totalY = 0;
            foreach (var line in lines)
            {
                totalY += FontAssets.MouseText.Value.MeasureString(line.Text).Y;
            }
            if (!Item.favorited)
            {
                var yAxis = (totalY * 0.5f + Main.mouseY + 26) * Main.UIScale;

                // 创建一个缩放矩阵，该矩阵沿 Y 轴缩放 -1，从而实现镜像翻转。
                Matrix scaleMatrix = Matrix.CreateScale(1, -1, 1);

                // 创建一个平移矩阵，该矩阵将坐标系向上移动 yAxis。
                Matrix translateToOrigin = Matrix.CreateTranslation(0, -yAxis, 0);

                // 创建一个平移矩阵，该矩阵将坐标系向下移动 yAxis。
                Matrix translateBack = Matrix.CreateTranslation(0, yAxis, 0);

                // 将这些矩阵组合起来，以先平移到原点，然后缩放（镜像翻转），然后平移回来的顺序进行。
                var mirrorY = translateToOrigin * scaleMatrix * translateBack;

                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                    Main.spriteBatch.GraphicsDevice.DepthStencilState, RasterizerState.CullNone, null, Main.UIScaleMatrix * mirrorY);
            }
            return base.PreDrawTooltip(lines, ref x, ref y);
        }

        public override void PostDrawTooltip(ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            if (!Item.favorited)
            {
                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap,
                    Main.spriteBatch.GraphicsDevice.DepthStencilState, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            }
            base.PostDrawTooltip(lines);
        }
    }

    public class SeijaCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(200, 200, 200);
        public override Color innerFrameColor => new Color(150, 150, 150) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(220, 220, 220).AdditiveColor() * 0.5f;
        public override bool CanClear() => false;
        public override void OnSnapProjectile(Projectile projectile)
        {
            if (projectile.type != ProjectileType<SeijaArrow>()) return;
            projectile.frame = projectile.frame == 0 ? 1 : 0;
            projectile.velocity = -projectile.velocity;
            projectile.timeLeft += 60;
        }

        public override void PostAI()
        {
            if (!player.ItemTimeIsZero && Main.GameUpdateCount % 7 == 0)
            {
                float maxX = 100;
                float speed = Main.rand.NextFloat(5, 13);
                Vector2 pos = Projectile.Center + new Vector2(Main.rand.NextFloat(-maxX, maxX), Main.rand.NextFloat(300, 400));
                Vector2 vel = -Vector2.UnitY.RotateRandom(0.25f) * speed;

                int damage = (int)(Projectile.damage * 0.5f);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel, ProjectileType<SeijaArrow>(), damage, Projectile.knockBack, Projectile.owner);
            }
        }
        public override void AltFunctionUse()
        {
            foreach(var item in Main.ActiveItems)
            {
                if ((bool)!Colliding(Projectile.getRect(), item.getRect())) continue;
                if (item.TryGetGlobalItem(out SeijaGlobalItem sItem))
                {
                    if (sItem.ReverseTimer > 0) continue;
                    if (!RecipeSystem.SeijaRecipes.KeyContains(item.type) && !RecipeSystem.SeijaRecipes.ValueContains(item.type))
                        continue;
                    sItem.ReverseTimer = 45;
                    ReverseParticle.SpawnReverse(Projectile.GetSource_FromAI(), item.Center, 1f, 45);
                    ReverseEffect(item.Center);
                }
            }
            player.itemTime = player.itemAnimation = 0;

            CameraFlashReverse.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, Color.White, Projectile.rotation, floatingsize / 16f, floatingsize * 1.4f / 16f, 20);

        }
        public static void ReverseEffect(Vector2 center)
        {
            Helper.PlayPitched("Reverse", volume: 0.7f, pitch: 0.3f, position: center);
            var types = new int[] { DustID.RedTorch,DustID.BlueTorch };
            for (int i = 0; i < 30; i++)
            {
                int num = Dust.NewDust(center, 1, 1, DustID.ShimmerSpark);
                Main.dust[num].scale *= 2f;
                if (Main.rand.NextBool(3)) Main.dust[num].scale *= 1.5f;
                if (Main.rand.NextBool())
                    Main.dust[num].color = Color.Red;
                else
                    Main.dust[num].color = Color.Blue;

                //var d = Dust.NewDustDirect(center, 1, 1, Main.rand.NextFromList(types));
                //d.scale = 1.5f;
                //d.noGravity = true;
            }
        }
        public override void SpawnFlash()
        {
            CameraFlashReverse.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, Color.White, Projectile.rotation, floatingsize / 16f, floatingsize * 1.4f / 16f, 20);

        }
        public override bool PreDraw(ref Color lightColor)
        {

            //BlendState inverseColor = new();
            //inverseColor.ColorSourceBlend = Blend.InverseDestinationColor;
            //inverseColor.ColorDestinationBlend = Blend.InverseSourceColor;
            //inverseColor.AlphaSourceBlend = Blend.Zero;
            //inverseColor.AlphaDestinationBlend = Blend.One;

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, inverseColor, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            base.PreDraw(ref lightColor);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }
    }

    public class SeijaGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public int ReverseTimer;
        public float ReverseProgress;
        public static void CheckConvert()
        {
            //foreach(var item in Main.ActiveItems)
            //{
            //    if (item.TryGetGlobalItem(out SeijaGlobalItem sItem))
            //    {
            //        if (sItem.ReverseTimer == 22)
            //        {
            //            int targetType = -1;

            //            if (RecipeSystem.SeijaRecipes.TryGetValueByKey(item.type, out int value))
            //                targetType = value;
            //            else if (RecipeSystem.SeijaRecipes.TryGetKeyByValue(item.type, out int key))
            //                targetType = key;

            //            if (targetType == -1) return;

            //            int stack = item.stack;
            //            var progress = sItem.ReverseProgress;
            //            var timer = sItem.ReverseTimer;
            //            var prev = sItem.ReverseTimer;
            //            item.SetDefaults(targetType);
            //            item.stack = stack;
            //            item.GetGlobalItem<SeijaGlobalItem>().ReverseProgress = progress;
            //            ReverseTimer = timer;
            //            var post = ReverseTimer;
            //        }
            //    }
            //}
        }
        public override void PostUpdate(Item item)
        {
            ReverseProgress = Utils.Remap(ReverseTimer, 34, 11, 0f, 1f);
            if (ReverseTimer == 23) 
            {

                int targetType = -1;

                {
                    var values = RecipeSystem.SeijaRecipes.GetValuesByKey(item.type);
                    if (values.Count > 0)
                    {
                        targetType = Main.rand.NextFromList([.. values]);
                    }
                }
                {
                    var keys = RecipeSystem.SeijaRecipes.GetKeysByValue(item.type);
                    if (keys.Count > 0)
                    {
                        targetType = Main.rand.NextFromList([.. keys]);
                    }
                }

                if (targetType == -1) return;

                int stack = item.stack;
                var progress = ReverseProgress;
                var timer = ReverseTimer;
                var prev = ReverseTimer;
                item.SetDefaults(targetType);
                item.stack = stack;
                item.GetGlobalItem<SeijaGlobalItem>().ReverseProgress = progress;
                item.GetGlobalItem<SeijaGlobalItem>().ReverseTimer = timer - 1;
                //var post = item.GetGlobalItem<SeijaGlobalItem>().ReverseTimer;
                //Main.NewText($"{prev} {post} {Main.GameUpdateCount}");
            }

            //ReverseProgress = Utils.Remap(ReverseTimer, 34, 11, 0f, 1f);
            if (ReverseTimer > 0)
                ReverseTimer--;
        }
        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var extraRot = Utils.Remap(ReverseProgress, 0f, 1f, 0, MathHelper.TwoPi);
            rotation += extraRot;
            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }
    }

    public class SeijaArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;
        public static MultedTrail strip = new();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            Projectile.SetTrail(4, 20);
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 40;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.SetImmune(20);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(2);
            Projectile.Opacity = 0f;
        }
        public override void AI()
        {

            //Projectile.velocity += Projectile.velocity.Length(0.05f);

            Projectile.Opacity += 0.05f;
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity, 0f, 1f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            int dusttype = Projectile.frame == 0 ? 281 : 45;

            //float scale = Projectile.scale * 0.5f;
            //int alpha = (int)Utils.Remap(Projectile.Opacity, 0, 1f, 255, 0);
            //Dust d = Dust.NewDustPerfect(Projectile.Center, dusttype, Projectile.velocity * 0f, alpha, Scale: scale);
            //d.noGravity = true;

            //Dust d2 = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 0.5f, dusttype, Projectile.velocity * 0f, alpha);
            //d2.noGravity = true;
        }
        public Color ColorFunction(float progress)
        {
            Color drawColor = Projectile.frame == 0 ? new(247, 148, 150) : new(139, 136, 240);
            return Color.Lerp(drawColor * .5f, Color.Black, progress).AdditiveColor() * Projectile.Opacity;
        }
        public float WidthFunction(float progress)
        {
            return 1f;
        }
        public float AlphaFunction(float progress)
        {
            float fadeinFactor = Utils.Remap(progress, 0f, 0.05f, 0f, 1f);
            return EaseManager.Evaluate(Ease.InOutSine, 1f - progress, 1f) * Projectile.Opacity * fadeinFactor;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            #region 拖尾
            strip.PrepareStrip(Projectile.oldPos, 2, ColorFunction, WidthFunction, 
                Projectile.Size / 2 - Main.screenPosition, AlphaFunction);
            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
            strip.DrawTrail();

            #endregion

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameWidth = texture.Width / Main.projFrames[Type];
            int x = frameWidth * Projectile.frame;

            Rectangle rect = new(x, 0, 14, 40);
            Color color = Color.White * Projectile.Opacity;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect,
                color, Projectile.rotation, Projectile.Size / 2, Projectile.scale, 0);

            return false;
        }
    }
}
