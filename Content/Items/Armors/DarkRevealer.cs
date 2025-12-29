using AyaMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.Localization;
using Terraria;
using AyaMod.Core.Attributes;
using AyaMod.Helpers;
using AyaMod.Core.Globals;
using AyaMod.Core.Prefabs;
using AyaMod.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AyaMod.Core.Loaders;
using Terraria.ID;

namespace AyaMod.Content.Items.Armors
{
    [PlayerEffect(OverrideEffectName = DarkRevealerSet)]
    [AutoloadEquip(EquipType.Head)]
    public class DarkRevealerMask : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBonus);

        public const string DarkRevealerSet = "DarkRevealerSet";
        public static LocalizedText DarkBonus { get; set; }

        public override void Load()
        {
            GlobalCamera.HoverNPCHook += DarkHover;
            GlobalCamera.PostClearHook += DarkClear;
        }
        public override void SetStaticDefaults()
        {
            DarkBonus = this.GetLocalization("DarkBonus");
        }

        public override void SetDefaults()
        {
            Item.width = 19;
            Item.height = 12;
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(gold: 2));
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ReporterDamage>() += CritBonus;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 8)
                .AddIngredient(ItemID.TissueSample, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<DarkRevealerCloak>() && legs.type == ItemType<DarkRevealerLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = DarkBonus.WithFormatArgs(DarkRevealedBonus, NoHitSecondRequired, StealthDuration).Value;
            DarkSetBonus(player);
        }

        public static void DarkSetBonus(Player player)
        {
            player.AddEffect(DarkRevealerSet);
            if (player.Aya().noHitTimer > NoHitSecondRequired * 60 && !player.HasBuff<StealthNoHitBuff>())
            {
                player.AddBuff(BuffType<StealthNoHitBuff>(), 2);

                for (int i = 0; i < 45; i++)
                {
                    var length = Main.rand.NextFloat(20, 100);
                    Vector2 pos = player.Center + Main.rand.NextVector2Unit() * length;
                    Vector2 vel = pos.DirectionToSafe(player.Center) * Utils.Remap(length, 20, 100, 1f, 6);
                    Dust d = Dust.NewDustPerfect(pos, DustID.Shadowflame, vel);
                    d.scale *= 2f;
                    d.noGravity = true;
                }
            }

        }

        public static void DarkHover(BaseCameraProj camera, NPC npc)
        {
            if(camera.player.HasEffect(DarkRevealerSet))
            {
                var darkReveal = npc.GetGlobalNPC<DarkRevealNPC>();
                darkReveal.darkRevealTimer++;
                if(darkReveal.darkRevealTimer > 0)
                {
                    darkReveal.darkRevealTimer = 0;
                    npc.AddBuff(BuffType<DarkRevealedBuff>(), 2);
                }
            }
        }

        public static void DarkClear(BaseCameraProj camera, int captureCount)
        {
            if (camera.player.HasEffect(DarkRevealerSet))
            {
                camera.player.AddBuff(BuffType<StealthBuff>(), StealthDuration * 60);
            }
        }

        public static int CritBonus = 9;
        public static int DarkRevealedBonus = 15;
        public static int NoHitSecondRequired = 10;
        public static int StealthDuration = 10;
    }

    [AutoloadEquip(EquipType.Body)]
    public class DarkRevealerCloak : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RegenBonus, DamageBonus);

        public override void SetDefaults()
        {
            Item.width = 19;
            Item.height = 12;
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(gold:2));
            Item.defense = 6;
        }
        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += RegenBonus;
            player.GetDamage<ReporterDamage>() += DamageBonus * 0.01f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 12)
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 12)
                .AddIngredient(ItemID.TissueSample, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public static int RegenBonus = 1;
        public static int DamageBonus = 8;
    }

    [AutoloadEquip(EquipType.Legs)]
    public class DarkRevealerLeggings : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, MoveSpeedBonus);
        public override void SetDefaults()
        {
            Item.width = 19;
            Item.height = 12;
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(gold: 2));
            Item.defense = 5;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus * 0.01f;
            player.moveSpeed += MoveSpeedBonus * 0.01f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddIngredient(ItemID.ShadowScale, 8)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.TissueSample, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public static int DamageBonus = 8;
        public static int MoveSpeedBonus = 8;
    }

    public class DarkRevealNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool darkRevealed;
        public int darkRevealTimer;
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (darkRevealed)
            {
                modifiers.FinalDamage *= 1f + DarkRevealerMask.DarkRevealedBonus / 100f;
            }
            base.ModifyIncomingHit(npc, ref modifiers);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (darkRevealed)
            {
                //var shader = ShaderLoader.GetShader("JustColorEffect");
                ////shader.Parameters["uDrawColor"].SetValue(drawColor.ToVector4());
                //spriteBatch.End();
                //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                //shader.CurrentTechnique.Passes[0].Apply();


                //spriteBatch.Draw()
                var shader = ShaderLoader.GetShader("StrokeEffect");
                var tex = TextureAssets.Npc[npc.type].Value;
                Color color = new Color(124, 28, 255);
                shader.Parameters["uImageSize"].SetValue(tex.Size());
                shader.Parameters["uColor"].SetValue(color.ToVector4());
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.CurrentTechnique.Passes[0].Apply();
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (darkRevealed)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        public override void ResetEffects(NPC npc)
        {
            darkRevealed = false;
        }
    }
}
