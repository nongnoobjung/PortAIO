using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

namespace ExorSDK.Champions.Ashe
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
            ///     The Q Combo Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.AARange) &&
                GameObjects.Player.HasBuff("asheqcastready") &&
                Vars.getCheckBoxItem(Vars.QMenu, "combo"))
            {
                Vars.Q.Cast();
            }

            if (Targets.Target.LSIsValidTarget(Vars.AARange + 20))
            {
                return;
            }

            /// <summary>
            ///     The W Combo Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
				!Invulnerable.Check(Targets.Target) &&
                Targets.Target.LSIsValidTarget(Vars.W.Range) &&
                Vars.getCheckBoxItem(Vars.WMenu, "combo"))
            {
                if (!Vars.W.GetPrediction(Targets.Target).CollisionObjects.Any())
                {
                    Vars.W.Cast(Vars.W.GetPrediction(Targets.Target).UnitPosition);
                }
            }


            /// <summary>
            ///     The E -> R Combo Logics.
            /// </summary>
            if (Vars.R.IsReady() &&
				!Invulnerable.Check(Targets.Target, DamageType.Magical, false) &&
                Vars.getCheckBoxItem(Vars.RMenu, "combo") &&
                Vars.getCheckBoxItem(Vars.WhiteListMenu, Targets.Target.ChampionName.ToLower()))
            {
				if (!Vars.R.GetPrediction(Targets.Target).CollisionObjects.Any())
                {
					if (Vars.E.IsReady() &&
						Vars.getCheckBoxItem(Vars.EMenu, "logical"))
					{
						Vars.E.Cast(Vars.E.GetPrediction(Targets.Target).UnitPosition);
					}

					Vars.R.Cast(Vars.R.GetPrediction(Targets.Target).UnitPosition);
				}
            }
        }
    }
}