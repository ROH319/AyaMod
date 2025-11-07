using AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Content.Items.PrefixHammers
{
    public class BloodthirstyHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Bloodthirsty>();
        }
    }
    public class ClassicalHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Classical>();
        }
    }
    public class FrugalHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Frugal>();
        }
    }
    public class MindControllerHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<MindController>();
        }
    }
}
