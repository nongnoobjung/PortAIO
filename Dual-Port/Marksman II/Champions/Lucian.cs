#region
using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Utils;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

#endregion

namespace Marksman.Champions
{
    internal class Lucian : Champion
    {
        public static LeagueSharp.Common.Spell Q, Q2;

        public static LeagueSharp.Common.Spell W;

        public static LeagueSharp.Common.Spell E;

        public static LeagueSharp.Common.Spell R;

        public static bool DoubleHit = false;

        private static int xAttackLeft;

        private static float xPassiveUsedTime;

        public Lucian()
        {
            Utils.Utils.PrintMessage("Lucian loaded.");

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 760);
            Q2 = new LeagueSharp.Common.Spell(SpellSlot.Q, 1100);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1000);

            Q.SetSkillshot(0.45f, 60f, 1100f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.30f, 80f, 1600f, true, SkillshotType.SkillshotLine);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 475);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1400);

            xPassiveUsedTime = Game.Time;

            Obj_AI_Base.OnProcessSpellCast += Game_OnProcessSpell;
        }

        public static Obj_AI_Base QMinion(AIHeroClient t)
        {
            var m = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All,
                MinionTeam.NotAlly, MinionOrderTypes.None);

            return (from vM
                        in m.Where(vM => vM.LSIsValidTarget(Q.Range))
                    let endPoint = vM.ServerPosition.LSTo2D().LSExtend(ObjectManager.Player.ServerPosition.LSTo2D(), -Q2.Range).To3D()
                    where
                        vM.LSDistance(t) <= t.LSDistance(ObjectManager.Player) &&
                        Intersection(ObjectManager.Player.ServerPosition.LSTo2D(), endPoint.LSTo2D(), t.ServerPosition.LSTo2D(), t.BoundingRadius + vM.BoundingRadius)
                    select vM).FirstOrDefault();
        }
        public static bool IsPositionSafeForE(AIHeroClient target, LeagueSharp.Common.Spell spell)
        {
            var predPos = spell.GetPrediction(target).UnitPosition.LSTo2D();
            var myPos = ObjectManager.Player.Position.LSTo2D();
            var newPos = (target.Position.LSTo2D() - myPos);
            newPos.Normalize();

            var checkPos = predPos + newPos * (spell.Range - Vector2.Distance(predPos, myPos));
            Obj_Turret closestTower = null;

            foreach (var tower in ObjectManager.Get<Obj_Turret>()
                .Where(tower => tower.IsValid && !tower.IsDead && Math.Abs(tower.Health) > float.Epsilon)
                .Where(tower => Vector3.Distance(tower.Position, ObjectManager.Player.Position) < 1450))
            {
                closestTower = tower;
            }

            if (closestTower == null)
                return true;

            if (Vector2.Distance(closestTower.Position.LSTo2D(), checkPos) <= 910)
                return false;

            return true;
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            return;
        }

        public static bool Intersection(Vector2 p1, Vector2 p2, Vector2 pC, float radius)
        {
            var p3 = new Vector2(pC.X + radius, pC.Y + radius);

            var m = ((p2.Y - p1.Y) / (p2.X - p1.X));
            var constant = (m * p1.X) - p1.Y;
            var b = -(2f * ((m * constant) + p3.X + (m * p3.Y)));
            var a = (1 + (m * m));
            var c = ((p3.X * p3.X) + (p3.Y * p3.Y) - (radius * radius) + (2f * constant * p3.Y) + (constant * constant));
            var d = ((b * b) - (4f * a * c));

            return d > 0;
        }

        public void Game_OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
        {
            if (!unit.IsMe) return;
            if (spell.SData.Name.Contains("summoner")) return;
            if (!Program.misc["Passive"].Cast<CheckBox>().CurrentValue) return;

            //if (spell.Slot == SpellSlot.E || spell.Slot == SpellSlot.W || spell.Slot == SpellSlot.E || spell.Slot == SpellSlot.R)
            if (spell.SData.Name.ToLower().Contains("lucianq") || spell.SData.Name.ToLower().Contains("lucianw") ||
                spell.SData.Name.ToLower().Contains("luciane") || spell.SData.Name.ToLower().Contains("lucianr"))
            {
                xAttackLeft = 1;
                xPassiveUsedTime = Game.Time;
            }

            if (spell.SData.Name.ToLower().Contains("lucianpassiveattack"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(500, () => { xAttackLeft -= 1; });
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                xAttackLeft = 0;
                return;
            }

            if (Game.Time > xPassiveUsedTime + 3 && xAttackLeft == 1)
            {
                xAttackLeft = 0;
            }

            if (Program.misc["Passive"].Cast<CheckBox>().CurrentValue && xAttackLeft > 0)
            {
                return;
            }
            
            AIHeroClient t;

            if (Q.IsReady() && Program.harass["UseQTH"].Cast<KeyBind>().CurrentValue && ToggleActive)
            {
                if (ObjectManager.Player.HasBuff("Recall"))
                    return;

                t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (t != null)
                    Q.CastOnUnit(t);
            }
            

            if (Q.IsReady() && Program.harass["UseQExtendedTH"].Cast<KeyBind>().CurrentValue && ToggleActive)
            {
                if (ObjectManager.Player.HasBuff("Recall"))
                    return;

                t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (t.LSIsValidTarget() && QMinion(t).LSIsValidTarget())
                {
                    if (ObjectManager.Player.LSDistance(t) > Q.Range)
                        Q.CastOnUnit(QMinion(t));
                }
            }


            if ((!ComboActive && !HarassActive))
            {
                return;
            }

            var useQExtended = Program.combo["UseQExtendedC"].Cast<ComboBox>().CurrentValue;
            if (useQExtended != 0)
            {
                switch (useQExtended)
                {
                    case 1:
                    {
                        t = TargetSelector.GetTarget(Q2.Range, DamageType.Physical);
                        var tx = QMinion(t);
                        if (tx.LSIsValidTarget())
                        {
                            if (!Orbwalking.InAutoAttackRange(t))
                                Q.CastOnUnit(tx);
                        }
                        break;
                    }

                    case 2:
                    {
                        var enemy = HeroManager.Enemies.Find(e => e.LSIsValidTarget(Q2.Range) && !e.IsZombie);
                        if (enemy != null)
                        {
                            var tx = QMinion(enemy);
                            if (tx.LSIsValidTarget())
                            {
                                Q.CastOnUnit(tx);
                            }
                        }
                        break;
                    }
                }
            }

            // Auto turn off Ghostblade Item if Ultimate active
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level > 0)
            {
                Program.MenuActivator["GHOSTBLADE"].Cast<CheckBox>().CurrentValue = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LucianR";
            }

            t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (!t.LSIsValidTarget())
            {
                Orbwalker.ForcedTarget = null;
                return;
            }

            var useQ = Program.combo["UseQC"].Cast<CheckBox>().CurrentValue;
            if (useQ && Q.IsReady())
            {
                if (t.LSIsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(t);
                    Orbwalker.ResetAutoAttack();
                }
            }

            var useW = Program.combo["UseWC"].Cast<CheckBox>().CurrentValue;
            if (useW && W.IsReady())
            {
                if (t.LSIsValidTarget(W.Range))
                {
                    W.Cast(t);
                    Orbwalker.ResetAutoAttack();
                }
            }

            var useE = Program.combo["UseEC"].Cast<ComboBox>().CurrentValue;
            if (useE != 0 && E.IsReady())
            {
                if (t.LSDistance(ObjectManager.Player.Position) > Orbwalking.GetRealAutoAttackRange(null) && t.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + E.Range - 50) && E.IsPositionSafe(t.Position.LSTo2D()))
                {
                    E.Cast(t.Position);
                    Orbwalker.ResetAutoAttack();
                }
                else if (Q.IsPositionSafe(Game.CursorPos.LSTo2D()))
                {
                    E.Cast(Game.CursorPos);
                    Orbwalker.ResetAutoAttack();
                }
                Orbwalker.ForcedTarget = t;
            }
        }

        public override void ExecuteLaneClear()
        {
            int laneQValue = Program.laneclear["Lane.UseQ"].Cast<ComboBox>().CurrentValue;
            if (laneQValue != 0)
            {
                var minion = Q.GetLineCollisionMinions(laneQValue);
                if (minion != null)
                {
                    Q.CastOnUnit(minion);
                }
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                minion = allMinions.FirstOrDefault(minionn => minionn.LSDistance(ObjectManager.Player.Position) <= Q.Range && HealthPrediction.LaneClearHealthPrediction(minionn, (int)Q.Delay * 2) > 0);
                if (minion != null)
                {
                    Q.CastOnUnit(minion);
                }
            }

            int laneWValue = Program.laneclear["Lane.UseW"].Cast<ComboBox>().CurrentValue;
            if (laneWValue != 0 && E.IsReady())
            {
                Vector2 minions = W.GetLineFarmMinions(laneWValue);
                if (minions != Vector2.Zero)
                {
                    W.Cast(minions);
                }
            }
        }

        public override void ExecuteJungleClear()
        {
            var jungleQValue = Program.jungleClear["Jungle.UseQ"].Cast<ComboBox>().CurrentValue;
            if (jungleQValue != 0 && Q.IsReady())
            {
                var bigMobsQ = Utils.Utils.GetMobs(Q.Range, jungleQValue == 2 ? Utils.Utils.MobTypes.BigBoys : Utils.Utils.MobTypes.All);
                if (bigMobsQ != null && bigMobsQ.Health > ObjectManager.Player.TotalAttackDamage * 2)
                {
                    Q.CastOnUnit(bigMobsQ);
                }
            }

            var jungleWValue = Program.jungleClear["Jungle.UseW"].Cast<ComboBox>().CurrentValue;
            if (jungleWValue != 0 && W.IsReady())
            {
                var bigMobsQ = Utils.Utils.GetMobs(W.Range, jungleWValue == 2 ? Utils.Utils.MobTypes.BigBoys : Utils.Utils.MobTypes.All);
                if (bigMobsQ != null && bigMobsQ.Health > ObjectManager.Player.TotalAttackDamage * 2)
                {
                    W.Cast(bigMobsQ);
                }
            }

            var jungleEValue = Program.jungleClear["Jungle.UseE"].Cast<ComboBox>().CurrentValue;
            if (jungleEValue != 0 && E.IsReady())
            {
                var jungleMobs =
                    Marksman.Utils.Utils.GetMobs(Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65,
                        Marksman.Utils.Utils.MobTypes.All);

                if (jungleMobs != null)
                {
                    switch (Program.jungleClear["Jungle.UseE"].Cast<ComboBox>().CurrentValue)
                    {
                        case 1:
                            {
                                if (!jungleMobs.BaseSkinName.ToLower().Contains("baron") ||
                                    !jungleMobs.BaseSkinName.ToLower().Contains("dragon"))
                                {
                                    if (jungleMobs.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
                                        E.Cast(
                                            jungleMobs.LSIsValidTarget(
                                                Orbwalking.GetRealAutoAttackRange(null) + 65)
                                                ? Game.CursorPos
                                                : jungleMobs.Position);
                                }
                                break;
                            }

                        case 2:
                            {
                                if (!jungleMobs.BaseSkinName.ToLower().Contains("baron") ||
                                    !jungleMobs.BaseSkinName.ToLower().Contains("dragon"))
                                {
                                    jungleMobs =
                                        Marksman.Utils.Utils.GetMobs(
                                            E.Range + Orbwalking.GetRealAutoAttackRange(null) + 65,
                                            Marksman.Utils.Utils.MobTypes.BigBoys);
                                    if (jungleMobs != null)
                                    {
                                        E.Cast(
                                            jungleMobs.LSIsValidTarget(
                                                Orbwalking.GetRealAutoAttackRange(null) + 65)
                                                ? Game.CursorPos
                                                : jungleMobs.Position);
                                    }
                                }
                                break;
                            }
                    }
                }
            }
        }

        private static float GetRTotalDamage(AIHeroClient t)
        {
            var baseAttackSpeed = 0.638;
            var wCdTime = 3;
            var passiveDamage = 0;

            var attackSpeed = (float)Math.Round(Math.Floor(1 / ObjectManager.Player.AttackDelay * 100) / 100, 2, MidpointRounding.ToEven);

            var RLevel = new[] { 7.5, 9, 10.5 };
            var shoots = 7.5 + RLevel[R.Level - 1];
            var shoots2 = shoots * attackSpeed;

            var aDmg = Math.Round(Math.Floor(ObjectManager.Player.LSGetAutoAttackDamage(t) * 100) / 100, 2, MidpointRounding.ToEven);
            aDmg = Math.Floor(aDmg);

            var totalAttackSpeedWithWActive = (float)Math.Round((attackSpeed + baseAttackSpeed / 100) * 100 / 100, 2, MidpointRounding.ToEven);

            var totalPossibleDamage = (float)Math.Round((totalAttackSpeedWithWActive * wCdTime * aDmg) * 100 / 100, 2, MidpointRounding.ToEven);

            return totalPossibleDamage + (float)passiveDamage;
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Q:"));
            config.Add("UseQExtendedC", new ComboBox("Q Extended:", 1, "Off", "Use for Selected Target", "Use for Any Target"));
            config.Add("UseWC", new CheckBox("W:"));
            config.Add("UseEC", new ComboBox("E:", 2, "Off", "On", "On: Protect AA Range"));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQTH", new KeyBind("Use Q (Toggle)", false, KeyBind.BindTypes.PressToggle, 'T'));
            config.Add("UseQExtendedTH", new KeyBind("Use Ext. Q (Toggle)", false, KeyBind.BindTypes.PressToggle, 'H'));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("Passive", new CheckBox("Check Passive"));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox("Q range"));//.SetValue(new Circle(true, Color.Gray)));
            config.Add("DrawQ2", new CheckBox("Ext. Q range"));//.SetValue(new Circle(true, Color.Gray)));
            config.Add("DrawW", new CheckBox("W range", false));//.SetValue(new Circle(false, Color.Gray)));
            config.Add("DrawE", new CheckBox("E range", false));//.SetValue(new Circle(false, Color.Gray)));
            config.Add("DrawR", new CheckBox("R range", false));//.SetValue(new Circle(false, Color.Chocolate)));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            string[] strQ = new string[5];
            strQ[0] = "Off";

            for (var i = 1; i < 5; i++)
            {
                strQ[i] = "Minion Count >= " + i;
            }

            config.Add("Lane.UseQ", new ComboBox("Q:", 3, strQ));
            config.Add("Lane.UseQ2", new ComboBox("Q Extended:", 1, "Off", "Out of AA Range"));

            string[] strW = new string[5];
            strW[0] = "Off";

            for (var i = 1; i < 5; i++)
            {
                strW[i] = "Minion Count >= " + i;
            }

            config.Add("Lane.UseW", new ComboBox("W:", 3, strW));

            config.Add("Lane.UseE", new ComboBox("E:", 1, "Off", "Under Ally Turrent Farm", "Out of AA Range", "Both"));


            string[] strR = new string[4];
            strR[0] = "Off";

            for (var i = 1; i < 4; i++)
            {
                strR[i] = "Minion Count >= Ulti Attack Count x " + i.ToString();
            }
            config.Add("Lane.UseR", new ComboBox("R:", 2, strR));

        
            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            config.Add("Jungle.UseQ", new ComboBox("Q:", 2, "Off", "On", "Just big Monsters"));
            config.Add("Jungle.UseW", new ComboBox("W:", 2, "Off", "On", "Just big Monsters"));
            config.Add("Jungle.UseE", new ComboBox("E:", 2, "Off", "On", "Just big Monsters"));

            return true;
        }

        private bool LucianHavePassiveBuff()
        {
            return ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "LucianPassive");
        }

        public override void PermaActive()
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            var enemy = HeroManager.Enemies.Find(e => e.LSIsValidTarget(E.Range + (Q.IsReady() ? Q.Range : Orbwalking.GetRealAutoAttackRange(null) + 65)) && !e.IsZombie);
            if (enemy != null)
            {
                if (enemy.Health < ObjectManager.Player.TotalAttackDamage*2 && !LucianHavePassiveBuff() && enemy.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65) && !Q.IsReady())
                {
                    if (W.IsReady() && Program.combo["UseWC"].Cast<CheckBox>().CurrentValue)
                    {
                        W.Cast(enemy.Position);
                    }
                    else if (E.IsReady() && Program.combo["UseEC"].Cast<ComboBox>().CurrentValue != 0)
                    {
                        E.Cast(enemy.Position);
                    }
                }

                var xPossibleComboDamage = 0f;
                xPossibleComboDamage += Q.IsReady() ? Q.GetDamage(enemy) + ObjectManager.Player.TotalAttackDamage * 2 : 0;
                xPossibleComboDamage += E.IsReady() ? ObjectManager.Player.TotalAttackDamage * 2 : 0;

                if (enemy.Health < xPossibleComboDamage)
                {
//                    if (enemy.LSDistance(ObjectManager.Player) > Orbwalking.GetRealAutoAttackRange(null) + 65))
                }

                if (E.IsReady() && Q.IsReady() && Program.combo["UseEC"].Cast<ComboBox>().CurrentValue != 0)
                {
                    E.Cast(enemy.Position);
                }
            }
        }
    }
}
