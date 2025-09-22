using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Core.Prefabs
{
    public abstract class BaseDyeFilm : BaseFilm
    {
        public override string Texture => AssetDirectory.DyeFilms + Name;
        public virtual int DyeID => 0;
    }
}
