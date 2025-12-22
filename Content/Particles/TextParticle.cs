using AyaMod.Core;
using AyaMod.Core.Systems.ParticleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;

namespace AyaMod.Content.Particles
{
    public class TextParticle : Particle
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;

        public Action action;

        public static TextParticle Spawn(IEntitySource source, Vector2 center, Vector2 vel, Action action)
        {
            TextParticle text = NewParticle<TextParticle>(source, center, vel);
            text.action = action;
            return text;
        }

        public override void AI()
        {
            action();
        }
    }
}
