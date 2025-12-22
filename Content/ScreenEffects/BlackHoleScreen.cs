using AyaMod.Content.Items.Cameras;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace AyaMod.Content.ScreenEffects
{
    public class BlackHoleScreen : ModSceneEffect
    {
        public static float size = 1;

        public static bool Active = false;

        public override void Load()
        {
            if (Main.dedServ) return;
            Filters.Scene["CancerBlackHole"] = new Filter(new BlackHoleScreenShader(Request<Effect>("AyaMod/Effects/TestEffect", AssetRequestMode.ImmediateLoad), "BlackHole"), EffectPriority.VeryHigh);
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            return Active;
        }
        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
            {
                if (!Filters.Scene["CancerBlackHole"].IsActive())
                    Filters.Scene.Activate("CancerBlackHole");
            }
            else
            {
                if (Filters.Scene["CancerBlackHole"].IsActive())
                    Filters.Scene.Deactivate("CancerBlackHole");
            }
        }

    }

    public class BlackHoleScreenShader : ScreenShaderData
    {
        public EffectPass pass;

        public BlackHoleScreenShader(string passName) : base(passName)
        {
        }

        public BlackHoleScreenShader(Asset<Effect> shader, string passName) : base(shader, passName)
        {
        }

        public override void Apply()
        {
            //return;
            //foreach (var projectile in Main.ActiveProjectiles)
            //{
            //    if (projectile.type != ProjectileType<CancerBlackHole>()) continue;

            //    Vector2 screenPos = projectile.Center - Main.screenPosition;
            //    Vector2 normalizedPos = new Vector2(
            //        screenPos.X / Main.screenWidth,
            //        screenPos.Y / Main.screenHeight
            //    );

            //    float normalizedRadius = projectile.ai[2] / Main.screenHeight;
            //    Shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            //    Shader.Parameters["blackHolePosition"].SetValue(normalizedPos);
            //    Shader.Parameters["eventHorizonRadius"].SetValue(normalizedRadius);
            //    Shader.Parameters["accretionDiskRadius"].SetValue(normalizedRadius * 3.0f); // 吸积盘半径通常为视界半径3倍
            //    Shader.Parameters["rotationSpeed"].SetValue(1.0f); // 旋转速度

            //    if (Shader != null)
            //        pass = Shader.CurrentTechnique.Passes[0];
            //    pass?.Apply();

            //}
        }
    }
}
