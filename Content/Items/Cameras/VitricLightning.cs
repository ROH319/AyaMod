using AyaMod.Content.Buffs;
using AyaMod.Core;
using AyaMod.Core.Loaders;
using AyaMod.Core.Prefabs;
using AyaMod.Core.Systems;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

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
            SetCaptureStats(1000, 60);
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

            var poses = AyaUtils.GetCameraRect(Projectile.Center, Projectile.rotation, Size, Size * 1.4f);

            float range = 200f;
            List<int> npclist = [];
            foreach (var pos in poses)
            {
                float minDist = range;
                NPC target = null;
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (!npc.CanBeChasedBy(Projectile) || npclist.Contains(npc.whoAmI)) continue;
                    float dist = npc.Hitbox.Distance(pos);
                    if (dist > minDist) continue;
                    target = npc;
                    minDist = dist;
                }
                if (target == null) continue;
                SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);
                Projectile spark = SpawnSpark(pos, target.whoAmI);
                npclist.Add(target.whoAmI);
            }
            #region Abandoned Design
            //var npc = Projectile.FindCloestNPC(260,true,true);
            //if (npc != null)
            //{
            //    Vector2 vel = Projectile.DirectionToSafe(npc.Center).RotatedBy((MathHelper.PiOver2 + MathHelper.PiOver4) * (Main.rand.NextBool() ? 1 : -1));
            //    var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel * 7, ModContent.ProjectileType<VitricElectric>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack / 4, Projectile.owner, 5, npc.whoAmI);
            //    p.localAI[2] = -1;
            //    SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);
            //}


            //int dustamount = 20;

            //for(int  i = 0; i < dustamount; i++)
            //{
            //    Vector2 pos = Projectile.Center + (MathHelper.TwoPi / dustamount * i).ToRotationVector2()/* * Main.rand.NextFloat(20, 80)*/;
            //    Vector2 vel = Projectile.Center.DirectionToSafe(pos) * Main.rand.NextFloat(2, 14);

            //    //Dust d = Dust.NewDustPerfect(pos, DustID.Electric, vel,128,Scale:0.6f);
            //    //d.noGravity = true;
            //}
            #endregion
        }
        public Projectile SpawnSpark(Vector2 pos, int target = -1)
        {
            Vector2 dir = pos.DirectionToSafe(Main.npc[target].Center).RotateRandom(MathHelper.PiOver2);/*AyaUtils.RandAngle.ToRotationVector2();*/
            int damage = (int)(Projectile.damage * 0.3f);
            Projectile spark = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, dir * 7f,
                ProjectileType<VitricSpark>(), damage, Projectile.knockBack / 4f, Projectile.owner, target, 0);
            spark.rotation = dir.ToRotation();
            return spark;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float alphaModifier = 1f;
            if (!CanHit) alphaModifier *= 0.2f;
            DrawElectricFrame(Projectile.Center, Projectile.rotation, floatingsize, FocusFactor, CameraStats.MaxFocusScale,
                outerFrameColor * alphaModifier, innerFrameColor * alphaModifier, focusCenterColor * alphaModifier);
            return false;

            //return base.PreDraw(ref lightColor);
        }
        public static void DrawElectricFrame(Vector2 center, float rot, float size,
            float focusdScale, float maxFocusScale, Color outerFrameColor, Color innerFrameColor, Color focusCenterColor)
        {
            float focusFactor = (focusdScale - 1f) / (maxFocusScale - 1f);

            float sizex = size;
            float sizey = size * 1.4f;
            var pos = AyaUtils.GetCameraRect(center, rot, sizex, sizey);

            Vector2 dir = pos[0].DirectionToSafe(pos[2]);
            Vector2 ndir = pos[0].DirectionToSafe(pos[1]);

            float borderWidth = 0.4f;

            float focusdSizeX = sizex * focusdScale;
            float focusdSizeY = sizey * focusdScale;
            var pos2 = AyaUtils.GetCameraRect(center, rot, focusdSizeX, focusdSizeY);

            Color borderColor = outerFrameColor * focusFactor;
            float frameBoundriesFactor = 0.3f;

            //内框
            DrawCameraFrameElectric(pos, innerFrameColor * (1 - focusFactor), borderWidth, frameBoundriesFactor);
            //外框
            DrawCameraFrameElectric(pos2, borderColor, borderWidth, frameBoundriesFactor - 0.1f);

            //焦点
            RenderHelper.DrawElectricLine(4, center - dir * sizex / 8f, center + dir * sizex / 8f, focusCenterColor, focusCenterColor, borderWidth, 0.5f);
            RenderHelper.DrawElectricLine(4, center - ndir * sizey / 8f, center + ndir * sizey / 8f, focusCenterColor, focusCenterColor, borderWidth, 0.5f);
        }
        public static void DrawCameraFrameElectric(Vector2[] boundries, Color borderColor, float borderWidth, float lengthPercent)
        {
            Vector2 dirx = boundries[0].DirectionToSafe(boundries[2]);
            Vector2 diry = dirx.RotatedBy(MathHelper.PiOver2);

            float sizex = boundries[0].Distance(boundries[2]);
            float sizey = boundries[0].Distance(boundries[1]);

            float lengthX = sizex * lengthPercent;
            float lengthY = sizey * lengthPercent;
            int interval = 6;
            float scale = 0.5f;

            RenderHelper.DrawElectricLine(interval, boundries[0], boundries[0] + dirx * lengthX, borderColor, borderColor, borderWidth, scale);
            RenderHelper.DrawElectricLine(interval, boundries[2], boundries[2] - dirx * lengthX, borderColor, borderColor, borderWidth, scale);
            RenderHelper.DrawElectricLine(interval, boundries[1], boundries[1] + dirx * lengthX, borderColor, borderColor, borderWidth, scale);
            RenderHelper.DrawElectricLine(interval, boundries[3], boundries[3] - dirx * lengthX, borderColor, borderColor, borderWidth, scale);
            RenderHelper.DrawElectricLine(interval, boundries[0], boundries[0] + diry * lengthY, borderColor, borderColor, borderWidth, scale);
            RenderHelper.DrawElectricLine(interval, boundries[1], boundries[1] - diry * lengthY, borderColor, borderColor, borderWidth, scale);
            RenderHelper.DrawElectricLine(interval, boundries[2], boundries[2] + diry * lengthY, borderColor, borderColor, borderWidth, scale);
            RenderHelper.DrawElectricLine(interval, boundries[3], boundries[3] - diry * lengthY, borderColor, borderColor, borderWidth, scale);
        }
    }
    public class VitricSpark : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float Target => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        /// <summary>
        /// 小于0时为停止移动
        /// </summary>
        public ref float StopMove => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 50);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.timeLeft = 100;
            Projectile.extraUpdates =6;
            Projectile.penetrate = -1;
            Projectile.SetImmune(-1);
            Projectile.ArmorPenetration = 50;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Timer < 12 || (Target != -1 && Target != target.whoAmI)) return false;
            return null;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                target.AddBuff(BuffType<ElectrifiedBuff>(), 5 * 60);
                //target.immune[Projectile.owner] = 10;
            }

            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
                dust.noGravity = true;
                float dot = Vector2.Dot(dust.velocity, Projectile.velocity);
                dust.velocity *= -(dot / 4f);
                dust.scale *= 0.6f;
            }
            EndMove();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldRot[i] = Main.rand.NextFloat(-1, 1);
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                target.AddBuff(BuffType<ElectrifiedBuff>(), 5 * 60);
                //target.immune[Projectile.owner] = 10;
            }
            EndMove();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldRot[i] = Main.rand.NextFloat(-1, 1);
            }
        }
        public override void AI()
        {
            if(StopMove < 0)
            {
                StopMove--;
                Projectile.velocity *= 0;
                Projectile.extraUpdates = 0;
                Projectile.friendly = false;
                Projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                return;
            }
            Timer++;
            if(Target != -1)
            {
                NPC target = Main.npc[(int)Target];
                if (target.Alive() && !target.friendly && target.lifeMax > 5 && !target.dontTakeDamage)
                {
                    if (Projectile.timeLeft < 5) Projectile.timeLeft++;
                    float chaseFactor = Utils.Remap(Timer, 0, 20, 0, 0.12f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionToSafe(target.Center) * 7f, chaseFactor);
                }
                else Target = -1;
            }
            else
            {
                Projectile.velocity = Projectile.velocity.RotateRandom(0.6f) * 7f;
                if (Projectile.timeLeft < 60) EndMove();
            }
            if(Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.UltraBrightTorch, AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(2f), Scale: 0.8f);
                d.noGravity = true;
            }
            if (Main.rand.NextBool(2))
            {

                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0, 0, 128, Scale: 0.6f);
                d.noGravity = true;
                //Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric);
                //d.noGravity = true;
            }
            Projectile.rotation = Main.rand.NextFloat(-1, 1);
        }

        public void EndMove()
        {
            StopMove = -1;
            Projectile.timeLeft = 45;
            Projectile.velocity *= 0;
            Projectile.friendly = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = AssetDirectory.StarTexture;
            for (int i = 0;i < Projectile.oldPos.Length - 1; i++)
            {
                if (i < -Projectile.ai[2] - 1) continue;
                if (Projectile.oldPos[i + 1].Equals(Vector2.Zero)) continue;

                float factor = (float)i / Projectile.oldPos.Length;
                DrawSegment(Color.SkyBlue, Projectile.oldPos[i + 1], Projectile.oldPos[i], Projectile.oldRot[i + 1], Projectile.oldRot[i], factor, Projectile.Opacity, Projectile.scale);

                //float offset = 40 * width;
                //var startRot = Projectile.oldRot[i];
                //var endRot = Projectile.oldRot[i + 1];

                //Vector2 randVec = (startRot).ToRotationVector2() * MathF.Sin(Projectile.oldPos[i].Y * -49711) * 20;
                //Vector2 randVec2 = (endRot).ToRotationVector2() * MathF.Sin(Projectile.oldPos[i+1].Y * -49711) * 20;
                //var startPos = Projectile.oldPos[i] + MathF.Sin(Projectile.oldPos[i].X * 1293 + Projectile.oldPos[i].Y * -49711) * randVec;
                //var endPos = Projectile.oldPos[i + 1] + MathF.Sin(Projectile.oldPos[i + 1].X * 1293 + Projectile.oldPos[i + 1].Y * -49711) * randVec; ;
                //float rot = endPos.AngleTo(startPos);
                //if (rot == 0f) continue;

                //Vector2 drawpos = Vector2.Lerp(startPos,endPos,0.5f) - Main.screenPosition;
                //Color drawColor = Color.SkyBlue * Projectile.Opacity;
                //drawColor.A /= 2;
                //rot = startPos.AngleTo(endPos) + MathHelper.PiOver2;
                //float dist = startPos.Distance(endPos);
                //Vector2 scale = new Vector2((1f - width) * Projectile.scale * 0.3f,dist / 18f * 0.5f);
                //Main.spriteBatch.Draw(star, drawpos, null, drawColor, rot, star.Size() / 2, scale, 0, 0);
            }
            return false;
        }

        public static void DrawSegment(Color baseColor, Vector2 sp, Vector2 ep, float sr, float er, float radius, float alpha, float scale)
        {

            float rot = (ep - sp).ToRotation();
            float lengthMax = 16f * radius;
            sp += new Vector2(0, sr).RotatedBy(rot) * lengthMax;
            ep += new Vector2(0, er).RotatedBy(rot) * lengthMax;

            Texture2D star = AssetDirectory.StarTexture;
            Vector2 drawPos = ep - Main.screenPosition;
            Color color = baseColor * alpha; color.A /= 2;
            rot = (ep - sp).ToRotation() + MathHelper.PiOver2;
            scale = (1 - radius) * scale * 0.4f;
            float dist = Vector2.Distance(sp, ep);

            Main.spriteBatch.Draw(star, drawPos, new Rectangle(0, 0, 72, 36), color, rot, new Vector2(36, 36), new Vector2(1f, 0.55f) * scale, 0, 0);
            Main.spriteBatch.Draw(star, drawPos, new Rectangle(0, 36, 72, 36), color, rot, new Vector2(36, 0), new Vector2(scale, dist / 18f), 0, 0);
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
        public int LastTargetIndex
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
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
        public override bool? CanDamage() => Projectile.ai[2] >= 0;
        public override bool? CanHitNPC(NPC target) => target.whoAmI == LastTargetIndex ? false : base.CanHitNPC(target);
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
                var npc = Projectile.FindCloestNPCIgnoreIndex(300,false,true, LastTargetIndex);
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
            //Main.NewText($"{Projectile.ai[2]}");
            if (Projectile.owner == Main.myPlayer)
            {
                target.AddBuff(ModContent.BuffType<ElectrifiedBuff>(), 5 * 60);
                //target.immune[Projectile.owner] = 10;
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
                var npc = Projectile.FindCloestNPCIgnoreIndex(350,false,true, target.whoAmI);
                //if (npc == target) Main.NewText("???");
                if (npc != null)
                {
                    Vector2 dir = Projectile.Center - target.Center;
                    Vector2 pos = target.Center - dir;
                    float rot = Utils.AngleLerp(Projectile.velocity.ToRotation(), Projectile.AngleToSafe(npc.Center), 0.4f);
                    Vector2 vel = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(rot) * 1f;
                    var p = Projectile.NewProjectileDirect(Projectile.GetSource_OnHit(target), pos, vel, Projectile.type, (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner, HitLeft - 1, npc.whoAmI,target.whoAmI);
                    
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
