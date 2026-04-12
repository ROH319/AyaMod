using AyaMod.Common.Easer;
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
using static System.Net.Mime.MediaTypeNames;
using static Terraria.ModLoader.BackupIO;

namespace AyaMod.Content.Items.Cameras
{
    public class BeeCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 35;

            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Rapier;

            Item.shootSpeed = 8;
            Item.knockBack = 8f;
            Item.shoot = ModContent.ProjectileType<BeeCameraProj>();
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 2, 0, 0));
            SetCameraStats(0.04f, 104, 2f);
            SetCaptureStats(1000, 60);
        }
    }

    public class BeeCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new(254, 246, 37);
        public override Color innerFrameColor => new Color(181, 213, 219) * 0.7f;
        public override Color focusCenterColor => new(227, 125, 22);
        public override Color flashColor => new Color(254, 194, 20).AdditiveColor() * 0.5f;

        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;


            //float radius = 96f;
            //float rotSpeed = 0.01f;
            //float speed = rotSpeed * radius;
            //int type = ProjectileType<BeeHiveCell>();
            //float rotDir = Main.rand.NextBool() ? 1 : -1;
            //switch (EffectCounter % 3)
            //{
            //    default:break;
            //    case 0:
            //        {
            //            Vector2 pos = AyaUtils.RandAngle.ToRotationVector2() * radius + Projectile.Center;
            //            Vector2 vel = Projectile.Center.DirectionToSafe(pos).RotatedBy(rotDir * MathHelper.PiOver2) * speed;
            //            int damage = Projectile.damage;
            //            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel,
            //                type, damage, 0f, Projectile.owner, rotSpeed, rotDir);
            //        }
            //        break;
            //    case 1:
            //        {
            //            float startAngle = AyaUtils.RandAngle;
            //            for (int i = 0; i < 3; i++)
            //            {
            //                Vector2 pos = (startAngle + MathHelper.TwoPi / 3 * i).ToRotationVector2() * radius + Projectile.Center;
            //                Vector2 vel = Projectile.Center.DirectionToSafe(pos).RotatedBy(rotDir * MathHelper.PiOver2) * speed;
            //                int damage = Projectile.damage;
            //                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel,
            //                    type, damage, 0f, Projectile.owner, rotSpeed, rotDir);
            //            }
            //        }
            //        break;
            //    case 2:
            //        {

            //            float startAngle = AyaUtils.RandAngle;
            //            for (int i = 0; i < 6; i++)
            //            {
            //                Vector2 pos = (startAngle + MathHelper.TwoPi / 6 * i).ToRotationVector2() * radius + Projectile.Center;
            //                Vector2 vel = Projectile.Center.DirectionToSafe(pos).RotatedBy(rotDir * MathHelper.PiOver2) * speed;
            //                int damage = Projectile.damage;
            //                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel,
            //                    type, damage, 0f, Projectile.owner, rotSpeed, rotDir);
            //            }
            //        }
            //        break;
            //}

            //EffectCounter++;

            int type = player.beeType();
            int count = 1;
            if (Main.rand.NextBool(6, 10)) count++;

            int damage = player.beeDamage(Projectile.damage / 3);
            for (int i = 0; i < count; i++)
            {
                var dir = AyaUtils.RandAngle.ToRotationVector2();
                Vector2 pos = Projectile.Center + dir * Main.rand.NextFloat(40, 80);
                var bee = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, dir * 2, type, damage, Projectile.knockBack / 4, player.whoAmI);
                bee.DamageType = ReporterDamage.Instance;
                bee.SetImmune(10);
            }
        }
    }

    public class BeeHiveCell : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + Name;
        public ref float rotSpeed => ref Projectile.ai[0];
        public ref float rotDir => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Timer = 30;
        }
        public override void AI()
        {
            if (Timer >= 0)
            {
                float factor = 1f - Timer / 30f;
                Projectile.Opacity = EaseManager.Evaluate(Ease.InCubic, factor, 1f);

                Projectile.velocity = Projectile.velocity.RotatedBy(rotSpeed * rotDir);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                if (Timer == 0 && Projectile.MyClient())
                {
                    Vector2 dir = Projectile.Center.DirectionToSafe(Main.MouseWorld);
                    float speed = 12;
                    int damage = Projectile.damage;
                    int type = ProjectileType<BeeNeedle>();
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, dir * speed, type, damage, Projectile.knockBack, Projectile.owner);

                    Projectile.velocity = -dir * speed * 0.2f;
                }
            }
            else
            {
                float factor = 1f + Timer / 30f;
                Projectile.Opacity = factor;
                Projectile.scale = Utils.Remap(factor, 0, 1, 1.5f, 1f);
            }

            Timer--;
        }
        public override void OnKill(int timeLeft)
        {
            //if (!Projectile.MyClient()) return;


        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            float scale = Projectile.scale * Projectile.width / texture.Width * 1.5f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, 
                Projectile.rotation, texture.Size() / 2, scale, SpriteEffects.None, 0);
            return false;
        }
    }
    public class BeeNeedle : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float Timer => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.extraUpdates = 2;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(15);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            State++;
        }
        public override void AI()
        {
            if (State > 0)
            {
                Projectile.velocity *= 0.95f;
                Timer++;
                if (Timer > 45)
                    Projectile.Kill();
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnKill(int timeLeft)
        {

            int dustAmount = 16;
            for (int i = 0; i < dustAmount; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Bee, Scale: 1.5f);
                d.noGravity = true;
            }
            if (!Projectile.MyClient()) return;

            int type = Main.player[Projectile.owner].beeType();

            int damage = Main.player[Projectile.owner].beeDamage(Projectile.damage / 3);
            var dir = Projectile.rotation.ToRotationVector2();
            var bee = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, dir * 2, type, damage, Projectile.knockBack / 4, Projectile.owner);
            bee.DamageType = ReporterDamage.Instance;
            bee.SetImmune(10);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = AssetDirectory.StarTexture;
            float length = 96f;
            float interval = 6f;
            float count = length / interval;
            Vector2 scale = new Vector2(0.4f,0.8f) * Projectile.scale;
            for (int i = 0; i < count; i++)
            {
                float factor = 1f - i / count;
                Color color = new Color(255, 218, 65).AdditiveColor() * factor;
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.Zero) * i * interval, 
                    null, color * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, star.Size() / 2, scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
