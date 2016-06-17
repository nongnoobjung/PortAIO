#region

using System;
using System.Linq;
using Infected_Twitch.Core;
using Infected_Twitch.Menus;
using LeagueSharp.SDK;

#endregion

namespace Infected_Twitch.Event
{
    internal class EOnDeath
    {
        public static void Update(EventArgs args)
        {
            if(!MenuConfig.EBeforeDeath) return;
            var target = GameObjects.EnemyHeroes.FirstOrDefault(x => x.LSIsValidTarget(1200) && Dmg.Stacks(x) > 0 && !x.IsDead && !x.IsZombie && !x.IsInvulnerable);
            if(target == null) return;

            if (!Spells.E.IsReady()) return;

            if (GameObjects.Player.HealthPercent <= 6.5)
            {
                Spells.E.Cast();
            }
        }
    }
}
