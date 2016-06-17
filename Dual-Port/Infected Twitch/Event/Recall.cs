#region

using System;
using Infected_Twitch.Core;
using Infected_Twitch.Menus;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

#endregion

namespace Infected_Twitch.Event
{
    internal class Recall
    {
        public static void Load()
        {
            Spellbook.OnCastSpell += (sender, eventArgs) =>
            {
               if (eventArgs.Slot != SpellSlot.Recall) return;
                if (!MenuConfig.QRecall) return;
                if (!Spells.Q.IsReady()) return;

                Spells.Q.Cast();
                DelayAction.Add((int)Spells.Q.Delay + 300, ()=> GameObjects.Player.Spellbook.CastSpell(SpellSlot.Recall));
                eventArgs.Process = false;
            };
        }
    }
}
