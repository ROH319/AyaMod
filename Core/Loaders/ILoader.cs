using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Core.Loaders
{
    public interface ILoader
    {
        public void PreLoad(Mod mod) { }

        public void Load(Mod mod, Type type) { }

        public void PostLoad(Mod mod) { }

        public void PreSetUp(Mod mod) { }

        public void SetUp(Mod mod, Type type) { }

        public void PostSetUp(Mod mod) { }

        public void PreUnLoad(Mod mod) { }

        public void UnLoad(Mod mod, Type type) { }
    }
}
