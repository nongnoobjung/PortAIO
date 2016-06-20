#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Policy;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Common;
using Marksman.Utils;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
#endregion

namespace Marksman.Champions
{
    using System.Linq;

    using Utils = LeagueSharp.Common.Utils;

    internal class Caitlyn : Champion
    {
        public static LeagueSharp.Common.Spell R;

        public LeagueSharp.Common.Spell E;

        public static LeagueSharp.Common.Spell Q;

        public bool ShowUlt;

        public string UltTarget;

        public static LeagueSharp.Common.Spell W;

        private bool canCastR = true;

        private static int LastCastWTick = 0;

        // private static bool headshotReady = ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "CaitlynHeadshotReady");

        private string[] dangerousEnemies = new[]
        {
            "Alistar", "Garen", "Zed", "Fizz", "Rengar", "JarvanIV", "Irelia", "Amumu", "DrMundo", "Ryze", "Fiora", "KhaZix", "LeeSin", "Riven",
            "Lissandra", "Vayne", "Lucian", "Zyra"
        };

        public Caitlyn()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1240);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 820);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 800);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 2000);

            Q.SetSkillshot(0.50f, 50f, 2000f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnEndScene += DrawingOnOnEndScene;

            Obj_AI_Base.OnBuffGain += (sender, args) =>
            {
                if (W.IsReady())
                {
                    BuffInstance aBuff =
                        (from fBuffs in
                             sender.Buffs.Where(
                                 s =>
                                 sender.Team != ObjectManager.Player.Team
                                 && sender.LSDistance(ObjectManager.Player.Position) < W.Range)
                         from b in new[]
                                       {
                                               "teleport", /* Teleport */ "pantheon_grandskyfall_jump", /* Pantheon */ 
                                               "crowstorm", /* FiddleScitck */
                                               "zhonya", "katarinar", /* Katarita */
                                               "MissFortuneBulletTime", /* MissFortune */
                                               "gate", /* Twisted Fate */
                                               "chronorevive" /* Zilean */
                                       }
                         where args.Buff.Name.ToLower().Contains(b)
                         select fBuffs).FirstOrDefault();

                    if (aBuff != null)
                    {
                        CastW(sender.Position);
                        //W.Cast(sender.Position);
                    }
                }
            };

            Marksman.Utils.Utils.PrintMessage("Caitlyn loaded.");
        }

        public void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Program.misc["Misc.AntiGapCloser"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (E.IsReady() && gapcloser.Sender.LSIsValidTarget(E.Range))
            {
                E.Cast(gapcloser.Sender.Position);
            }
        }

        public override void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            //if (args.Slot == SpellSlot.W && LastCastWTick + 2000 > Utils.TickCount)
            //{
            //    args.Process = false;
            //}
            //else
            //{
            //    args.Process = true;
            //}

            //if (args.Slot == SpellSlot.Q)
            //{
            //    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && GetValue<bool>("UseQC"))
            //    {
            //        var t = TargetSelector.GetTarget(Q.Range - 20, DamageType.Physical);
            //        if (!t.LSIsValidTarget())
            //        {
            //            args.Process = false;
            //        }
            //        else
            //        {
            //            args.Process = true;
            //            //CastQ(t);
            //        }
            //    }
            //}
        }

        public override void Drawing_OnDraw(EventArgs args)
        {

            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (t.LSIsValidTarget())
            {
                Render.Circle.DrawCircle(t.Position, 105f, Color.GreenYellow);

                var wcCenter = ObjectManager.Player.Position.LSExtend(t.Position,
                    ObjectManager.Player.LSDistance(t.Position) / 2);

                Vector2 wcLeft = ObjectManager.Player.Position.LSTo2D() +
                                 Vector2.Normalize(t.Position.LSTo2D() - ObjectManager.Player.Position.LSTo2D())
                                     .Rotated(ObjectManager.Player.LSDistance(t.Position) < 300
                                         ? 45
                                         : 37 * (float)Math.PI / 180) * ObjectManager.Player.LSDistance(t.Position) / 2;

                Vector2 wcRight = ObjectManager.Player.Position.LSTo2D() +
                                  Vector2.Normalize(t.Position.LSTo2D() - ObjectManager.Player.Position.LSTo2D())
                                      .Rotated(ObjectManager.Player.LSDistance(t.Position) < 300
                                          ? -45
                                          : -37 * (float)Math.PI / 180) * ObjectManager.Player.LSDistance(t.Position) / 2;

                Render.Circle.DrawCircle(wcCenter, 50f, Color.Red);
                Render.Circle.DrawCircle(wcLeft.To3D(), 50f, Color.Green);
                Render.Circle.DrawCircle(wcRight.To3D(), 50f, Color.Yellow);
            }
            //var bx = HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range * 3));
            //foreach (var n in bx)
            //{
            //    if (n.LSIsValidTarget(800) && ObjectManager.Player.LSDistance(n) < 450)
            //    {
            //        Vector3[] x = new[] { ObjectManager.Player.Position, n.Position };
            //        Vector2 aX =
            //            Drawing.WorldToScreen(new Vector3(CommonGeometry.CenterOfVectors(x).X,
            //                CommonGeometry.CenterOfVectors(x).Y, CommonGeometry.CenterOfVectors(x).Z));

            //        Render.Circle.DrawCircle(CommonGeometry.CenterOfVectors(x), 85f, Color.White );
            //        Drawing.DrawText(aX.X - 15, aX.Y - 15, Color.GreenYellow, n.ChampionName);

            //    }
            //}

            //var enemies = HeroManager.Enemies.Where(e => e.LSIsValidTarget(1500));
            //var objAiHeroes = enemies as AIHeroClient[] ?? enemies.ToArray();
            //IEnumerable<AIHeroClient> nResult =
            //    (from e in objAiHeroes join d in dangerousEnemies on e.ChampionName equals d select e)
            //        .Distinct();

            //foreach (var n in nResult)
            //{
            //    var x = E.GetPrediction(n).CollisionObjects.Count;
            //    Render.Circle.DrawCircle(n.Position, (Orbwalking.GetRealAutoAttackRange(null) + 65) - 300, Color.GreenYellow);
            //}

            var nResult = HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range - 200));
            foreach (var n in nResult.Where(n => n.LSIsFacing(ObjectManager.Player)))
            {
                if (n.LSIsValidTarget())
                {
                    Render.Circle.DrawCircle(n.Position, E.Range - 200, Color.GreenYellow, 1);
                }
            }

            LeagueSharp.Common.Spell[] spellList = { Q, W, E, R };
            foreach (var spell in spellList)
            {
                var menuItem = Program.marksmanDrawings["Draw" + spell.Slot].Cast<CheckBox>().CurrentValue;
                if (menuItem)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.FromArgb(100, 255, 255, 255));
                }
            }
        }

        static void CastQ(Obj_AI_Base t)
        {
            if (Q.CanCast(t))
            {
                var qPrediction = Q.GetPrediction(t);
                var hithere = qPrediction.CastPosition.LSExtend(ObjectManager.Player.Position, -100);

                if (qPrediction.Hitchance >= Q.GetHitchance())
                {
                    Q.Cast(hithere);
                }
            }
        }

        static void CastW(Vector3 pos, bool delayControl = true)
        {
            if (!W.IsReady())
            {
                return;
            }

            //if (headshotReady)
            //{
            //    return;
            //}

            if (delayControl && LastCastWTick + 2000 > Utils.TickCount)
            {
                return;
            }

            W.Cast(pos);
        }

        static void CastW2(Obj_AI_Base t)
        {
            if (t.LSIsValidTarget(W.Range))
            {
                BuffType[] buffList =
                {
                    BuffType.Fear,
                    BuffType.Taunt,
                    BuffType.Stun,
                    BuffType.Slow,
                    BuffType.Snare
                };

                foreach (var b in buffList.Where(t.HasBuffOfType))
                {
                    CastW(t.Position);
                }
            }
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
            {
                return;
            }

            var x = 0;
            foreach (var b in ObjectManager.Player.Buffs.Where(buff => buff.DisplayName == "CaitlynHeadshotCount"))
            {
                x = b.Count;
            }

            for (int i = 1; i < 7; i++)
            {
                CommonGeometry.DrawBox(new Vector2(ObjectManager.Player.HPBarPosition.X + 23 + (i * 17), ObjectManager.Player.HPBarPosition.Y + 25), 15, 4, Color.Transparent, 1, Color.Black);
            }
            var headshotReady = ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "CaitlynHeadshotReady");
            for (int i = 1; i < (headshotReady ? 7 : x + 1); i++)
            {
                CommonGeometry.DrawBox(new Vector2(ObjectManager.Player.HPBarPosition.X + 24 + (i * 17), ObjectManager.Player.HPBarPosition.Y + 26), 13, 3, headshotReady ? Color.Red : Color.LightGreen, 0, Color.Black);
            }

            var rCircle2 = Program.marksmanDrawings["Draw.UltiMiniMap"].Cast<CheckBox>().CurrentValue;
            if (rCircle2)
            {
#pragma warning disable 618
                LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.FromArgb(255, 255, 255, 255), 1, 23, true);
#pragma warning restore 618
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {
            R.Range = 500 * (R.Level == 0 ? 1 : R.Level) + 1500;

            AIHeroClient t;

            if (W.IsReady() && (Program.misc["AutoWI"].Cast<ComboBox>().CurrentValue == 1 || (Program.misc["AutoWI"].Cast<ComboBox>().CurrentValue == 2 && ComboActive)))
            {
                t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (t.LSIsValidTarget(W.Range))
                {
                    if (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) ||
                        t.HasBuffOfType(BuffType.Taunt) || t.HasBuffOfType(BuffType.Knockup) ||
                        t.HasBuff("zhonyasringshield") || t.HasBuff("Recall"))
                    {
                        CastW(t.Position);
                    }

                    if (t.HasBuffOfType(BuffType.Slow))
                    {
                        var hit = t.LSIsFacing(ObjectManager.Player)
                            ? t.Position.LSExtend(ObjectManager.Player.Position, +140)
                            : t.Position.LSExtend(ObjectManager.Player.Position, -140);
                        CastW(hit);
                    }
                }
            }

            if (Q.IsReady() && (Program.misc["AutoQI"].Cast<ComboBox>().CurrentValue == 1 || (Program.misc["AutoQI"].Cast<ComboBox>().CurrentValue == 2 && ComboActive)))
            {
                t = TargetSelector.GetTarget(Q.Range - 30, DamageType.Physical);
                if (t.LSIsValidTarget(Q.Range)
                    && (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) || t.HasBuffOfType(BuffType.Taunt) || (t.Health <= ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q) && !Orbwalking.InAutoAttackRange(t))))
                {
                    CastQ(t);
                }
            }

            if (Program.combo["UseQMC"].Cast<KeyBind>().CurrentValue)
            {
                t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                CastQ(t);
            }

            //if (GetValue<KeyBind>("UseEMC").Active)
            //{
            //    t = TargetSelector.GetTarget(E.Range - 50, DamageType.Physical);
            //    E.Cast(t);
            //}

            if (Program.combo["UseRMC"].Cast<KeyBind>().CurrentValue && R.IsReady())
            {
                foreach (var e in HeroManager.Enemies.Where(e => e.LSIsValidTarget(R.Range)).OrderBy(e => e.Health))
                {
                    R.CastOnUnit(e);
                }
            }

            if (Program.misc["UseEQC"].Cast<KeyBind>().CurrentValue && E.IsReady() && Q.IsReady())
            {
                t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (t.LSIsValidTarget(E.Range)
                    && t.Health
                    < ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.Q)
                    + ObjectManager.Player.LSGetSpellDamage(t, SpellSlot.E) + 20 && E.CanCast(t))
                {
                    E.Cast(t);
                    CastQ(t);
                }
            }

            if ((!ComboActive && !HarassActive) || !Orbwalker.CanMove)
            {
                return;
            }

            //var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));
            var useW = Program.combo["UseWC"].Cast<CheckBox>().CurrentValue;
            var useE = Program.combo["UseEC"].Cast<CheckBox>().CurrentValue;
            var useR = Program.combo["UseRC"].Cast<CheckBox>().CurrentValue;

            //if (Q.IsReady() && useQ)
            //{
            //    t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            //    if (t != null)
            //    {
            //        CastQ(t);
            //    }
            //}

            if (useE && E.IsReady())
            {
                //var enemies = HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range));
                //var objAiHeroes = enemies as AIHeroClient[] ?? enemies.ToArray();
                //IEnumerable<AIHeroClient> nResult =
                //    (from e in objAiHeroes join d in dangerousEnemies on e.ChampionName equals d select e)
                //        .Distinct();

                //foreach (var n in nResult.Where(n => n.LSIsFacing(ObjectManager.Player)))
                //{
                //    if (n.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65 - 300) && E.GetPrediction(n).CollisionObjects.Count == 0)
                //    {
                //        E.Cast(n.Position);
                //        if (W.IsReady())
                //            W.Cast(n.Position);
                //    }
                //}

                var nResult = HeroManager.Enemies.Where(e => e.LSIsValidTarget(E.Range));
                foreach (var n in nResult)
                {
                    if (n.LSIsValidTarget(n.LSIsFacing(ObjectManager.Player) ? E.Range - 200 : E.Range - 300) && E.GetPrediction(n).CollisionObjects.Count == 0)
                    {
                        E.Cast(n.Position);
                    }
                }
            }

            if (useW && W.IsReady())
            {
                var nResult = HeroManager.Enemies.Where(e => e.LSIsValidTarget(W.Range));
                foreach (var n in nResult)
                {
                    if (ObjectManager.Player.LSDistance(n) < 450 && n.LSIsFacing(ObjectManager.Player))
                    {
                        CastW(CommonGeometry.CenterOfVectors(new[] { ObjectManager.Player.Position, n.Position }));
                    }
                }
            }

            if (R.IsReady() && useR)
            {
                foreach (var e in HeroManager.Enemies.Where(e => e.LSIsValidTarget(R.Range) && e.Health <= R.GetDamage(e) && !Orbwalking.InAutoAttackRange(e) && canCastR))
                {
                    R.CastOnUnit(e);
                }
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            var t = target as AIHeroClient;
            if (t == null || (!ComboActive && !HarassActive)) return;

            //var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));
            //if (useQ) Q.Cast(t, false, true);

            base.Orbwalking_AfterAttack(target, args);
        }

        public override bool ComboMenu(Menu config)
        {
            config.Add("UseQC", new CheckBox("Q:"));
            config.Add("UseQMC", new KeyBind("Q: Semi-Manual", false, KeyBind.BindTypes.HoldActive, 'G'));
            config.Add("UseWC", new CheckBox("W:"));
            config.Add("UseEC", new CheckBox("E:"));
            config.Add("UseRC", new CheckBox("R:"));
            config.Add("UseRMC", new KeyBind("R: Semi-Manual", false, KeyBind.BindTypes.HoldActive, 'R'));


            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.Add("UseQH", new CheckBox("Use Q"));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.Add("DrawQ", new CheckBox(Marksman.Utils.Utils.Tab + "Q:"));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawW", new CheckBox(Marksman.Utils.Utils.Tab + "W:"));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            config.Add("DrawE", new CheckBox(Marksman.Utils.Utils.Tab + "E:"));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            config.Add("DrawR", new CheckBox(Marksman.Utils.Utils.Tab + "R:"));//.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            config.Add("Draw.UltiMiniMap", new CheckBox(Marksman.Utils.Utils.Tab + "Draw Ulti Minimap"));//.SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.Add("Misc.AntiGapCloser", new CheckBox("E Anti Gap Closer"));
            config.Add("UseEQC", new KeyBind("Use E-Q Combo", false, KeyBind.BindTypes.HoldActive, 'T'));
            config.Add("Dash", new KeyBind("Dash to Mouse", false, KeyBind.BindTypes.HoldActive, 'Z'));
            config.Add("AutoQI", new ComboBox("Auto Q (Stun/Snare/Taunt/Slow)", 2, "Off", "On: Everytime", "On: Combo Mode"));
            config.Add("AutoWI", new ComboBox("Auto W (Stun/Snare/Taunt)", 2, "Off", "On: Everytime", "On: Combo Mode"));

            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            return true;
        }

        public override bool JungleClearMenu(Menu config)
        {
            return true;
        }

        public override void ExecuteFlee()
        {
            if (E.IsReady())
            {
                var pos = Vector3.Zero;
                var enemy =
                    HeroManager.Enemies.FirstOrDefault(
                        e =>
                            e.LSIsValidTarget(E.Range +
                                            (ObjectManager.Player.MoveSpeed > e.MoveSpeed
                                                ? ObjectManager.Player.MoveSpeed - e.MoveSpeed
                                                : e.MoveSpeed - ObjectManager.Player.MoveSpeed)) && E.CanCast(e));

                pos = enemy?.Position ??
                      ObjectManager.Player.ServerPosition.LSTo2D().LSExtend(Game.CursorPos.LSTo2D(), -300).To3D();
                //E.Cast(pos);
            }

            base.PermaActive();
        }
    }
}