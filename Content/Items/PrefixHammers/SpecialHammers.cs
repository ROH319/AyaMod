using AyaMod.Content.Items.Materials;
using AyaMod.Content.Prefixes.CameraPrefixes.ExtraPrefixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;

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
                .AddIngredient(ItemID.TissueSample, 10)
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
                .AddIngredient(ItemID.ShadowScale, 10)
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
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReforgeHammer>()
                .AddIngredient(ItemID.Blinkroot, 5)
                .AddIngredient(ItemID.Daybloom, 5)
                .AddIngredient(ItemID.Deathweed, 5)
                .AddIngredient(ItemID.Fireblossom, 5)
                .AddIngredient(ItemID.Moonglow, 5)
                .AddIngredient(ItemID.Shiverthorn, 5)
                .AddIngredient(ItemID.Waterleaf, 5)
                .AddTile(TileID.AlchemyTable)
                .Register();
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
    public class NamelessHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Nameless>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IzanagiObject>(20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class PhotosyntheticHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Photosynthetic>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class HallowedHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Hallowed>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
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
                .AddIngredient(ItemID.Ectoplasm, 10)
                .Register();
        }
    }
    public class FestiveHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Festive>();
        }
    }
    public class HarvestingHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Harvesting>();
        }
    }
    public class IntelligentHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Intelligent>();
        }
    }
    public class SoaringHammer : BasePrefixHammer
    {
        
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Soaring>();
        }
    }
    public class EvolutiveHammer : BasePrefixHammer
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrefixToForge = GetInstance<Evolutive>();
        }
    }
}
