using AyaMod.Content.Items.Films;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AyaMod.Core.Prefabs
{
    public abstract class BaseFilm : ModItem
    {
        public override string Texture => AssetDirectory.Films + Name;
        public virtual StatModifier DamageModifier => StatModifier.Default;
        public List<(string name, string value, string valueDev)> FilmArgs = [];
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 999;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.ammo = ItemType<CameraFilm>();
        }

        public override bool? CanBeChosenAsAmmo(Item weapon, Player player)
        {
            if (weapon.ModItem is BaseCamera)
            {
                return true;
            }
            return base.CanBeChosenAsAmmo(weapon, player);
        }
        public virtual float EffectChance => 100;

        public virtual float GetTotalChance(Player player) => player.Camera().FilmEffectChanceModifier.ApplyTo(EffectChance);

        public virtual bool CheckEffect(Player player)
        {
            return Main.rand.Next(100) < GetTotalChance(player);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            bool dev = Main.LocalPlayer.DevEffect();
            foreach (var tooltip in tooltips)
            {
                if (!tooltip.Name.StartsWith("Tooltip")) continue;
                var text = tooltip.Text;
                foreach(var args in FilmArgs)
                {
                    if (!text.Contains($"<{args.name}>")) continue;
                    string value = dev ? $"[c/808080:{args.value}]" : $"{args.value}";
                    string valueDev = dev ? $"{args.valueDev}" : $"[c/808080:{args.valueDev}]";
                    tooltip.Text = text.Replace($"<{args.name}>", value + "/" + valueDev);
                }
            }
        }

        public virtual void ModifyHitNPCFilm(BaseCameraProj projectile, NPC target, ref NPC.HitModifiers modifiers) { }
        public virtual void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnSnap(BaseCameraProj projectile) { }
        public virtual void OnSnapInSight(BaseCameraProj projectile) { }
        public virtual void PreClearProjectile(BaseCameraProj projectile) { }
        public virtual void OnClearProjectile(BaseCameraProj projectile) { }
        public virtual void PostClearProjectile(BaseCameraProj projectile, int capturecount) { }
        public virtual void PreAI(BaseCameraProj projectile) { }
    }
}
