#region

using System;
using System.Linq;
using Infected_Twitch.Core;
using Infected_Twitch.Menus;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;

#endregion

namespace Infected_Twitch.Event
{
    internal class Modes : Core.Core
    {
        public static void Update(EventArgs args)
        {
            AutoE();


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Lane();
                Jungle();
            }
               
        }

        private static void AutoE()
        {
           
            if (!Spells.E.IsReady()) return;

            if (MenuConfig.KillstealE)
            {
                if(Target == null || Target.IsDead || Target.IsInvulnerable || !Target.LSIsValidTarget(Spells.E.Range)) return;
                if (Dmg.Executable(Target))
                {
                    Spells.E.Cast();
                }
            }
           
            if (MenuConfig.StealEpic)
            {
                foreach (var m in ObjectManager.Get<Obj_AI_Base>().Where(x => Dragons.Contains(x.CharData.BaseSkinName) && !x.IsDead))
                {
                    if (m.Health < Player.LSGetSpellDamage(m, SpellSlot.E))
                    {
                        Spells.E.Cast();
                    }
                }
            }

            if(!MenuConfig.StealRed) return;

            var mob = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && !GameObjects.JungleSmall.Contains(m) && m.LSIsValidTarget(Spells.E.Range)).ToList();

            foreach (var m in mob)
            {
                if (m.CharData.BaseSkinName.Contains("SRU_Red"))
                {
                    if (m.Health < Player.LSGetSpellDamage(m, SpellSlot.E))
                    {
                        Spells.E.Cast();
                    }
                }
            }
        }

        private static void Combo()
        {
           if(!MenuConfig.ComboW) return;

            if(Target == null || Target.IsInvulnerable || !Target.LSIsValidTarget(Spells.W.Range)) return;

            if (MenuConfig.UseYoumuu && Target.LSIsValidTarget(Spells.W.Range))
            {
                Usables.CastYomu();
            }

            if (Target.HealthPercent <= 70)
            {
                Usables.Botrk();
            }
             

             if(Target.Health < Player.GetAutoAttackDamage(Target) * 2 && Target.Distance(Player) < Player.AttackRange) return;

               if (!Spells.W.IsReady()) return;

                if (!(Player.ManaPercent >= 7.5)) return;

            Spells.W.Cast(Target.Position);
        }

        private static void Harass()
        {
            if(Target == null || Target.IsInvulnerable || !Target.LSIsValidTarget()) return;

            if (Dmg.Stacks(Target) >= MenuConfig.HarassE && Target.Distance(Player) >= Player.AttackRange + 50)
            {
                Spells.E.Cast();
            }

            if (!MenuConfig.HarassW) return;

            Spells.W.Cast(Target.Position);
        }

        private static void Lane()
        {
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, ObjectManager.Player.Position, Spells.W.Range).Where(m => m.LSIsValidTarget());
            var position = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions, Spells.W.Width, (int)Spells.W.Range);
            if (!MenuConfig.LaneW) return;
            if(!Spells.W.IsReady()) return;

            if (position.HitNumber >= 2)
            {
                Spells.W.Cast(position.CastPosition);
            }
                
        }

        private static void Jungle()
        {
            if(Player.Level == 1) return;
            var mob = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && !GameObjects.JungleSmall.Contains(m) && m.LSIsValidTarget(Spells.E.Range)).ToList();
            var position = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(mob, Spells.W.Width, (int)Spells.W.Range);

            if (MenuConfig.JungleW && Player.ManaPercent >= 20)
            {
                if (mob.Count == 0) return;

                if (position.HitNumber >= 2)
                {
                    Spells.W.Cast(position.CastPosition);
                }
            }
           
            if(!MenuConfig.JungleE) return;

            foreach (var m in mob)
            {
                if (Dmg.Executable(m))
                {
                    Spells.E.Cast();
                }
            }
        }
    }
}
