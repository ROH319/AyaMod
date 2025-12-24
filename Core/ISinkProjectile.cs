using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Core
{
    /// <summary>
    /// “沉底”弹幕，会尝试在弹幕数组中维持index靠0的位置
    /// </summary>
    public interface ISinkProjectile
    {
        public float SinkDepth { get; }
    }
}
