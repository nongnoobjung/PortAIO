#region

using System;
using Infected_Twitch.Core;
using Infected_Twitch.Menus;
using LeagueSharp.SDK;
using EloBuddy.SDK;
using EloBuddy;

#endregion

namespace Infected_Twitch.Event
{
    internal class Killsteal : Core.Core
    {
        public static void Update(EventArgs args)
        {
            var target = TargetSelector.GetTarget(600f, DamageType.Physical);

            if (MenuConfig.KillstealIgnite)
            {
                if (!Spells.Ignite.IsReady()) return;

                if (target.IsValidTarget(600f) && Dmg.IgniteDmg >= target.Health)
                {
                    GameObjects.Player.Spellbook.CastSpell(Spells.Ignite, target);
                }
            }

            if (MenuConfig.KillstealE)
            {
                if (Target == null || Target.IsDead || !Target.LSIsValidTarget(Spells.E.Range)) return;
                if (Dmg.Executable(Target))
                {
                    Spells.E.Cast();
                }
            }

            if (target.HealthPercent <= 10 && !Spells.Q.IsReady())
            {
                Usables.Botrk();
            }
        }
    }
}
