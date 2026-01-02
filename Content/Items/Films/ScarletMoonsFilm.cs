using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AyaMod.Content.Items.Films
{
    public class ScarletMoonsFilm : BaseFilm
    {
        public override string Texture => AssetDirectory.Films + Name;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(HealPercent, MoonLeechHealPercent);
        public static int HealPercent = 2;
        public static int MoonLeechHealPercent = 1;
        public static int MaxHealAmount = 15;
        public static int MaxHealAmountDev = 20;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Yellow8, Item.sellPrice(copper:8));
            FilmArgs = [("maxheal", MaxHealAmount.ToString(), MaxHealAmountDev.ToString())];
        }
        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile == null) return;
            int healAmount = damageDone * HealPercent / 100;
            if(projectile.player.HasBuff(BuffID.MoonLeech))
            {
                healAmount = damageDone * MoonLeechHealPercent / 100;
            }
            healAmount = (int)MathHelper.Clamp(healAmount, 0, Main.LocalPlayer.DevEffect() ? MaxHealAmountDev : MaxHealAmount);
            projectile.player.Heal(healAmount);
        }

        public override void AddRecipes()
        {
            CreateRecipe(500)
                .AddIngredient<BloodyFilm>(500)
                .AddIngredient(ItemID.Ectoplasm, 5)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }
    }
}
