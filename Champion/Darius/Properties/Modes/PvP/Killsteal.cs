using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.Data.Enumerations;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK;

namespace ExorSDK.Champions.Darius
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
        public static void Killsteal(EventArgs args)
        {
            /// <summary>
            ///     The KillSteal R Logic.
            /// </summary>
            /// 
            var target = TargetSelector.GetTarget(Vars.R.Range, DamageType.Physical);
            if (target == null || !target.IsValid)
            {
                return;
            }
            if (Vars.getCheckBoxItem(Vars.RMenu, "killsteal") && Vars.R.IsReady() && target.IsValidTarget(Vars.R.Range))
            {
                foreach (var hero in
                    ObjectManager.Get<AIHeroClient>().Where(hero => hero.LSIsValidTarget(Vars.R.Range)))
                {
                    if (ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.R) > hero.Health)
                    {
                        Vars.R.CastOnUnit(target);
                    }

                    else if (ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.R) < hero.Health)
                    {
                        foreach (var buff in hero.Buffs.Where(buff => buff.Name == "dariushemo"))
                        {
                            if (ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.R, 1) * (1 + buff.Count / 5) - 1
                                > target.Health)
                            {
                                Vars.R.CastOnUnit(target);
                            }
                        }
                    }
                }
            }
        }
    }
}