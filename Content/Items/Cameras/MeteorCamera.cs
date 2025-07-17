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
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AyaMod.Common.Easer;

namespace AyaMod.Content.Items.Cameras
{
    public class MeteorCamera : BaseCamera
    {

        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 30;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<MeteorCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 0, 78, 0));
            SetCameraStats(0.03f, 104, 2f);
            SetCaptureStats(100, 5);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MeteoriteBar, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class MeteorCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(223, 159, 137);
        public override Color innerFrameColor => new Color(147, 104, 87) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(228, 162, 174).AdditiveColor() * 0.5f;

        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;
            Player owner = Main.player[Projectile.owner];

            int dustamount = 90;
            float startRot = owner.AngleToSafe(Projectile.Center);
            float length = 70;
            for (int i = 0; i < dustamount; i++)
            {
                float factor = (float)i / dustamount;
                float rot = MathHelper.TwoPi * factor + startRot;
                Vector2 dir = rot.ToRotationVector2();
                float radius = length * 0.8f + MathF.Sin(factor * MathHelper.TwoPi * 5) * length / 2;
                Vector2 pos = Projectile.Center + dir * radius;
                Vector2 vel = (pos - Projectile.Center).Length(5);
                Dust d = Dust.NewDustPerfect(pos, DustID.YellowStarDust, vel,Scale:1f);
                d.noGravity = true;

            }
            {
                Vector2 vel = new Vector2(0, -7);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + vel * 10, vel, ModContent.ProjectileType<MeteorStar>(), (int)(Projectile.damage * 0.22f), Projectile.knockBack / 4, Projectile.owner, -1, Projectile.whoAmI);
            }
            base.OnSnap();
        }
    }

    public class MeteorStar : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "RoundTriangle2";

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 20);
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public override void AI()
        {

            float factor = Projectile.TimeleftFactor();

            if (Projectile.ai[0] < 0)
            {
                Projectile.Opacity = 1 - factor;
                Projectile.rotation += 0.08f;
            }
            else
            {
                NPC npc = Main.npc[(int)Projectile.ai[0]];
                if (npc != null && npc.CanBeChasedBy())
                {
                    float length = Projectile.velocity.Length();
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity,Projectile.DirectionToSafe(npc.Center) * length,0.1f);
                }
                Projectile.rotation += 0.01f;

            }

            //Projectile.Opacity = EaseManager.Evaluate(Ease.OutCubic, factor, 1);
            //Projectile.velocity *= 0.95f;

            int dustamount = 1;
            if (Projectile.ai[0] >= 0) dustamount++;
            for (int i = 0;i < dustamount;i++)
            {
                float speedx = Projectile.velocity.X + Main.rand.NextFloat(-2, 2);
                float speedy = Projectile.velocity.Y + Main.rand.NextFloat(-2, 2);
                Dust d = Dust.NewDustDirect(Projectile.position + Projectile.Size / 4, Projectile.width / 2, Projectile.height / 2, DustID.YellowStarDust, speedx, speedy,Scale:1.2f);
                d.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] < 0)
            {
                NPC npc = Main.projectile[(int)Projectile.ai[1]].FindCloestNPC(2000, true, true);
                if (npc == null) return;
                int count = 1;
                if (Main.rand.NextBool(3)) count++;
                for (int i = 0; i < count; i++)
                {
                    Vector2 pos = npc.Center + new Vector2(Main.rand.NextFloat(-200, 200), -1000);
                    var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, pos.DirectionToSafe(npc.Center) * 20, Projectile.type, Projectile.damage * 3, Projectile.knockBack, Projectile.owner, npc.whoAmI);
                    p.extraUpdates = 1;
                    p.Scale(1.5f, false);
                }
            }
            else
            {
                int size = 150;
                Projectile.penetrate = -1;
                Projectile.SetHitbox(size);
                Projectile.Damage();
            }


            int dustamount = 60;
            float startRot = Projectile.rotation + MathHelper.TwoPi / 10;
            float length = 30;
            for (int i = 0; i < dustamount; i++)
            {
                float factor = (float)i / dustamount;
                float rot = MathHelper.TwoPi * factor + startRot;
                Vector2 dir = rot.ToRotationVector2();
                float radius = length * 0.8f + MathF.Sin(factor * MathHelper.TwoPi * 5) * length / 2;
                Vector2 pos = Projectile.Center + dir * radius;
                Vector2 vel = (pos - Projectile.Center).Length(4);
                Dust d = Dust.NewDustPerfect(pos, DustID.YellowStarDust, vel,Scale:1.5f);
                d.noGravity = true;

            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, RenderHelper.MaxAdditive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);


            DrawStar(texture, Projectile.Center, new Color(255, 255, 128), 1f, 0.5f, 0.8f, 0.9f);
            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone);


            DrawStar(texture, Projectile.Center, Color.White, 0.7f, 0.5f, 0.8f, 0.4f);

            return false;
        }

        public void DrawStar(Texture2D texture, Vector2 center, Color color, float alpha, float scalex, float scaley, float scaleMult)
        {
            color = color.AdditiveColor() * alpha * Projectile.Opacity;
            Vector2 scale = new Vector2(scalex, scaley) * scaleMult * Projectile.scale;
            for (int i = 0; i < 5; i++)
            {
                float rot = MathHelper.TwoPi * (float)i / 5 + Projectile.rotation;
                Vector2 pos = center + rot.ToRotationVector2() * 7 - Main.screenPosition;
                Main.spriteBatch.Draw(texture, pos, null, color, rot + MathHelper.PiOver2, texture.Size() / 2, scale, 0, 0);

            }
        }
    }
}
