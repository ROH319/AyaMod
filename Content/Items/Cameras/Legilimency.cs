using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AyaMod.Content.Buffs;

namespace AyaMod.Content.Items.Cameras
{
    public class Legilimency : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 40;

            Item.useTime = Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<LegilimencyProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(0, 2,0, 0));
            SetCameraStats(0.04f, 104, 2f);
            SetCaptureStats(100, 5);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 12)
                .AddIngredient(ItemID.TissueSample, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public static int ConfusedDotDmg = 30;
    }

    public class LegilimencyProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(185, 47, 122);
        public override Color innerFrameColor => new Color(253, 216, 190) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(251, 167, 203).AdditiveColor() * 0.5f;

        public override void ModifyHitNPCAlt(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<ConfusedBuff>()))
            {
                modifiers.FinalDamage *= 3f;
                target.ClearBuff<ConfusedBuff>();
            }
        }

        public override void HoverNPC(NPC npc)
        {
            if (npc.friendly || npc.lifeMax == 5) return;

            var cnpc = npc.Camera();
            if (!player.ItemTimeIsZero)
            {
                cnpc.LegilimencyTimer--;
                if (cnpc.LegilimencyTimer < 0) cnpc.LegilimencyTimer = 0;
                return;
            }
            cnpc.LegilimencyTimer++;
            if (cnpc.LegilimencyTimer > 20 && !npc.HasBuff<ConfusedBuff>() && !npc.buffImmune[ModContent.BuffType<ConfusedBuff>()])
            {
                npc.AddBuff(ModContent.BuffType<ConfusedBuff>(), 4 * 60);
                cnpc.LegilimencyTimer = 0;

                int dustamount = 24;
                for(int i = 0; i < dustamount; i++)
                {
                    Vector2 vel = (AyaUtils.RandAngle).ToRotationVector2() * Main.rand.NextFloat(2, 6);
                    Dust d = Dust.NewDustPerfect(npc.Center, 205, vel);
                    d.noGravity = true;
                }
            }
        }

        public override void NotHoverNPC(NPC npc)
        {
            if (npc.friendly || npc.lifeMax == 5) return;

            var cnpc = npc.Camera();
            cnpc.LegilimencyTimer--;
            if (cnpc.LegilimencyTimer < 0) cnpc.LegilimencyTimer = 0;
        }

    }
}
