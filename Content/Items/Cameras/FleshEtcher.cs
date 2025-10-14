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
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class FleshEtcher : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;
            Item.damage = 28;

            Item.useTime = Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.shoot = ModContent.ProjectileType<FleshEtcherProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 0, 24, 0));
            SetCameraStats(0.03f, 92, 2f);
            SetCaptureStats(1000, 60);

            base.SetOtherDefaults();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 8)
                .AddIngredient(ItemID.Amethyst, 3)
                .AddTile(TileID.Anvils)
                .Register();

        }
    }

    public class FleshEtcherProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(255, 20, 31);
        public override Color innerFrameColor => new Color(255, 124, 172) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(255, 183, 210);

        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;
 

            float startRot = Main.rand.NextFloat(-0.5f, 0.5f);

            int damage = (int)(Projectile.damage * 1f);
            Vector2 offset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(40);
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + offset, Vector2.Zero, ModContent.ProjectileType<FleshEtcherMouth>(), damage, 0, Projectile.owner);
            p.rotation = startRot;

            int dustcount = 20;

            for(int i = -1; i < 2; i+= 2)
            {
                for (int j = -dustcount / 2; j <= dustcount / 2; j++)
                {
                    Vector2 direction = new Vector2(0, i).RotatedBy(Math.PI / 2 / (dustcount + 1) * j).RotatedBy(startRot);
                    float speed = Main.rand.NextFloat(1, 3);
                    Vector2 vel = direction.RotatedByRandom(0.3f) * speed;
                    float scale = Main.rand.NextFloat(0.7f, 1.3f);
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Unit() * 35, 235, vel, Scale: scale);
                }
            }
        }
    }

    public class FleshEtcherMouth : ModProjectile
    {
        public override string Texture => AssetDirectory.VanillaProjPath(811);
        public ref float FreezeTime => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 164;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.penetrate = -1;
            Projectile.SetImmune(-1);
            Projectile.timeLeft = 60;
        }
        public override bool? CanDamage()
        {
            
            return Projectile.TimeleftFactor() < 0.25f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = 45 + Main.rand.Next(-4,5);
            //Projectile.rotation = Main.rand.NextFloat(-0.5f,0.5f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //FreezeTime = 5;
        }

        public override void AI()
        {
            if (FreezeTime > 0)
            {
                Projectile.timeLeft++;
                FreezeTime--;
            }

            if(Projectile.timeLeft < 2)
            {
                Projectile.timeLeft++;
                Projectile.localAI[0]++;
                if(Projectile.localAI[0] > 10)
                {
                    Projectile.Kill();
                }

                Projectile.Opacity = Utils.Remap(Projectile.localAI[0], 0, 10, 1, 0);
            }

            if(Projectile.timeLeft < 6)
            {
                if(Projectile.soundDelay == 0)
                {
                    Projectile.soundDelay = -1;
                    var sound = Main.rand.NextBool() ? SoundID.NPCDeath12 : SoundID.NPCDeath23;
                    SoundEngine.PlaySound((SoundStyle?)SoundID.NPCDeath12.WithVolumeScale(1f), Projectile.Center);
                }

                int dustcount = 10;
                for (int i = 0; i < dustcount; i++)
                {
                    float speed = Main.rand.NextFloat(2, 16);
                    Vector2 vel = Projectile.rotation.ToRotationVector2().RotatedByRandom(0.3f) * (Main.rand.NextBool() ? 1 : -1) * speed;
                    float scale = Main.rand.NextFloat(0.5f, 2.5f);
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(8), 235, vel, Scale:scale);
                    d.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D star = TextureAssets.Extra[98].Value;
            float timeFactor = Projectile.TimeleftFactor();

            float x = Utils.Remap(EaseManager.Evaluate(Ease.Linear, timeFactor,1f), 0, 1f, MathHelper.Pi, 0);

            int count = 6;
            for(int i = -1; i < 2; i += 2)
            {
                for(int j = -count / 2; j <= count / 2; j++)
                {
                    Vector2 anchor = new Vector2(j * 10, 0).RotatedBy(Projectile.rotation);
                    Vector2 direction = new Vector2(0, i).RotatedBy(Math.PI / 2 / (count + 1) * j * -i).RotatedBy(Projectile.rotation);
                    if (j != MathF.Abs(-count / 2)) direction = direction.RotatedBy(MathF.Sin(Projectile.rotation * 53898) * 0.2f);
                    float easevalue = EaseManager.Evaluate(Ease.OutSine, MathF.Sin(x), 1f);
                    //if (x > MathHelper.PiOver2) easevalue = EaseManager.Evaluate(Ease.InOutCubic, MathF.Sin(x), 1f);
                    Vector2 offset = anchor + direction * (80 + 30 * MathF.Sin(Utils.Remap(j, -3, 3, 0, MathHelper.Pi))) * easevalue;
                    if (j != MathF.Abs(-count / 2))
                    {
                        offset += (Projectile.rotation * 321).ToRotationVector2() * MathF.Sin(Projectile.rotation * 3 + j * 53225 + MathF.Sin(Projectile.rotation * i * 324)) * 20;
                    }
                    float scaleFactor = Utils.Remap(MathF.Abs(j), 0, 3, 0.5f, 1.3f) * Utils.Remap(MathF.Sin(x),0,1f,1.2f,0.6f);
                    Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + offset, null, Color.White * Projectile.Opacity * 0.6f, direction.ToRotation() + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale  * scaleFactor * 1.1f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition + offset, null, Color.White * Projectile.Opacity * 0.8f, direction.ToRotation() + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * scaleFactor * 0.6f, SpriteEffects.None, 0);

                }
            }

            return false;
        }
    }

}