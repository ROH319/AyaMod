using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace AyaMod.Core.Systems.ParticleSystem
{
    public abstract class Particle : ModTexturedType
    {
        public int Type { get; internal set; }

        public int whoamI;

        public override string Texture => AssetDirectory.Particles + Name;

        public Vector2 Center;
        public Vector2 Velocity;
        public float Scale;
        public float Rotation;
        public float AngularSpeed;

        public float alpha;
        public float alphaMultiplier = 1f;
        /// <summary>
        /// 每帧透明度倍率
        /// </summary>
        public float AlphaMultiplier = 1f;
        /// <summary>
        /// 每帧缩放倍率
        /// </summary>
        public float ScaleMultiplier = 1f;
        /// <summary>
        /// 每帧速度倍率
        /// </summary>
        public float VelocityMultiplier = 1f;
        public bool active;
        public float timer;
        public float maxtime = 60;

        public Vector2[] oldCenter;
        public float[] oldRot;

        public int frame;
        public virtual int maxFrame => 0;
        public int frameCounter;

        public Color color;

        public IEntitySource Source;

        protected sealed override void Register()
        {
            ModTypeLookup<Particle>.Register(this);

            ParticleLoader.Particles ??= new List<Particle>();
            ParticleLoader.Particles.Add(this);

            Type = ParticleLoader.ReserveParticleID();
        }


        public virtual Particle NewInstance()
        {
            var inst = (Particle)Activator.CreateInstance(GetType(), true);
            inst.Type = Type;
            inst.whoamI = Main.rand.Next(1000);
            return inst;
        }



        public static T NewParticle<T>(IEntitySource source, Vector2 center, Vector2 velocity, Color color = default, float scale = 1f, float rotation = 0f, float alpha = 1f, int maxtime = 60) where T : Particle
        {
            if (Main.netMode == NetmodeID.Server)
                return null;
            T p = ParticleLoader.GetParticle(ParticleManager.ParticleType<T>()).NewInstance() as T;

            p.active = true;
            p.Center = center;
            p.Velocity = velocity;
            p.Scale = scale;
            p.Rotation = rotation;
            p.alpha = alpha;
            p.color = color;
            p.timer = 0;
            p.maxtime = maxtime;
            p.AngularSpeed = 0f;
            p.OnSpawn();
            p.Source = source;
            ParticleManager.Particles.Add(p);

            return p;
        }

        public static Particle NewParticle(int type, Vector2 center, Vector2 velocity, Color color = default, float scale = 1f, float rotation = 0f, int maxtime = 60)
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            Particle p = ParticleLoader.GetParticle(type).NewInstance();

            p.active = true;
            p.Center = center;
            p.Velocity = velocity;
            p.Scale = scale;
            p.Rotation = rotation;
            p.color = color;
            p.timer = 0;
            p.maxtime = maxtime;
            p.OnSpawn();
            ParticleManager.Particles.Add(p);

            return p;
        }

        public virtual void OnSpawn() { }

        public void InnerAI()
        {

            AI();
            if (ShouldUpdateCenter())
                Center += Velocity;
            if (ShouldUpdateRotation())
                Rotation += AngularSpeed;
            alpha *= AlphaMultiplier;
            Scale *= ScaleMultiplier;
            Velocity *= VelocityMultiplier;
            if (ShouldUpdateTimer())
                timer++;
            if (timer > maxtime) active = false;
            if (!active)
            {
                oldCenter = null;
                oldRot = null;
            }
        }

        public virtual void AI() { }

        public virtual bool ShouldUpdateCenter() => true;

        public virtual bool ShouldUpdateRotation() => true;

        public virtual bool ShouldUpdateTimer() => true;

        public float GetTimeFactor() => timer / maxtime;

        public virtual float GetAlpha() => alpha * alphaMultiplier;

        public void FrameLooping(int frameRate)
        {
            frame++;
            if(frameCounter > frameRate)
            {
                frame++;
                if (frame > maxFrame - 1)
                    frame = 0;
                frameCounter = 0;
            }
        }
        public void SetVelMult(float velMult)
        {
            VelocityMultiplier = velMult;
        }
        public void SetAlphaMult(float alphaMult)
        {
            AlphaMultiplier = alphaMult;
        }
        public void SetScaleMult(float scaleMult)
        {
            ScaleMultiplier = scaleMult;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GetTexture().Value;
            spriteBatch.Draw(texture, Center - Main.screenPosition, null, color * alpha, Rotation, texture.Size() / 2, Scale, 0f, 0f);
        }

        public Asset<Texture2D> GetTexture() => ParticleManager.ParticleAssets[Type];

        public void InitOldCenter(int length)
        {
            oldCenter = new Vector2[length];
            for (int i = 0; i < length; i++)
                oldCenter[i] = Center;
        }

        public void InitOldRot(int length)
        {
            oldRot = new float[length];
            for (int i = 0; i < length; i++)
                oldRot[i] = Rotation;
        }

        public void InitOldCaches(int length)
        {
            oldCenter = new Vector2[length];
            oldRot = new float[length];
            for (int i = 0; i < length; i++)
            {
                oldCenter[i] = Center;
                oldRot[i] = Rotation;
            }
        }

        /// <summary>
        /// 更新纪录点，较小的记录点为较新的
        /// </summary>
        public void UpdateOldCenter()
        {
            if (oldCenter is null || oldCenter.Length == 0)
                return;

            for (int i = oldCenter.Length - 1; i > 0; i--)
                oldCenter[i] = oldCenter[i - 1];

            oldCenter[0] = Center;
        }

        /// <summary>
        /// 更新纪录点，较小的记录点为较新的
        /// </summary>
        public void UpdateOldRot()
        {
            if (oldRot is null || oldRot.Length == 0)
                return;

            for (int i = oldRot.Length - 1; i > 0; i--)
                oldRot[i] = oldRot[i - 1];

            oldRot[0] = Rotation;
        }
    }
}
