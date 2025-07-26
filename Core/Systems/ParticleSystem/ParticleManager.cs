using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AyaMod.Core.Systems.ParticleSystem
{
    public class ParticleManager : ModSystem
    {

        public static List<Particle> Particles = new List<Particle>();

        public static Asset<Texture2D>[] ParticleAssets;

        public static int ParticleType<T>() where T : Particle => ModContent.GetInstance<T>()?.Type ?? 0;

        public static int count;


        public override void PostAddRecipes()
        {
            if (Main.dedServ) return;

            ParticleAssets = new Asset<Texture2D>[ParticleLoader.ParticleCount];
            for (int i = 0; i < ParticleLoader.ParticleCount; i++)
            {
                Particle particle = ParticleLoader.GetParticle(i);
                if (particle != null)
                {
                    ParticleAssets[i] = ModContent.Request<Texture2D>(particle.Texture);
                }
            }
            base.PostAddRecipes();
        }
        public override void Load()
        {
            if (Main.dedServ) return;
            On_Main.DrawDust += ParticleDrawer;
        }

        private void ParticleDrawer(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            if (Main.gameMenu) return;

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,Main.DefaultSamplerState,DepthStencilState.None,RasterizerState.CullNone,null,Main.GameViewMatrix.ZoomMatrix);
            for (int i = 0; i < ParticleManager.Particles.Count; i++)
            {
                Particle particle = ParticleManager.Particles[i];
                if (particle == null || !particle.active) continue;

                particle.Draw(Main.spriteBatch);
            }
            Main.spriteBatch.End();
        }

        public override void Unload()
        {
            ParticleLoader.Unload();

            ParticleAssets = null;
            Particles = null;
        }

        public override void PostUpdateDusts()
        {
            UpdateParticle();
        }


        public static void UpdateParticle()
        {
            if (Main.netMode == NetmodeID.Server)//不在服务器上运行
                return;
            if (Main.gameInactive)//不在游戏暂停时运行
                return;

            for (int i = 0; i < Particles.Count; i++)
            {
                Particle particle = Particles[i];

                if (particle == null)
                    continue;

                particle.AI();
                if (particle.ShouldUpdateCenter())
                    particle.Center += particle.Velocity;
                if (particle.ShouldUpdateRotation())
                    particle.Rotation += particle.AngularSpeed;
                if (!particle.active)
                {
                    particle.oldCenter = null;
                    particle.oldRot = null;
                }

            }

            Particles.RemoveAll(p => p == null || !p.active);
        }
    }
}
