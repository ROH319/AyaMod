using AyaMod.Common.Easer;
using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class CobaltCamera : BaseCamera
    {

        public override void SetOtherDefaults()
        {
            Item.width = 38;
            Item.height = 30;

            Item.damage = 50;

            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<CobaltCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.LightPurple6, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.03f, 152, 1.6f, 0.5f);
            SetCaptureStats(1000, 60);
        }

        public static float CobaltSpreadRange = 200f;
    }
    public class CobaltCameraProj : BaseCameraProj
    {

        public override Color outerFrameColor => new Color(0, 70, 156);
        public override Color innerFrameColor => new Color(132, 234, 255) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(116, 192, 255).AdditiveColor() * 0.5f;

        public override void OnHitNPCAlt(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.GetGlobalNPC<CobaltNPC>().CobaltMarked)
                target.GetGlobalNPC<CobaltNPC>().CobaltMarked = true;

            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(target.position, target.width, target.height, DustID.FrostHydra, Scale: 2f);
                d.noGravity = true;
            }
        }
        public override void OnSnap()
        {
            base.OnSnap();
        }
    }

    public class CobaltNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool CobaltMarked = false;

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (CobaltMarked)
            {
                modifiers.FinalDamage *= 1.1f;
                modifiers.ArmorPenetration += 5;
            }
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (!CobaltMarked) return;
            for (int i = 0; i < 5; i++)
            {
                Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.FrostHydra, Scale: 2f);
                d.noGravity = true;
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!CobaltMarked) return;

            for (int i = 0; i < 5; i++)
            {
                Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.FrostHydra, Scale: 2f);
                d.noGravity = true;
            }
        }
        public override void OnKill(NPC npc)
        {
            if (CobaltMarked)
            {
                //RingParticle.Spawn(npc.GetSource_Death(), npc.Center, new Color(116, 192, 255) * 0.5f, 20, 200, 
                //    0, 0.2f, 0.2f, 0.7f, 30, 120, Ease.OutSine, Ease.InCubic);


                //RingParticle.Spawn(npc.GetSource_Death(), npc.Center, new Color(116, 192, 255).AdditiveColor() * 0.3f, 40, 200, 0.8f, 0f,
                //            0.3f, 0.5f, 30, 180, Ease.OutCubic, Ease.OutCubic);

                foreach (var n in Main.ActiveNPCs)
                {
                    if (n.friendly || n.lifeMax <= 5 || n.whoAmI == npc.whoAmI) continue;
                    if (n.Distance(npc.Center) > CobaltCamera.CobaltSpreadRange) continue;

                    n.GetGlobalNPC<CobaltNPC>().CobaltMarked = true;
                }
            }
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (CobaltMarked)
            {
                var texture = TextureAssets.Projectile[538].Value;
                Vector2 pos = npc.Center + new Vector2(0f, (-npc.height / 2) - 20f) - Main.screenPosition;
                float scale = 1f + MathF.Cos(Main.GameUpdateCount * 0.05f + npc.whoAmI * 492 + npc.height * 1703) * 0.2f;
                Main.spriteBatch.Draw(texture, pos, null, Color.White, 0, texture.Size() / 2, scale, 0, 0);

                //var factor = 0.25f;
                //float Scale = 0.5f;
                //var texture = AssetDirectory.StarTexture;
                //Color cobaltColor = new(72, 73, 255);
                //float alpha = 1f;
                //Vector2 offset = new(0, -npc.height / 2 - 20);
                //for (int i = 0; i < 4; i++)
                //{
                //    Vector2 dir = (i * MathHelper.PiOver2).ToRotationVector2();
                //    Vector2 scale = Scale * new Vector2((1 - factor) / 2f, 1f);
                //    Main.spriteBatch.Draw(texture, npc.Center + offset + dir * factor * 20f - Main.screenPosition, null, cobaltColor * alpha,
                //        (i + 1) * MathHelper.PiOver2, new Vector2(36), scale, 0, 0);
                //}
            }
        }
    }
}
