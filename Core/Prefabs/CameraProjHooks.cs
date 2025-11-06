using AyaMod.Core.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Core.Prefabs
{
    public partial class BaseCameraProj
    {
        public void CombinedPreClear()
        {
            PreClear();
            UpdateFilm(film => film.PreClearProjectile(this));
            GlobalCamera.PreClear(this);
        }

        public void CombinedPostClear(int captureCount)
        {
            PostClear(captureCount);
            UpdateFilm(film => film.PostClearProjectile(this, captureCount));
            GlobalCamera.PostClear(this, captureCount);
        }

        public void CombinedHoverNPC(NPC npc)
        {
            HoverNPC(npc);
            GlobalCamera.HoverNPC(this, npc);
        }

        public void CombinedNotHoverNPC(NPC npc)
        {
            NotHoverNPC(npc);
            GlobalCamera.NotHoverNPC(this, npc);
        }

        public void CombinedOnSnap()
        {
            OnSnap();
            UpdateFilm((film => film.OnSnap(this)));
            GlobalCamera.OnSnap(this);
        }

        public void CombinedOnSnapInSight()
        {
            OnSnapInSight();
            UpdateFilm((film => film.OnSnapInSight(this)));
            GlobalCamera.OnSnapInSight(this);
        }
    }
}
