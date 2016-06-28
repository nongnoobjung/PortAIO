using LeagueSharp.Common;
using LeagueSharp;
using SPrediction;
using System;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;

namespace Nechrito_Gragas
{
    class Mode
    {
        private static AIHeroClient Player => ObjectManager.Player;

        public static Vector3 rpred(AIHeroClient Target)
        {
            var pos = Spells.R.GetVectorSPrediction(Target, -50).CastTargetPosition;

            if (Target != null && !pos.LSIsWall())
            {
                if (Target.LSIsFacing(Player))
                {
                    if (Target.IsMoving)
                    {
                        pos = pos.LSExtend(Player.Position.LSTo2D(), -90);
                    }
                    pos = pos.LSExtend(Player.Position.LSTo2D(), -100);
                }

                if (!Target.LSIsFacing(Player))
                {
                    if (Target.IsMoving)
                    {
                        pos = pos.LSExtend(Player.Position.LSTo2D(), -145);
                    }
                    pos = pos.LSExtend(Player.Position.LSTo2D(), -130);
                }
            }
            return pos.To3D2();
        }

        public static Vector3 qpred(AIHeroClient Target)
        {
            var pos = Spells.Q.GetVectorSPrediction(Target, 50).CastTargetPosition;

            pos = pos.LSExtend(Player.Position.LSTo2D(), +Spells.R.Range);

            if (Target != null && !pos.LSIsWall())
            {
                if (Target.LSIsFacing(Player))
                {
                    if (Target.IsMoving)
                    {
                        pos = pos.LSExtend(Player.Position.LSTo2D(), 90);
                    }
                    pos = pos.LSExtend(Player.Position.LSTo2D(), 100);
                }

                if (!Target.LSIsFacing(Player))
                {
                    if (Target.IsMoving)
                    {
                        pos = pos.LSExtend(Player.Position.LSTo2D(), 150);
                    }
                    pos = pos.LSExtend(Player.Position.LSTo2D(), 140);
                }
            }

            return pos.To3D2();
        }

        public static void ComboLogic()
        {
            var Target = TargetSelector.SelectedTarget;

            if (Target != null && !Target.IsZombie && MenuConfig.ComboR && Target.LSDistance(Player) <= 1050f)
            {
                if (Target.IsDashing()) return;

                if (Spells.Q.IsReady() && Spells.R.IsReady())
                {
                    if (Program.GragasQ == null)
                    {
                        Spells.Q.Cast(qpred(Target), true);
                    }

                    if (Spells.R.IsReady())
                    {
                        Spells.R.Cast(rpred(Target), true);
                    }

                    if (Program.GragasQ != null && Target.LSDistance(Program.GragasQ.Position) <= 250)
                    {
                        Spells.Q.Cast();

                        var pos = Spells.E.GetVectorSPrediction(Target, Spells.E.Range).CastTargetPosition;
                        Spells.E.Cast(pos);
                    }
                }
            }

            var target = TargetSelector.GetTarget(700f, DamageType.Magical);

            if (target != null && target.IsValidTarget() && !target.IsZombie)
            {

                if (Spells.Q.IsReady())
                {
                    if (!Spells.R.IsReady())
                    {

                        if (Program.GragasQ == null)
                        {
                            Spells.Q.Cast(target, true);
                        }
                        if (Program.GragasQ != null && target.LSDistance(Program.GragasQ.Position) <= 250)
                        {
                            Spells.Q.Cast();
                        }
                    }
                }

                // Smite
                if (Spells.Smite != SpellSlot.Unknown && Spells.R.IsReady() && Player.Spellbook.CanUseSpell(Spells.Smite) == SpellState.Ready && !target.IsZombie)
                {
                    Player.Spellbook.CastSpell(Spells.Smite, Target);
                }

                else if (Spells.W.IsReady() && !Spells.R.IsReady())
                {
                    Spells.W.Cast();
                }

                // E
                else if (Spells.E.IsReady() && !Spells.W.IsReady())
                {
                    var pos = Spells.E.GetVectorSPrediction(Target, Spells.E.Range).CastTargetPosition;

                    if (!Spells.E.CheckMinionCollision(pos))
                    {
                        Spells.E.Cast(pos);
                    }
                }
            }
        }

        public static void JungleLogic()
        {
            var mobs = MinionManager.GetMinions(Player.Position, Spells.W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (mobs.Count == 0 || mobs == null || Player.Spellbook.IsAutoAttacking)
                    return;

                foreach (var m in mobs)
                {
                    if (m.LSDistance(Player) <= 400f)
                    {
                        if (Spells.W.IsReady())
                        {
                            Spells.W.Cast();
                        }

                        if (Spells.E.IsReady())
                        {
                            Spells.E.Cast(m);
                        }

                        if (Spells.Q.IsReady())
                        {
                            Spells.Q.Cast(m);
                        }
                    }
                }
            }
        }
        public static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(Spells.R.Range - 50, DamageType.Magical);
            if (target != null && target.IsValidTarget() && !target.IsZombie)
            {
                if (Spells.E.IsReady() && MenuConfig.harassE)
                {
                    Spells.E.Cast(target);
                }

                if (Spells.Q.IsReady() && MenuConfig.harassQ)
                {
                    var pos = Spells.Q.GetSPrediction(target).CastPosition;
                    Spells.Q.Cast(pos);
                }

                if (Spells.W.IsReady())
                {
                    if (target.LSDistance(Player) <= Player.AttackRange)
                    {
                        Spells.W.Cast();
                    }
                }
            }
        }
    }
}