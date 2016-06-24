using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy.SDK;
using EloBuddy;

namespace ExorAIO.Champions.Karma
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.LSIsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Support Mode Option.
            /// </summary>
            if (Orbwalker.LastTarget != null &&
                    Orbwalker.LastTarget is Obj_AI_Minion &&
                    GameObjects.AllyHeroes.Any(a => a.Distance(GameObjects.Player) < 2500) &&
                    Vars.getCheckBoxItem(Vars.MiscMenu, "support"))
            {
                Orbwalker.DisableAttacking = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
            }

            /// <summary>
            ///     The AoE E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.R.IsReady() &&
                Vars.getCheckBoxItem(Vars.RMenu, "empe") &&
                GameObjects.Player.CountEnemyHeroesInRange(2000f) >= 2 &&
                GameObjects.Player.CountAllyHeroesInRange(600f) >=
                    Vars.getSliderItem(Vars.EMenu, "aoe") &&
                Vars.getSliderItem(Vars.EMenu, "aoe") != 6)
            {
                Vars.R.Cast();
                Vars.E.CastOnUnit(GameObjects.Player);
            }
        }
    }
}