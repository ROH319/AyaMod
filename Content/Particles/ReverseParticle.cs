using AyaMod.Core;
using AyaMod.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace AyaMod.Content.Particles
{
    public class ReverseParticle : Particle
    {
        public override string Texture => AssetDirectory.Projectiles + "SeijaArrow";
        public override int maxFrame => 2;
        public float RotSpeed;
        public static ReverseParticle Spawn(IEntitySource source, Vector2 center, Vector2 vel, float scale, float alpha, int frame, int maxTime = 3 * 60)
        {
            ReverseParticle reverse = NewParticle<ReverseParticle>(source, center, vel, Color.White, scale, 0f, alpha);
            reverse.maxtime = maxTime;
            reverse.frame = frame;
            return reverse;
        }
        public static void SpawnReverse(IEntitySource source, Vector2 center, float alpha, int maxTime = 60)
        {
            float length =30;
            float speed = length / (maxTime / 3);
            float rotspeed = MathHelper.Pi / (maxTime / 3f);
            float radius = speed / rotspeed;
            for (int i = -1; i < 2; i+=2)
            {
                int frame = i == -1 ? 0 : 1;
                Vector2 pos = center + new Vector2(radius, -length) * i;
                ReverseParticle reverse = Spawn(source, pos, Vector2.UnitY * i * speed, 1f, alpha, frame, maxTime);
                reverse.RotSpeed = rotspeed;
            }
        }
        public override void AI()
        {
            var factor = GetTimeFactor();

            if(factor > 1 / 3f && factor <= 2 / 3f)
            {
                Velocity = Velocity.RotatedBy(RotSpeed);
            }
            if (factor > 2 / 3f)
            {
                var i = frame == 0 ? -1 : 1;
                Velocity = -Vector2.UnitY * i * Velocity.Length();
            }
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
            float alphafadein = Utils.Remap(factor, 0, 1 / 6f, 0f, 1f);
            float alphafadeout = Utils.Remap(factor, 5 / 6f, 1f, 1f, 0f);
            alphaMultiplier = alphafadein * alphafadeout;
            base.AI();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GetTexture().Value;


            int frameWidth = texture.Width / maxFrame;
            int x = frameWidth * frame;

            Rectangle rect = new(x, 0, 14, 40);
            Color color = Color.White * GetAlpha();

            Main.EntitySpriteDraw(texture, Center - Main.screenPosition, rect,
                color, Rotation, texture.Size() / 2, Scale, 0);

        }
    }
}
