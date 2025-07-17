using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace AyaMod.Core.Configs
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public static ClientConfig Instance;

        [Header("CameraRelated")]
        [DefaultValue(0.6f)]
        public float SnapVolume;

        [DefaultValue(true)]
        public bool SnapFlash;

        [DefaultValue(1f)]
        public float SnapFlashAlpha;
    }
}
