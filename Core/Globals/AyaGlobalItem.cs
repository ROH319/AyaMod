using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace AyaMod.Core.Globals
{
    public class AyaGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool WingUpdate(int wings, Player player, bool inUse)
        {
            var ayaPlayer = player.Aya();
            if (ayaPlayer.freeFlyFrame > 0)
            {
                ayaPlayer.freeFlyFrame--;
                player.wingTime++;
            }

            return base.WingUpdate(wings, player, inUse);
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.ModItem == null || item.ModItem is not IPlaceholderItem) return;

            tooltips.Add(new TooltipLine(Mod, "IPlaceholderItemText", AyaUtils.GetText("Items.Extras.IPlaceholderItemText"))
            {
                OverrideColor = Color.Red
            });
        }
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.ModItem == null || item.ModItem is not IPlaceholderItem) return true;

            Texture2D tex = TextureAssets.Item[item.type].Value;
            float timer = Math.Abs((Main.GlobalTimeWrappedHourly % 1) - 0.5f) * 2f;
            float range = 1 + timer * 2f;
            Color color = Color.White * (1f / timer);
            color.A = (byte)(color.A * 0.5f);

            for (int i = 0; i < 6; i++)
            {
                Vector2 dir = MathHelper.ToRadians(i * 60).ToRotationVector2();
                spriteBatch.Draw(tex, position + dir * range, frame, color, 0f, origin, scale, 0, 0f);
            }

            return true;
        }
    }
}
