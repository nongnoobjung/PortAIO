using System;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;
using System.Linq;

namespace ExorSDK.Champions.Caitlyn
{
    /// <summary>
    ///     The champion class.
    /// </summary>
    internal class Caitlyn
    {
        /// <summary>
        ///     Loads Caitlyn.
        /// </summary>
        public void OnLoad()
        {
            /// <summary>
            ///     Initializes the menus.
            /// </summary>
            Menus.Initialize();

            /// <summary>
            ///     Initializes the methods.
            /// </summary>
            Methods.Initialize();

            /// <summary>
            ///     Initializes the drawings.
            /// </summary>
            Drawings.Initialize();
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnUpdate(EventArgs args)
        {
            if (GameObjects.Player.IsDead)
            {
                return;
            }

            /// <summary>
            ///     Updates the spells.
            /// </summary>
            Spells.Initialize();

            /// <summary>
            ///     Initializes the Automatic actions.
            /// </summary>
            Logics.Automatic(args);

            /// <summary>
            ///     Initializes the Killsteal events.
            /// </summary>
            Logics.Killsteal(args);

            if (GameObjects.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Logics.Combo(args);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Logics.Harass(args);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Logics.Clear(args);
            }
        }

        /// <summary>
        ///     Called on do-cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                switch (args.SData.Name)
                {
                    case "CaitlynEntrapment":
                    case "CaitlynEntrapmentMissile":
                        if (Vars.W.IsReady() &&
                            Vars.getCheckBoxItem(Vars.WMenu, "combo"))
                        {
                            Vars.W.Cast(Vars.W.GetPrediction(Targets.Target).CastPosition);
                        }
                        break;

                    case "CaitlynYordleTrap":
                        if (Vars.Q.IsReady() &&
                            Vars.getCheckBoxItem(Vars.QMenu, "combo"))
                        {
                            Vars.Q.Cast(Targets.Target.ServerPosition);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        ///     Fired on an incoming gapcloser.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.GapCloserEventArgs" /> instance containing the event data.</param>
        public static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (Vars.E.IsReady() &&
                args.IsDirectedToPlayer &&
                args.Sender.LSIsValidTarget(Vars.E.Range) &&
                !Invulnerable.Check(args.Sender, DamageType.Magical, false) &&
                Vars.getCheckBoxItem(Vars.EMenu, "gapcloser"))
            {
                if (!Vars.E.GetPrediction(args.Sender).CollisionObjects.Any())
                {
                    Vars.E.Cast(args.Sender.ServerPosition);
                    return;
                }
            }

            if (Vars.W.IsReady() &&
                args.Sender.IsValidTarget(Vars.W.Range) &&
                !Invulnerable.Check(args.Sender, DamageType.Magical, false) &&
                Vars.getCheckBoxItem(Vars.WMenu, "gapcloser"))
            {
                if (!ObjectManager.Get<Obj_AI_Minion>().Any(
                    m =>
                        m.Distance(args.End) < 100f &&
                        m.CharData.BaseSkinName.Equals("caitlyntrap")))
                {
                    Vars.W.Cast(args.End);
                }
            }
        }

        /// <summary>
        ///     Called on interruptable spell.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="args">The <see cref="Events.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        public static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (Invulnerable.Check(args.Sender, DamageType.Magical, false))
            {
                return;
            }

            if (Vars.E.IsReady() &&
                args.Sender.IsValidTarget(Vars.E.Range) &&
                Vars.getCheckBoxItem(Vars.EMenu, "interrupter"))
            {
                if (!Vars.E.GetPrediction(args.Sender).CollisionObjects.Any())
                {
                    Vars.E.Cast(Vars.E.GetPrediction(args.Sender).UnitPosition);
                    return;
                }
            }

            if (Vars.W.IsReady() &&
                args.Sender.IsValidTarget(Vars.W.Range) &&
                Vars.getCheckBoxItem(Vars.WMenu, "interrupter"))
            {
                if (!ObjectManager.Get<Obj_AI_Minion>().Any(
                    m =>
                        m.CharData.BaseSkinName.Equals("caitlyntrap") &&
                        m.Distance(Vars.W.GetPrediction(args.Sender).CastPosition) < 100f))
                {
                    Vars.W.Cast(Vars.W.GetPrediction(args.Sender).CastPosition);
                }
            }
        }
    }
}