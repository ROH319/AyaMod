using AyaMod.Content.Buffs;
using AyaMod.Content.Projectiles.Auras;
using AyaMod.Core;
using AyaMod.Core.Attributes;
using AyaMod.Core.Globals;
using AyaMod.Core.ModPlayers;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    [PlayerEffect(OverrideEffectName = RumorBroadcasterSet)]
    public class RumorBroadcasterHelmet : ModItem, IPlaceholderItem 
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, CritBonus);
        public const string RumorBroadcasterSet = "RumorBroadcasterSet";
        public static LocalizedText RumorBroadcasterBonus { get; set; }

        public static int DamageBonus = 15;
        public static int CritBonus = 10;
        public override void Load()
        {
            AyaPlayer.DoubleTapHook += RumorBroadcasterKeyEffect;
            GlobalCamera.SnapHook += RumorBroadcasterSnap;
        }

        public override void SetStaticDefaults()
        {
            RumorBroadcasterBonus = this.GetLocalization("RumorBroadcasterBonus");
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 26;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(silver: 3));
            Item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
            player.GetCritChance<ReporterDamage>() += CritBonus;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<RumorBroadcasterChestplate>() && legs.type == ItemType<RumorBroadcasterGreaves>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = RumorBroadcasterBonus.Value;
            player.AddEffect(RumorBroadcasterSet);
            RumorBroadcasterSetEffect(player);
        }
        public static void RumorBroadcasterSetEffect(Player player)
        {
            int type = ProjectileType<RumorVortex>();
            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, type, 60, 0f, player.whoAmI, 6 * 60);
        }
        public static void RumorBroadcasterKeyEffect(Player player)
        {
            if (!player.HasEffect(RumorBroadcasterSet)) return;
            foreach(Projectile p in Main.ActiveProjectiles)
            {
                if (p.type != ProjectileType<RumorVortex>()) continue;
                (p.ModProjectile as RumorVortex).Release();
            }
        }

        public static void RumorBroadcasterSnap(Core.Prefabs.BaseCameraProj projectile)
        {
            if (!projectile.player.HasEffect(RumorBroadcasterSet)) return;
            //每5次拍摄触发
            if (projectile.player.Camera().GlobalSnapCounter % 5 != 0) return;

            var aura = AuraFriendly.Spawn(projectile.Projectile.GetSource_FromAI(), projectile.Projectile.Center, 2 * 60, BuffType<MadnessBuff>(),
                3 * 60, 300f, new Color(255, 247, 170, 128), new Color(255, 255, 81, 156), projectile.player.whoAmI);
            aura.SetRadiusFadeout(1f, Common.Easer.Ease.Linear);
            aura.SetAlphaFadeout(0.6f, Common.Easer.Ease.OutSine);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class RumorVortex : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;
        public ref float ChargeTime => ref Projectile.ai[0];
        public ref float Released => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            Projectile.width = Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10 * 60;
            Projectile.SetImmune(10);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)ChargeTime;
        }
        public void Release() { Released = 2; }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player == null || !player.Alive()) return;
            if (!player.HasEffect(RumorBroadcasterHelmet.RumorBroadcasterSet)) Projectile.Kill();

            float range = 200;
            NPC npc = AyaUtils.FindCloestNPC(player.Center, range, false);
            Vector2 idlePos = player.Center;
            if (npc == null)
            {
                idlePos = player.Center + MathF.Cos(Main.GameUpdateCount * 0.1f).ToRotationVector2() * range / 2;
            }
            else
            {
                idlePos = npc.Center;
            }
            Projectile.Center = Vector2.Lerp(Projectile.Center, idlePos, 0.2f);

            if (Released < 1) 
            {
                SpreadRumor(Projectile, 3, 300f);
            }
            else
            {
                ReleaseEffect(player);
            }

            float factor = Projectile.TimeleftFactor();
            Projectile.Opacity = Utils.Remap(factor, 0, 1f, 1f, 0f);

            if (Projectile.timeLeft < 3 && Released < 1) Projectile.timeLeft++;
            Projectile.rotation += 0.02f;
        }
        public void ReleaseEffect(Player player)
        {
            SpreadRumorShot(6, 500f, Projectile.GetSource_FromAI(), Projectile.Center, 10f, Projectile.damage, player.whoAmI);
            Projectile.Kill();
        }
        public static void SpreadRumorShot(int max, float range, IEntitySource source, Vector2 pos, float speed, int damage, int owner)
        {
            int counter = 0;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (counter >= max || pos.Distance(n.Center) > 500f || !n.CanBeChasedBy()) continue;

                Vector2 vel = pos.DirectionToSafe(n.Center) * speed;
                int type = ProjectileType<RumorShot>();
                Projectile.NewProjectileDirect(source, pos, vel, type, damage, 0f, owner);
                counter++;
            }
        }
        public static void SpreadRumor(Projectile projectile, int max, float range)
        {
            foreach(var npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy()) continue;
                var gnpc = npc.GetGlobalNPC<AyaGlobalNPC>();
                gnpc.SpreadRumorCounter++;
                if (!npc.HasBuff<MadnessBuff>()) continue;
                if (!projectile.Colliding(projectile.getRect(), npc.getRect())) continue;
                if(gnpc.SpreadRumorCounter > 2 * 60)
                {

                    SpreadRumorShot(max, range, npc.GetSource_FromThis(), npc.Center, 10f, npc.damage / 3, Main.myPlayer);
                    gnpc.SpreadRumorCounter = 0;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            float scale = texture.Width / (float)Projectile.width * Projectile.scale;
            int repeat = 6;
            for(int i = 0; i < repeat; i++)
            {
                float rot = Projectile.rotation + MathHelper.PiOver2 / repeat * i;
                float alpha = Utils.Remap(MathF.Cos((float)(i / (float)repeat * MathHelper.Pi + Main.timeForVisualEffects * 0.01f)), -1f, 1f, 0.1f, 1f);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * alpha, rot, texture.Size() / 2, scale, 0);

            }

            return false;
        }
    }
    public class RumorShot : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + "RumorVortex";
        public ref float StickyToNPC => ref Projectile.ai[0];
        public ref float TransformFlag => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10 * 60;
            Projectile.SetImmune(10);
        }
        public bool Transformed => TransformFlag < 1;
        public void Transform()
        { 
            TransformFlag = 2;
            Projectile.ResetLocalNPCHitImmunity();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            StickyToNPC = target.whoAmI;
            if (!Transformed) Transform();
        }
        public override void AI()
        {
            if (!Transformed)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Obsidian, Projectile.velocity.Length(5), Scale: 2f);
                d.noGravity = true;
            }
            else 
            {
                RumorVortex.SpreadRumor(Projectile, 3, 300f);
                if (StickyToNPC >= 0)
                {
                    NPC npc = Main.npc[(int)StickyToNPC];
                    if (npc != null && npc.CanBeChasedBy())
                    {
                        float dist = Projectile.Distance(npc.Center);
                        if (dist < 100f && dist > 20f)
                            Projectile.Center = Vector2.Lerp(Projectile.Center, npc.Center, 0.05f);
                    }
                    else StickyToNPC = -1;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Transformed) return false;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            float scale = texture.Width / (float)Projectile.width * Projectile.scale;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, scale, 0);

            return false;
        }
    }
    [AutoloadEquip(EquipType.Body)]
    public class RumorBroadcasterChestplate : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBonus);

        public static int CritBonus = 5;

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(silver: 3));
            Item.defense = 12;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ReporterDamage>() += CritBonus;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class RumorBroadcasterGreaves : ModItem, IPlaceholderItem
    {
        public override string Texture => AssetDirectory.Armors + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBonus, MoveSpeedBonus);

        public static int DamageBonus = 10;
        public static int MoveSpeedBonus = 8;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(silver: 3));
            Item.defense = 12;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ReporterDamage>() += DamageBonus / 100f;
            player.moveSpeed += MoveSpeedBonus / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
