using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using EloBuddy;
using EloBuddy.SDK;

namespace ExorAIO.Champions.Nautilus
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
            ///     The Support Mode Option.
            /// </summary>
            if (Orbwalker.LastTarget != null &&
                Orbwalker.LastTarget is Obj_AI_Minion &&
                GameObjects.AllyHeroes.Any(a => a.Distance(GameObjects.Player) < 2500) &&
                Vars.getCheckBoxItem(Vars.MiscMenu, "support"))
            {
                Orbwalker.DisableAttacking = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
            }
        }
    }
}