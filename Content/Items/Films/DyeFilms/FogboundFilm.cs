using AyaMod.Core;
using AyaMod.Core.Prefabs;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class FogboundFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";
        public override int DyeID => 4662;
    }
}
