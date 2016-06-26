using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;

namespace ExorAIO.Champions.Cassiopeia
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
            if (Bools.HasSheenBuff() ||
                !Targets.Target.LSIsValidTarget())
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.E.Range) &&
                Targets.Target.HasBuffOfType(BuffType.Poison) &&
                !Invulnerable.Check(Targets.Target, DamageType.Magical, false) &&
                Vars.getCheckBoxItem(Vars.EMenu, "combo"))
            {
                DelayAction.Add(
                    Vars.getSliderItem(Vars.EMenu, "delay"), () =>
                {
                    Vars.E.CastOnUnit(Targets.Target);
                });
            }

            if (Invulnerable.Check(Targets.Target, DamageType.Magical))
            {
                return;
            }

            /// <summary>
            ///     The R Combo Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.getSliderItem(Vars.RMenu, "combo") != 6 &&
                Vars.getSliderItem(Vars.RMenu, "combo") <=
                    Targets.RTargets.Count())
            {
                Vars.R.Cast(Targets.RTargets[0].ServerPosition);
            }

            if (Targets.Target.HasBuffOfType(BuffType.Poison))
            {
                return;
            }

            /// <summary>
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range) &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).CastPosition);
                return;
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                !Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                !Targets.Target.LSIsValidTarget(Vars.AARange - GameObjects.Player.BoundingRadius) &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                Vars.W.Cast(Vars.W.GetPrediction(Targets.Target).CastPosition);
            }
        }
    }
}