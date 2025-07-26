using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Core
{
    public struct FloatModifier
    {
        public static readonly StatModifier Default = new();

        public float Additive { get; set; } = 0f;
        public float Multiplicative { get; set; } = 1f;

        public FloatModifier() { }
        
        public FloatModifier(float additive, float multiplicative)
        {
            Additive = additive;
            Multiplicative = multiplicative;
        }

        public FloatModifier SetAdditive(float additive) { Additive = additive; return this; }
        public FloatModifier SetMultiplicative(float multiplicative) { Multiplicative = multiplicative; return this; }

        public float Apply(float value)
            => (value + Additive) * Multiplicative;

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            if (obj is not FloatModifier f)
                return false;
            return this == f;
        }

        public static bool operator ==(FloatModifier a, FloatModifier b)
            => a.Additive == b.Additive && a.Multiplicative == b.Multiplicative;
        public static bool operator !=(FloatModifier a, FloatModifier b)
            => a.Additive != b.Additive || a.Multiplicative != b.Multiplicative;

    }
}
