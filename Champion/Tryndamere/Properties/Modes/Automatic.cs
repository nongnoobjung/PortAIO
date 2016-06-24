using System;
using LeagueSharp;
using LeagueSharp.SDK;
using ExorAIO.Utilities;
using EloBuddy;

namespace ExorAIO.Champions.Tryndamere
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
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.LSIsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >= 75 &&
                Health.GetPrediction(GameObjects.Player, (int)(250 + Game.Ping/2f)) <= GameObjects.Player.MaxHealth/2 &&
                Vars.getCheckBoxItem(Vars.QMenu, "logical"))
            {
                Vars.Q.Cast();
            }

            /// <summary>
            ///     The Lifesaver R Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                GameObjects.Player.CountEnemyHeroesInRange(1500f) > 0 &&
                Vars.getCheckBoxItem(Vars.RMenu, "lifesaver"))
            {
				if (GameObjects.Player.HealthPercent < 17 ||
                    Health.GetPrediction(GameObjects.Player, (int)(250 + Game.Ping / 2f)) <= GameObjects.Player.MaxHealth / 5)
                {
					Vars.R.Cast();
				}
            }
        }
    }
}