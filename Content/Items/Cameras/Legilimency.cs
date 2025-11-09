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
            SetCaptureStats(1000, 60);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 8)
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
            if (target.HasBuff<ConfusedBuff>())
            {
                modifiers.FinalDamage *= 3f;
            }
        }
        public override void OnHitNPCAlt(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.HasBuff<ConfusedBuff>())
            {
                target.ClearBuff<ConfusedBuff>();
            }
        }

        public override void HoverNPC(NPC npc)
        {
            if (npc.friendly || npc.lifeMax == 5) return;

            var cnpc = npc.GetGlobalNPC<LegilimencyNPC>();
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

            var cnpc = npc.GetGlobalNPC<LegilimencyNPC>();
            cnpc.LegilimencyTimer--;
            if (cnpc.LegilimencyTimer < 0) cnpc.LegilimencyTimer = 0;
        }

    }

    public class LegilimencyNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool Confused;
        public int LegilimencyTimer;
        public override void ResetEffects(NPC npc)
        {
            Confused = false;
        }
        public override bool PreAI(NPC npc)
        {

            if (Confused)
            {
                bool legilimencyEffect = Main.player.Any((player => player.AliveCheck(npc.Center, 3000) && player.HeldItem.type == ModContent.ItemType<Legilimency>()));

                if (legilimencyEffect)
                {
                    //降低20%速度
                    npc.Aya().SpeedModifier *= 0.8f;
                }

                if (Main.rand.NextBool(3))
                {
                    foreach (var player in Main.ActivePlayers)
                    {
                        if (!player.AliveCheck(npc.Center, 2000) || !(player.HeldItem.type == ModContent.ItemType<Legilimency>())) continue;


                        Vector2 vel = npc.DirectionToSafe(player.Center) * 2f;
                        Dust d = Dust.NewDustPerfect(npc.Center, 112, vel, 128, Scale: 1f);
                        d.noGravity = true;
                    }
                }
            }
            return base.PreAI(npc);
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (Confused)
            {

                bool legilimencyEffect = Main.player.Any((player => player.AliveCheck(npc.Center, 3000)));

                if (legilimencyEffect)
                {
                    //15dps
                    npc.lifeRegen -= Legilimency.ConfusedDotDmg;
                }
                if (damage < 5)
                    damage = 5;
            }
        }
    }
}
