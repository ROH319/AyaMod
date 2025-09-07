using AyaMod.Content.Items.Accessories.Movements;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AyaMod.Core.ModPlayers
{
    public partial class AyaPlayer
    {

        public const int DashDown = 0;
        public const int DashUp = 1;
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public enum DashType
        {
            None,
            GaleGeta0,
            GaleGeta1,
            GaleGeta2
        }
        public DashType AyaDash = DashType.None;
        public bool HasDash;
        public int DashDelay;
        public int DashTimer;
        public int DashDir = -1;

        public bool IsUltraDashing;
        public float UltraDashDir;

        /// <summary>
        /// 是否能够冲刺，判断条件为：钩爪勾到了，玩家不被拖拽，玩家不在坐骑上
        /// </summary>
        public bool CanDashSpecialCondition => Player.grappling[0] == -1 && !Player.tongued && !Player.mount.Active && !IsUltraDashing;
        
        public void BanVanallaDash()
        {
            Player.dashType = 0;
            Player.dash = 0;
        }

        public static void AddDashes(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            if (player.HasEffect<GaleGeta>())
            {
                GaleGeta.AddDash(player);
            }
            if(player.HasEffect<GaleGeta1>())
            {
                GaleGeta1.AddDash(player);
            }
            if(player.HasEffect<GaleGeta2>())
            {
                GaleGeta2.AddDash(player);
            }
        }

        public static void WhileDashing(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;
            AyaPlayer modPlayer = player.Aya();
            if (modPlayer.AyaDash == DashType.None)
                return;

            switch (modPlayer.AyaDash)
            {
                case DashType.GaleGeta2:
                    GaleGeta2.WhileDashing2(player, modPlayer.DashDir);
                    break;
                case DashType.GaleGeta1:
                    GaleGeta1.WhileDashing1(player, modPlayer.DashDir);
                    break;
                case DashType.GaleGeta0:
                    GaleGeta.WhileDashing0(player, modPlayer.DashDir);
                    break;
            }
        }

        public void UpdateDash()
        {
            if(DashDelay == 0 && DashDir != -1 && CanDashSpecialCondition)
            {

                switch (AyaDash)
                {
                    case DashType.GaleGeta0:
                        GaleGeta.GetaDash0(Player, DashDir);
                        break;
                    case DashType.GaleGeta1:
                        GaleGeta1.GetaDash1(Player, DashDir);
                        break;
                    case DashType.GaleGeta2:
                        GaleGeta2.GetaDash2(Player, DashDir);
                        break;
                    default:break;
                }
            }
            
            if(DashDelay > 0)
            {
                BanVanallaDash();
                DashDelay--;

                WhileDashing(Player);

            }

            if(DashTimer > 0)
            {

                BanVanallaDash();
                DashTimer--;
            }
        }

        public void ResetDashDir()
        {
            DashDir = -1;
            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
                DashDir = DashRight;
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
                DashDir = DashLeft;
        }
    }
}
