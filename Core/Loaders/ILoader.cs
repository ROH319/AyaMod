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

        /// <summary>
        /// 对每个Type进行加载
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="type"></param>
        public void Load(Mod mod, Type type) { }

        public void PostLoad(Mod mod) { }

        public void PreSetUp(Mod mod) { }

        /// <summary>
        /// 对每个Type进行设置
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="type"></param>
        public void SetUp(Mod mod, Type type) { }

        public void PostSetUp(Mod mod) { }

        public void PreUnLoad(Mod mod) { }

        public void UnLoad(Mod mod, Type type) { }
    }
}
