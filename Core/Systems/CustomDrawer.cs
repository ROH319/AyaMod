using AyaMod.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;

namespace AyaMod.Core.Systems
{
    /// <summary>
    /// 与绘制有关的自定义钩子
    /// </summary>
    public class CustomDrawer : ModSystem
    {
        public delegate void DrawDelegate();
        public static event DrawDelegate FilterDrawer = () => { };

        public static event DrawDelegate PreProjectileDrawer = () => { };
        public static event DrawDelegate PostProjectileDrawer = () => { };
        public override void Load()
        {
            if (Main.dedServ) return;
            On_FilterManager.EndCapture += On_FilterManager_EndCapture;
            On_Main.InitTargets_int_int += On_Main_InitTargets_int_int;
            On_Main.DrawInfernoRings += On_Main_DrawInfernoRings;

            On_Main.DrawDust += ParticleManager.ParticleDrawer;
            On_Main.DrawProjectiles += On_Main_DrawProjectiles;
        }

        public override void Unload()
        {
            if (Main.dedServ) return;
            On_FilterManager.EndCapture -= On_FilterManager_EndCapture;
            On_Main.InitTargets_int_int -= On_Main_InitTargets_int_int;
            On_Main.DrawInfernoRings -= On_Main_DrawInfernoRings;

            On_Main.DrawDust -= ParticleManager.ParticleDrawer;
            On_Main.DrawProjectiles -= On_Main_DrawProjectiles;
        }

        private void On_Main_InitTargets_int_int(On_Main.orig_InitTargets_int_int orig, Main self, int width, int height)
        {
            orig(self, width, height);
            NewScreenTarget();
        }
        private void On_FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            //if (!CaptureManager.Instance.IsCapturing)
            {
                if (Main.screenTarget.RenderTargetUsage == RenderTargetUsage.DiscardContents)
                {
                    NewScreenTarget();
                }
                foreach (DrawDelegate d in FilterDrawer.GetInvocationList())
                {
                    d.Invoke();
                }
            }
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);

        }
        private void On_Main_DrawInfernoRings(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);
        }

        private void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            foreach (DrawDelegate d in PreProjectileDrawer.GetInvocationList())
            {
                d.Invoke();
            }
            orig(self);
            foreach (DrawDelegate d in PostProjectileDrawer.GetInvocationList())
            {
                d.Invoke();
            }
        }

        public void NewScreenTarget()
        {
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            Main.screenTarget.Dispose();
            Main.screenTarget = new RenderTarget2D(gd,
                gd.PresentationParameters.BackBufferWidth,
                gd.PresentationParameters.BackBufferHeight, false,
                gd.PresentationParameters.BackBufferFormat, DepthFormat.None,
                0, RenderTargetUsage.PreserveContents);
        }
    }
}
