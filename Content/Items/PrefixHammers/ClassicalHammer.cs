using AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Content.Items.PrefixHammers
{
    public class ClassicalHammer : BasePrefixHammer
    {
        public override int PrefixTypeToForge => PrefixType<Classical>();
    }
}
