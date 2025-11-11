using AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

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
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReforgeHammer>()
                .AddIngredient(ItemID.TissueSample, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class DevouringHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Devouring>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReforgeHammer>()
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class AbundantHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Abundant>();
        }
    }
    public class NecromanticHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Necromantic>();
        }
    }
    public class AlchemisticalHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Alchemistical>();
        }
    }
    public class PhantomHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Phantom>();
        }
    }
    public class TetradHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Tetrad>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReforgeHammer>()
                .AddIngredient(ItemID.WarriorEmblem)
                .AddIngredient(ItemID.RangerEmblem)
                .AddIngredient(ItemID.SorcererEmblem)
                .AddIngredient(ItemID.SummonerEmblem)
                .Register();
        }
    }
    public class EquilibratedHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Equilibrated>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReforgeHammer>()
                .AddIngredient(ItemID.LightShard)
                .AddIngredient(ItemID.DarkShard)
                .Register();
        }
    }
    public class GreedyHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Greedy>();
        }
    }
    public class SpectralHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Spectral>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReforgeHammer>()
                .AddIngredient(ItemID.Ectoplasm, 8)
                .Register();
        }
    }
}
