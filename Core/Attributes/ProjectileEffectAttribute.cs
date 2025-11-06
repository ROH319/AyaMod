using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Core.Attributes
{
    /// <summary>
    /// 表示这个类是一个弹幕效果
    /// 可以使用<see cref="OverrideEffectName"/>修改效果名
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ProjectileEffectAttribute : Attribute
    {
        /// <summary>
        /// 覆盖的效果名
        /// </summary>
        public string OverrideEffectName;

        /// <summary>
        /// 额外的效果名
        /// </summary>
        public string[] ExtraEffectNames;
    }
}
