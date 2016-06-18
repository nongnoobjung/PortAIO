using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;

namespace ExorSDK.Champions.Jhin
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Combo(EventArgs args)
        {
            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.R.Instance.Name.Equals("JhinRShot") &&
                Vars.getCheckBoxItem(Vars.RMenu, "combo"))
            {
                if (Targets.RTargets.Any())
                {
                    if (Vars.getCheckBoxItem(Vars.RMenu, "nearmouse"))
                    {
                        Vars.R.Cast(Vars.R.GetPrediction(Targets.RTargets.OrderBy(t => t.Distance(Game.CursorPos)).FirstOrDefault()).UnitPosition);
                    }
                    else
                    {
                        Vars.R.Cast(Vars.R.GetPrediction(Targets.RTargets.FirstOrDefault()).UnitPosition);
                    }
                }
				else
				{
					Vars.R.Cast(Game.CursorPos);
				}
            }

            if (Bools.HasSheenBuff() ||
				!Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range) &&
                GameObjects.Player.HasBuff("JhinPassiveReload") &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                Vars.Q.CastOnUnit(Targets.Target);
            }
        }
    }
}