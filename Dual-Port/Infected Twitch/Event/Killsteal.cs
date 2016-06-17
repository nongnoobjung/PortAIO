#region

using System;
using Infected_Twitch.Core;
using Infected_Twitch.Menus;
using LeagueSharp.SDK;
using EloBuddy.SDK;

#endregion

namespace Infected_Twitch.Event
{
    internal class Killsteal
    {
        public static void Update(EventArgs args)
        {
            var target = TargetSelector.GetTarget(600f, EloBuddy.DamageType.Physical);

            if (MenuConfig.KillstealIgnite)
            {
                if (!Spells.Ignite.IsReady()) return;

                if (target.LSIsValidTarget(600f) && Dmg.IgniteDmg >= target.Health)
                {
                    GameObjects.Player.Spellbook.CastSpell(Spells.Ignite, target);
                }
            }

            if (target.HealthPercent <= 10 && !Spells.Q.IsReady())
            {
                Usables.Botrk();
            }
        }
    }
}
