using System;
using System.Linq;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using EloBuddy;
using EloBuddy.SDK;

namespace ExorAIO.Champions.Ezreal
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
            ///     The Q LastHit Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "farmhelper")) &&
                Vars.getSliderItem(Vars.QMenu, "farmhelper") != 101)
            {
                foreach (var minion in Targets.Minions.Where(
                    m =>
                        !m.LSIsValidTarget(Vars.AARange) &&
                        Vars.GetRealHealth(m) > GameObjects.Player.GetAutoAttackDamage(m) &&
                        Vars.GetRealHealth(m) < (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)).OrderBy(
                            o =>
                                o.MaxHealth))
                {
                    if (!Vars.Q.GetPrediction(minion).CollisionObjects.Any(c => Targets.Minions.Contains(c)))
                    {
                        Vars.Q.Cast(Vars.Q.GetPrediction(minion).UnitPosition);
                    }
                }
            }

            /// <summary>
            ///     The Tear Stacking Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                !Targets.Minions.Any() &&
                Bools.HasTear(GameObjects.Player) &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) &&
                GameObjects.Player.CountEnemyHeroesInRange(1500) == 0 &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.MiscMenu, "tear")) &&
                Vars.getSliderItem(Vars.MiscMenu, "tear") != 101)
            {
                Vars.Q.Cast(Game.CursorPos);
            }

            if (GameObjects.Player.TotalMagicalDamage > GameObjects.Player.TotalAttackDamage)
            {
                return;
            }

            /// <summary>
            ///     Initializes the orbwalkingmodes.
            /// </summary>
            /// 

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (Orbwalker.LastTarget as AIHeroClient == null)
                {
                    return;
                }
            }

            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (Orbwalker.LastTarget as Obj_HQ == null &&
                    Orbwalker.LastTarget as Obj_AI_Turret == null &&
                    Orbwalker.LastTarget as Obj_BarracksDampener == null)
                {
                    return;
                }
            }

            else
            {
                if (!GameObjects.Jungle.Contains(Orbwalker.LastTarget) &&
                    Orbwalker.LastTarget as Obj_HQ == null &&
                    Orbwalker.LastTarget as AIHeroClient == null &&
                    Orbwalker.LastTarget as Obj_AI_Turret == null &&
                    Orbwalker.LastTarget as Obj_BarracksDampener == null)
                {
                    return;
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                ObjectManager.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "logical")) &&
                Vars.getSliderItem(Vars.WMenu, "logical") != 101)
            {
                foreach (var target in GameObjects.AllyHeroes.Where(
                    t =>
                        !t.IsMe &&
                        t.Spellbook.IsAutoAttacking &&
                        t.LSIsValidTarget(Vars.W.Range, false)))
                {
                    Vars.W.Cast(Vars.W.GetPrediction(target).UnitPosition);
                }
            }
        }
    }
}