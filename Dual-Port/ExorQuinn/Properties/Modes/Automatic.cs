using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp.SDK;
using EloBuddy.SDK;

namespace ExorSDK.Champions.Quinn
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
            /// <summary>
            ///     Block Attacks while in R stance.
            /// </summary>
            Orbwalker.DisableAttacking = Vars.R.Instance.Name.Equals("QuinnRFinale");

            if (GameObjects.Player.CountEnemiesInRange(GameObjects.Player.GetAutoAttackRange()) == 0)
            {
                Orbwalker.ForcedTarget = null;
            }

            /// <summary>
            ///     The Focus Logic (Passive Mark).
            /// </summary>
            foreach (var target in GameObjects.EnemyHeroes.Where(
                t =>
                    t.HasBuff("quinnw") &&
                    t.LSIsValidTarget(Vars.AARange)))
            {
                Orbwalker.ForcedTarget = target;
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Vars.getCheckBoxItem(Vars.WMenu, "logical"))
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(
                    x =>
                        !x.IsDead &&
                        !x.IsVisible &&
                        x.Distance(GameObjects.Player.ServerPosition) < Vars.W.Range))
                {
                    Vars.W.Cast();
                }

                if (Vars.Locations.Any(h => GameObjects.Player.Distance(h) < Vars.W.Range))
                {
                    Vars.W.Cast();
                }
            }

            /// <summary>
            ///     The Automatic R Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                GameObjects.Player.InFountain() &&
                Vars.R.Instance.Name.Equals("QuinnR"))
            {
                Vars.R.Cast();
            }
        }
    }
}