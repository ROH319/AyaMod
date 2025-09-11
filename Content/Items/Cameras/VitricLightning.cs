using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using Terraria.ID;
using AyaMod.Helpers;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.Collections;
using Terraria.Audio;
using AyaMod.Content.Buffs;
using AyaMod.Core;

namespace AyaMod.Content.Items.Cameras
{
    public class VitricLightning : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 27;

            Item.useTime = Item.useAnimation = 45;
            Item.crit = 3;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<VitricLightningProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 12f;
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 0, 60, 0));
            SetCameraStats(0.04f, 98, 2f);
            SetCaptureStats(100, 5);
        }
        public static int VitricDotDmg = 10;
    }

    public class VitricLightningProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(48, 253, 255);
        public override Color innerFrameColor => new Color(227, 221, 201) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(248, 255, 212).AdditiveColor() * 0.5f;


        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;
            var npc = Projectile.FindCloestNPC(260,true,true);
            if (npc != null)
            {
                Vector2 vel = Projectile.DirectionToSafe(npc.Center).RotatedBy((MathHelper.PiOver2 + MathHelper.PiOver4) * (Main.rand.NextBool() ? 1 : -1));
                var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel * 7, ModContent.ProjectileType<VitricElectric>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack / 4, Projectile.owner, 5, npc.whoAmI);
                p.localAI[2] = -1;
                SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);
            }


            int dustamount = 20;

            for(int  i = 0; i < dustamount; i++)
            {
                Vector2 pos = Projectile.Center + (MathHelper.TwoPi / dustamount * i).ToRotationVector2()/* * Main.rand.NextFloat(20, 80)*/;
                Vector2 vel = Projectile.Center.DirectionToSafe(pos) * Main.rand.NextFloat(2, 14);

                //Dust d = Dust.NewDustPerfect(pos, DustID.Electric, vel,128,Scale:0.6f);
                //d.noGravity = true;
            }
        }
    }

    public class VitricElectric : ModProjectile
    {
        public override string Texture => AssetDirectory.StarTexturePass;

        public int HitLeft
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public int TargetIndex
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public float[] OldOffset;
        public float[] OldOffsetRot;
        public float[] OldOffsetVel;

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 50);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 3;
            Projectile.penetrate = -1;
            Projectile.SetImmune(-1);
            Projectile.timeLeft = 200;
            Projectile.ArmorPenetration = 50;
        }

        public override void OnSpawn(IEntitySource source)
        {
            OldOffset = new float[Projectile.oldPos.Length];
            OldOffsetRot = new float[Projectile.oldPos.Length];
            OldOffsetVel = new float[Projectile.oldPos.Length];
            Projectile.localAI[2] = 45;
            base.OnSpawn(source);
        }


        public override void AI()
        {
            for (int i = Projectile.oldPos.Length - 1; i > 0; --i)
            {

                OldOffset[i] = OldOffset[i - 1];
                OldOffsetRot[i] = OldOffsetRot[i - 1];
                OldOffsetVel[i] = OldOffsetVel[i - 1];

                OldOffset[i] += OldOffsetVel[i];
                OldOffsetVel[i] *= 0.95f;
            }

            if (Projectile.ai[2] < 0)
            {
                Projectile.velocity *= 0;
                Projectile.extraUpdates = 0;
                return;
            }

            OldOffset[0] = 0;
            OldOffsetRot[0] = AyaUtils.RandAngle;
            if (Projectile.timeLeft % 8 == 0) OldOffsetVel[0] = Main.rand.NextFloat(4, 8) * 0.15f;

            if (Main.rand.NextBool(5)) {
                Vector2 ndir = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
                Vector2 pos = Projectile.Center + ndir * Main.rand.NextFloat(-1,1) * 5;
                Vector2 vel = Projectile.velocity * 0.6f;
                Dust d = Dust.NewDustPerfect(pos, DustID.Electric, vel,Scale:0.5f);
                d.noGravity = true;
            }

            if (TargetIndex >= 0 && --Projectile.localAI[2] <= 0)
            {
                NPC target = Main.npc[TargetIndex];
                if (target.active && target.CanBeChasedBy())
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionToSafe(target.Center).Length(6f), 0.2f);
                }
                else TargetIndex = -1;
            }
            else
            {
                var npc = Projectile.FindCloestNPC(150,false,true);
                if (npc != null) TargetIndex = npc.whoAmI;
            }
            base.AI();
        }

        public void EndMove()
        {
            Projectile.timeLeft = 30;
            Projectile.ai[2] = -1;
            Projectile.friendly = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (Projectile.owner == Main.myPlayer)
            {
                target.AddBuff(ModContent.BuffType<ElectrifiedBuff>(), 5 * 60);
                target.immune[Projectile.owner] = 10;
            }

            int dustamount = 7;

            for (int i = 0; i < dustamount; i++)
            {
                Vector2 pos = Projectile.Center + (MathHelper.TwoPi / dustamount * i).ToRotationVector2()/* * Main.rand.NextFloat(20, 80)*/;
                float rot = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                float velMult = Utils.Remap(MathF.Abs(rot), 0, MathHelper.PiOver4, 1.2f, 0.4f);
                Vector2 vel = Projectile.velocity.RotatedBy(MathHelper.Pi + rot) * velMult * Main.rand.NextFloat(0.7f,1.3f);

                Dust d = Dust.NewDustPerfect(pos, DustID.Electric, vel, 128, Scale: 0.6f);
                d.noGravity = true;
            }
            if (HitLeft > 0)
            {
                var npc = Projectile.FindCloestNPC(250,false,true);
                if (npc != null)
                {
                    Vector2 dir = Projectile.Center - target.Center;
                    Vector2 pos = target.Center - dir;
                    float rot = Utils.AngleLerp(Projectile.velocity.ToRotation(), Projectile.AngleToSafe(npc.Center), 0.4f);
                    Vector2 vel = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(rot) * 1f;
                    var p = Projectile.NewProjectileDirect(Projectile.GetSource_OnHit(target), pos, vel, Projectile.type, (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner, HitLeft - 1, npc.whoAmI);
                    
                }
            }

            EndMove();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = TextureAssets.Projectile[Type].Value;

            int length = Projectile.oldPos.Length - 1;

            for(int i = 0; i < length; i++)
            {

                float factor = i / (float)length;
                float factorRevert = Utils.Remap(factor, 0, 1f, 1f, 0);

                Color color = Color.LightSkyBlue.AdditiveColor() * 1f * factorRevert;
                for (int j = 0; j < 2; j++)
                {

                    for (int k = 0; k < 2; k++)
                    {
                        Vector2 start = Projectile.oldPos[i] + (OldOffsetRot[i] + MathHelper.Pi * j).ToRotationVector2() * OldOffset[i] * (0.2f + k * 0.6f);
                        Vector2 end = Projectile.oldPos[i + 1] + (OldOffsetRot[i + 1] + MathHelper.Pi * j).ToRotationVector2() * OldOffset[i + 1] * (0.2f + k * 0.6f);

                        float dist = start.Distance(end);
                        if (dist < 1f) continue;
                        float rot = (end - start).ToRotation();
                        float scaleFactor = 1.2f;
                        float widthScale = 0.4f * scaleFactor;
                        float heightScale = (dist / star.Height) * 0.25f * scaleFactor;
                        if (k > 0) color *= 0.5f;

                        Vector2 pos = Vector2.Lerp(start, end, 0.5f) - Main.screenPosition;
                        Main.EntitySpriteDraw(star, pos, null, color, rot, star.Size() / 2, new Vector2(widthScale,heightScale), 0, 0);
                    }
                }
            }

            return false;
        }
    }
}
