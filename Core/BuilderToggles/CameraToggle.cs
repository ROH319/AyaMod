using AyaMod.Content.Items.Testing;
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
        public static LocalizedText OnDmgText { get; private set; }
        public static LocalizedText OffText { get; private set; }
        public static LocalizedText OffDmgText { get; private set; }
        public static LocalizedText DisabledText { get; private set; }

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
            OnDmgText = this.GetLocalization(nameof(OnDmgText));
            OffText = this.GetLocalization(nameof(OffText));
            OffDmgText = this.GetLocalization(nameof(OffDmgText));
            DisabledText = this.GetLocalization(nameof(DisabledText));
        }

        public override string DisplayValue() =>
            CurrentState switch
            {
                0 => OffText.Value,
                1 => OnText.Value,
                _ => "???"
            } + "\n" +
            (Main.player[Main.myPlayer].HasEffect<NoDmgModifier>() ? DisabledText.Value :
            CurrentState switch
            {
                0 => OffDmgText.WithFormatArgs(CameraPlayer.CameraManualSnapDamageModifier).Value,
                1 => OnDmgText.WithFormatArgs(CameraPlayer.CameraAutoSnapDamageModifier).Value,
                _ => "???"
            });

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
