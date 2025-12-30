using AyaMod.Common.RecipeConditions;
using AyaMod.Content.Items.Films;
using AyaMod.Content.Items.Films.DyeFilms;
using AyaMod.Content.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Core.Systems
{
    public class RecipeSystem : ModSystem
    {
        public static int AnyDyeItem;
        public override void AddRecipeGroups()
        {
            var group = new RecipeGroup(() => Language.GetTextValue($"Mods.AyaMod.RecipeGroups.AnyDyeItem"),
                ItemID.RedHusk, ItemID.OrangeBloodroot, ItemID.YellowMarigold, ItemID.LimeKelp, ItemID.GreenMushroom, ItemID.TealMushroom,
                ItemID.CyanHusk, ItemID.SkyBlueFlower, ItemID.BlueBerries, ItemID.PurpleMucos, ItemID.VioletHusk, ItemID.PinkPricklyPear, ItemID.BlackInk);
            AnyDyeItem = RecipeGroup.RegisterGroup("AyaMod:AnyDyeItem", group);
        }
        public override void AddRecipes()
        {
            IzanagiObject.RegisterIzanagiRecipe(ItemID.CopperOre, ItemID.StoneBlock, 24, 24);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.TinOre, ItemID.StoneBlock, 24, 24);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.IronOre, ItemID.StoneBlock, 18, 18);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.LeadOre, ItemID.StoneBlock, 18, 18);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.SilverOre, ItemID.StoneBlock, 16, 16);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.TungstenOre, ItemID.StoneBlock, 16, 16);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.GoldOre, ItemID.StoneBlock, 12, 12);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.PlatinumOre, ItemID.StoneBlock, 12, 12);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.DemoniteOre, ItemID.StoneBlock, 9, 9);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.CrimtaneOre, ItemID.StoneBlock, 9, 9);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.Hellstone, ItemID.StoneBlock, 9, 9);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.Meteorite, ItemID.StoneBlock, 9, 9);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.CobaltOre, ItemID.StoneBlock, 8, 8);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.PalladiumOre, ItemID.StoneBlock, 8, 8);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.MythrilOre, ItemID.StoneBlock, 6, 6);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.OrichalcumOre, ItemID.StoneBlock, 6, 6);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.AdamantiteOre, ItemID.StoneBlock, 4, 4);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.TitaniumOre, ItemID.StoneBlock, 4, 4);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.ChlorophyteOre, ItemID.StoneBlock, 3, 3);

            IzanagiObject.RegisterIzanagiRecipe(ItemID.JungleGrassSeeds, ItemID.GrassSeeds, 30, 30);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.MushroomGrassSeeds, ItemID.GrassSeeds, 30, 30);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.CrimsonSeeds, ItemID.GrassSeeds, 30, 30);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.CorruptSeeds, ItemID.GrassSeeds, 30, 30);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.HallowedSeeds, ItemID.GrassSeeds, 30, 30);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.AshGrassSeeds, ItemID.GrassSeeds, 30, 30);

            IzanagiObject.RegisterIzanagiRecipe(ItemID.Amethyst, ItemID.Glass, 7, 7);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.Topaz, ItemID.Glass, 7, 7);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.Sapphire, ItemID.Glass, 5, 5);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.Emerald, ItemID.Glass, 5, 5);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.Ruby, ItemID.Glass, 3, 3);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.Diamond, ItemID.Glass, 3, 3);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.Amber, ItemID.Glass, 3, 3);
            IzanagiObject.RegisterIzanagiRecipe(ItemID.CrystalShard, ItemID.Glass, 3, 3);


            Recipe.Create(ItemID.LifeCrystal)
                .AddIngredient(ItemID.Glass, 4)
                .AddIngredient(ItemID.HealingPotion)
                .AddIngredient<IzanagiObject>()
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .Register();

            Recipe.Create(ItemID.LifeFruit)
                .AddRecipeGroup(RecipeGroupID.Fruit)
                .AddIngredient(ItemID.HealingPotion)
                .AddIngredient<IzanagiObject>()
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .Register();

            RegisterFilmRecipe();
        }

        public static void RegisterFilmRecipe()
        {
            List<int> filmList = [
                ItemType<ReflectiveFilm>(),
                ItemType<GelFilm>(),
                ItemType<PinkGelFilm>(),
                ItemType<AcidFilm>(),
                ItemType<PurpleOozeFilm>(),
                ItemType<FogboundFilm>(),
                ItemType<BloodbathFilm>(),
            ];
            List<int> filmPostEyes = [
                ItemType<HadesFilm>(),
                ItemType<ReflectiveFilm>(),
                ItemType<ShiftingSandsFilm>(),
            ];
            List<int> filmPostBrain = [
                ItemType<ShadowFilm>(),
                ItemType<GrimFilm>(),
                ItemType<BurningHadesFilm>(),
                ItemType<ReflectiveSilverFilm>(),
            ];
            List<int> filmPostSke = [
                ItemType<WispFilm>(),
                ItemType<BlueAcidFilm>(),
                ItemType<TwilightFilm>(),
            ];
            List<int> filmHardmode = [
                ItemType<PixieFilm>(),
                ItemType<ShiftingPearlsandsFilm>(),
                ItemType<ReflectiveGoldFilm>(),
                ItemType<InfernalWispFilm>(),
            ];
            List<int> filmPostAnyMech = [
                ItemType<UnicornWispFilm>(),
                ItemType<LokisFilm>(),
            ];
            List<int> filmPostAllMech = [
                ItemType<ReflectiveMetalFilm>(),
                ItemType<ChlorophyteFilm>(),
                ItemType<PhaseFilm>()
            ];
            List<int> filmPostPlant = [
                ItemType<GlowingMushroomFilm>(),
                ItemType<RedAcidFilm>(),
            ];
            List<int> filmPostGolem = [
                ItemType<NegativeFilm>(),
                ItemType<ReflectiveObsidianFilm>(),
                ItemType<MartianFilm>(),
                ItemType<MidnightRainbowFilm>(),
                ItemType<PrismaticFilm>(),
            ];
            List<int> filmPostKakaa = [
                ItemType<SolarFilm>(),
                ItemType<VortexFilm>(),
                ItemType<NebulaFilm>(),
                ItemType<StardustFilm>(),
            ];

            CreateFilmRecipe(filmList);
            CreateFilmRecipe(filmPostEyes, Condition.DownedEyeOfCthulhu);
            CreateFilmRecipe(filmPostBrain, Condition.DownedBrainOfCthulhu);
            CreateFilmRecipe(filmPostSke, Condition.DownedSkeletron);
            CreateFilmRecipe(filmHardmode, Condition.Hardmode);
            CreateFilmRecipe(filmPostAnyMech, Condition.DownedMechBossAny);
            CreateFilmRecipe(filmPostAllMech, Condition.DownedMechBossAll);
            CreateFilmRecipe(filmPostPlant, Condition.DownedPlantera);
            CreateFilmRecipe(filmPostGolem, Condition.DownedGolem);
            CreateFilmRecipe(filmPostKakaa, Condition.DownedCultist);

            CreateFilmRecipe([ItemType<ShadowflameHadesFilm>()], AyaConditions.DownedGoblinSummoner);
            CreateFilmRecipe([ItemType<LivingFlameFilm>()], Condition.DownedSkeletronPrime);
            CreateFilmRecipe([ItemType<LivingOceanFilm>()], Condition.DownedDestroyer);
            CreateFilmRecipe([ItemType<LivingRainbowFilm>()], Condition.DownedTwins);
        }
        public static void CreateFilmRecipe(List<int> filmList, Condition condition = null)
        {
            foreach (var film in filmList)
            {
                var recipe = Recipe.Create(film, 200)
                    .AddIngredient<CameraFilm>(200)
                    .AddIngredient<StrangePlantEssence>();
                if (condition != null) recipe.AddCondition(condition);
                recipe.Register();
            }
        }
    }
}
