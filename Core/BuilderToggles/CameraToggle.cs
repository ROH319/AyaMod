using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Humanizer;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Core.BuilderToggles
{
    public class CameraToggle : BuilderToggle
    {
        public override string Texture => AssetDirectory.UI + "BuilderToggles/" + Name;
        public override string HoverTexture => Texture;
        public override bool Active() => Main.player[Main.myPlayer].HeldCamera();

        public override Position OrderPosition => new After(TorchBiome);

        public static LocalizedText OnText { get; private set; }
        public static LocalizedText OffText { get; private set; }

        public override int NumberOfStates => 2;

        public static bool AutoSnapEnabled
        {
            get
            {
                var instance = ModContent.GetInstance<CameraToggle>();
                return instance.Active() && instance.CurrentState == 1;
            }
        }

        public override void SetStaticDefaults()
        {
            OnText = this.GetLocalization(nameof(OnText));
            OffText = this.GetLocalization(nameof(OffText));
        }

        public override string DisplayValue() => CurrentState switch
        {
            0 => OffText.WithFormatArgs(CameraPlayer.CameraManualSnapDamageModifier).Value,
            1 => OnText.WithFormatArgs(CameraPlayer.CameraAutoSnapDamageModifier).Value,
            _ => "???"
        };

        public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
        {
            drawParams.Frame = drawParams.Texture.Frame(4, 1, CurrentState == 0 ? 1 : 0);
            drawParams.Position += new Vector2(1, 0);
            drawParams.Scale = 0.8f;

            return base.Draw(spriteBatch, ref drawParams);
        }

        public override bool DrawHover(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
        {
            drawParams.Frame = drawParams.Texture.Frame(4, 1, CurrentState == 0 ? 3 : 2);
            drawParams.Position += new Vector2(1, 0);
            drawParams.Scale = 0.8f;

            return base.DrawHover(spriteBatch, ref drawParams);
        }

        public override bool OnLeftClick(ref SoundStyle? sound)
        {
            sound = SoundID.Unlock;
            return true;
        }
    }
}
