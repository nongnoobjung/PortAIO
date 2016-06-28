using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;

namespace ExorAIO.Champions.Jhin
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
            ///     The Automatic Q LastHit Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.HasBuff("JhinPassiveReload") &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "lasthit")) &&
                Vars.getSliderItem(Vars.QMenu, "lasthit") != 101)
            {
                foreach (var minion in Targets.Minions.Where(
                    m =>
                        m.LSIsValidTarget(Vars.Q.Range) &&
                        Vars.GetRealHealth(m) <
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)))
                {
                    Vars.Q.CastOnUnit(minion);
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                !GameObjects.Player.IsUnderEnemyTurret() &&
                Vars.getCheckBoxItem(Vars.WMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.HasBuff("jhinespotteddebuff") &&
                        t.LSIsValidTarget(Vars.W.Range-150f) &&
                        !t.LSIsValidTarget(Vars.AARange+50f) &&
                        !Vars.W.GetPrediction(t).CollisionObjects.Any(
                            c =>
                                !c.HasBuff("jhinespotteddebuff") &&
                                GameObjects.EnemyHeroes.Contains(c)) &&
                        Vars.getCheckBoxItem(Vars.WhiteListMenu, t.ChampionName.ToLower())))
                {
                    if (Bools.IsImmobile(target))
                    {
                        Vars.W.Cast(target.ServerPosition);
                        return;
                    }
                    else
                    {
                        if (!target.LSIsFacing(GameObjects.Player) &&
                            GameObjects.Player.LSIsFacing(target))
                        {
                            Vars.W.Cast(Vars.W.GetPrediction(target).UnitPosition);
                            return;
                        }

                        if (target.LSIsFacing(GameObjects.Player) &&
                            !GameObjects.Player.LSIsFacing(target) &&
                            !GameObjects.EnemyHeroes.Any(
                                t =>
                                    t.LSIsValidTarget(Vars.Q.Range+50f)))
                        {
                            Vars.W.Cast(Vars.W.GetPrediction(target).UnitPosition);
                        }
                    }
                }
            }

            /// <summary>
            ///     The Automatic E Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.getCheckBoxItem(Vars.EMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.E.Range)))
                {
                    Vars.E.Cast(GameObjects.Player.ServerPosition.LSExtend(
                        target.ServerPosition,
                        GameObjects.Player.Distance(target) + target.BoundingRadius*2));
                }
            }
        }
    }
}