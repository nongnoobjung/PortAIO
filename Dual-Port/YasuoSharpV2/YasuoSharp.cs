using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace YasuoSharpV2
{
    internal class YasuoSharp
    {


        public const string CharName = "Yasuo";


        public static Menu comboMenu, smartR, flee, lasthit, laneclear, harass, drawings, extra, smartW, debug, Config;

        public static string lastSpell = "";

        public static int afterDash = 0;

        public static bool canSave = true;
        public static bool canExport = true;
        public static bool canDelete = true;

        public static bool wasStream = false;


        public static List<Skillshot> DetectedSkillshots = new List<Skillshot>();

        public YasuoSharp()
        {

            // map = new Map();
            /* CallBAcks */
            CustomEvents.Game.OnGameLoad += onLoad;

        }

        private static void onLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != CharName)
                return;

            Yasuo.setSkillShots();
            Yasuo.setDashes();
            Yasuo.point1 = Yasuo.Player.Position;
            Chat.Print("YasuoSharpV2 by DeTuKs");

            Console.WriteLine("YasuoSharpV2 by DeTuKs");

            try
            {

                Config = MainMenu.AddMenu("YasuoSharp", "YasuoASHARP");

                //Combo
                comboMenu = Config.AddSubMenu("Combo Sharp", "combo");
                comboMenu.Add("comboItems", new CheckBox("Use Items"));

                //SmartR
                smartR = Config.AddSubMenu("Smart R");
                smartR.Add("smartR", new CheckBox("Smart R"));
                smartR.Add("useRHit", new Slider("Use R if hit", 3, 1, 5));
                smartR.Add("useRHitTime", new CheckBox("Use R when they land"));
                smartR.Add("useEWall", new CheckBox("use E to safe"));

                //Flee away
                flee = Config.AddSubMenu("Flee");
                //flee.Add("flee", new KeyBind("E away", false, KeyBind.BindTypes.HoldActive, 'Z'));
                flee.Add("fleeStack", new CheckBox("Stack Q while flee"));

                //LastHit
                lasthit = Config.AddSubMenu("LastHit Sharp", "lHit");
                lasthit.Add("useQlh", new CheckBox("Use Q"));
                lasthit.Add("useElh", new CheckBox("Use E"));

                //LaneClear
                laneclear = Config.AddSubMenu("LaneClear Sharp", "lClear");
                laneclear.Add("useQlc", new CheckBox("Use Q"));
                laneclear.Add("useEmpQHit", new Slider("Emp Q Min hit", 3, 1, 6));
                laneclear.Add("useElc", new CheckBox("Use E"));

                //Harass
                harass = Config.AddSubMenu("Harass Sharp", "harass");
                harass.Add("harassTower", new CheckBox("Harass under tower", false));
                harass.Add("harassOn", new CheckBox("Harass enemies"));
                harass.Add("harQ3Only", new CheckBox("Use only Q3", false));

                //Drawings
                drawings = Config.AddSubMenu("Drawing Sharp", "drawing");
                drawings.Add("disDraw", new CheckBox("Dissabel drawing", false));
                drawings.Add("drawQ", new CheckBox("Draw Q range"));
                drawings.Add("drawE", new CheckBox("Draw E range"));
                drawings.Add("drawR", new CheckBox("Draw R range"));
                drawings.Add("drawWJ", new CheckBox("Draw Wall Jumps"));

                //Extra
                extra = Config.AddSubMenu("Extra Sharp", "extra");
                extra.Add("djTur", new CheckBox("Dont Jump turrets"));
                extra.Add("autoLevel", new CheckBox("Auto Level"));
                extra.Add("levUpSeq", new ComboBox("Sequence : ", 0, "Q E W Q start", "Q E Q W start"));

                //SmartW
                smartW = Config.AddSubMenu("Wall Usage", "aShots");
                smartW.Add("smartW", new CheckBox("Smart WW"));
                smartW.Add("smartEDogue", new CheckBox("E use dogue"));
                smartW.Add("wwDanger", new CheckBox("WW only dangerous", false));
                smartW.Add("wwDmg", new Slider("WW if does proc HP", 0, 1, 100));
                getSkilshotMenu(smartW);
                TargetedSpellManager.setUp(smartW);

                //Debug
                debug = Config.AddSubMenu("Debug", "debug");
                debug.Add("WWLast", new KeyBind("Print last ww blocked", false, KeyBind.BindTypes.HoldActive, 'T'));
                debug.Add("saveDash", new KeyBind("saveDashd", false, KeyBind.BindTypes.HoldActive, 'O'));
                debug.Add("exportDash", new KeyBind("export dashes", false, KeyBind.BindTypes.HoldActive, 'P'));
                debug.Add("deleteDash", new KeyBind("deleteLastDash", false, KeyBind.BindTypes.HoldActive, 'I'));

                TargetSpellDetector.init();

                Drawing.OnDraw += onDraw;
                Game.OnUpdate += OnGameUpdate;

                GameObject.OnCreate += OnCreateObject;
                GameObject.OnDelete += OnDeleteObject;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
                Spellbook.OnStopCast += onStopCast;
                CustomEvents.Unit.OnLevelUp += OnLevelUp;

                Game.OnSendPacket += OnGameSendPacket;
                Game.OnProcessPacket += OnGameProcessPacket;

                SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
                SkillshotDetector.OnDeleteMissile += OnDeleteMissile;
            }
            catch
            {
                Chat.Print("Oops. Something went wrong with Yasuo - Sharpino");
            }

        }


        public static void getSkilshotMenu(Menu m)
        {
            m.AddSeparator();
            //Create the skillshots submenus.
            m.AddGroupLabel("Enemy Skillshots");

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.Team != ObjectManager.Player.Team)
                {
                    foreach (var spell in SpellDatabase.Spells)
                    {
                        if (spell.ChampionName == hero.ChampionName)
                        {
                            m.AddLabel(spell.MenuItemName);
                            m.Add("DangerLevel" + spell.MenuItemName, new Slider("Danger level", spell.DangerValue, 1, 5));
                            m.Add("IsDangerous" + spell.MenuItemName, new CheckBox("Is Dangerous", spell.IsDangerous));
                            m.Add("Draw" + spell.MenuItemName, new CheckBox("Draw"));
                            m.Add("Enabled" + spell.MenuItemName, new CheckBox("Enabled"));
                            m.AddSeparator();
                        }
                    }
                }
            }
        }

        public static bool skillShotIsDangerous(string Name)
        {
            if (smartW["IsDangerous" + Name] != null)
            {
                return smartW["IsDangerous" + Name].Cast<CheckBox>().CurrentValue;
            }
            return true;
        }

        public static bool EvadeSpellEnabled(string Name)
        {
            if (smartW["Enabled" + Name] != null)
            {
                return smartW["Enabled" + Name].Cast<CheckBox>().CurrentValue;
            }
            return true;
        }

        public static void updateSkillshots()
        {
            foreach (var ss in DetectedSkillshots)
            {
                ss.Game_OnGameUpdate();
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                Yasuo.Q.SetSkillshot(Yasuo.getNewQSpeed(), 50f, float.MaxValue, false, SkillshotType.SkillshotLine);

                if (Yasuo.startDash + 470000 / ((700 + Yasuo.Player.MoveSpeed)) < Environment.TickCount && Yasuo.isDashigPro)
                {
                    Yasuo.isDashigPro = false;
                }

                //updateSkillshots();
                //Remove the detected skillshots that have expired.
                DetectedSkillshots.RemoveAll(skillshot => !skillshot.IsActive());

                AIHeroClient target = TargetSelector.GetTarget((Yasuo.E.IsReady()) ? 1500 : 475, DamageType.Physical);
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Yasuo.doCombo(target);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    Yasuo.doLastHit(target);
                    Yasuo.useQSmart(target);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    Yasuo.doLastHit(target);
                    Yasuo.useQSmart(target);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    Yasuo.doLaneClear(target);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                {
                    Yasuo.fleeToMouse();
                    Yasuo.stackQ();
                }

                if (debug["saveDash"].Cast<KeyBind>().CurrentValue && canSave)
                {
                    Yasuo.saveLastDash();
                    canSave = false;
                }
                else
                {
                    canSave = true;
                }

                if (debug["deleteDash"].Cast<KeyBind>().CurrentValue && canDelete)
                {
                    if (Yasuo.dashes.Count > 0)
                        Yasuo.dashes.RemoveAt(Yasuo.dashes.Count - 1);
                    canDelete = false;
                }
                else
                {
                    canDelete = true;
                }
                if (debug["exportDash"].Cast<KeyBind>().CurrentValue && canExport)
                {
                    using (var file = new System.IO.StreamWriter(@"C:\YasuoDashes.txt"))
                    {

                        foreach (var dash in Yasuo.dashes)
                        {
                            string dashS = "dashes.Add(new YasDash(new Vector3(" +
                                           dash.from.X.ToString("0.00").Replace(',', '.') + "f," +
                                           dash.from.Y.ToString("0.00").Replace(',', '.') + "f," +
                                           dash.from.Z.ToString("0.00").Replace(',', '.') +
                                           "f),new Vector3(" + dash.to.X.ToString("0.00").Replace(',', '.') + "f," +
                                           dash.to.Y.ToString("0.00").Replace(',', '.') + "f," +
                                           dash.to.Z.ToString("0.00").Replace(',', '.') + "f)));";
                            //new YasDash(new Vector3(X,Y,Z),new Vector3(X,Y,Z))

                            file.WriteLine(dashS);
                        }
                        file.Close();
                    }

                    canExport = false;
                }
                else
                {
                    canExport = true;
                }

                if (debug["WWLast"].Cast<KeyBind>().CurrentValue)
                {
                    Console.WriteLine("Last WW skill blocked: " + lastSpell);
                    Chat.Print("Last WW skill blocked: " + lastSpell);
                }

                if (harass["harassOn"].Cast<CheckBox>().CurrentValue && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                {
                    if (target != null)
                        Yasuo.useQSmart(target, harass["harQ3Only"].Cast<CheckBox>().CurrentValue);
                }

                foreach (var mis in DetectedSkillshots)
                {
                    Yasuo.useWSmart(mis);

                    if (smartW["smartEDogue"].Cast<CheckBox>().CurrentValue && !Yasuo.isSafePoint(Yasuo.Player.Position.LSTo2D(), true).IsSafe)
                        Yasuo.useEtoSafe(mis);
                }

                if (smartR["smartR"].Cast<CheckBox>().CurrentValue && Yasuo.R.IsReady())
                    Yasuo.useRSmart();

                Yasuo.processTargetedSpells();



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void onDraw(EventArgs args)
        {
            if (drawings["disDraw"].Cast<CheckBox>().CurrentValue)
                return;



            Drawing.DrawText(100, 100, Color.Red, "targ Spells: " + TargetSpellDetector.ActiveTargeted.Count);

            foreach (Obj_AI_Base jun in MinionManager.GetMinions(Yasuo.Player.ServerPosition, 700, MinionTypes.All, MinionTeam.Neutral))
            {
                Drawing.DrawCircle(jun.Position, 70, Color.Green);
                Vector2 posAfterE = Yasuo.Player.ServerPosition.LSTo2D() + (Vector2.Normalize(jun.ServerPosition.LSTo2D() - Yasuo.Player.ServerPosition.LSTo2D()) * 475);
                // Vector2 posAfterE = Yasuo.Player.Position.LSTo2D().Extend(jun.Position.LSTo2D(), 475);//jun.ServerPosition.LSTo2D().Extend() + (Vector2.Normalize(Yasuo.Player.Position.LSTo2D() - jun.ServerPosition.LSTo2D()) * 475);
                Drawing.DrawCircle(posAfterE.To3D(), 50, Color.Violet);
                Vector3 posAfterDash = Yasuo.Player.GetPath(posAfterE.To3D()).Last();
                Drawing.DrawCircle(posAfterDash, 50, Color.DarkRed);

            }

            if (drawings["drawQ"].Cast<CheckBox>().CurrentValue)
                LeagueSharp.Common.Utility.DrawCircle(Yasuo.Player.Position, 475, (Yasuo.isDashigPro) ? Color.Red : Color.Blue, 10, 10);
            if (drawings["drawR"].Cast<CheckBox>().CurrentValue)
                LeagueSharp.Common.Utility.DrawCircle(Yasuo.Player.Position, 1200, Color.Blue);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee) && drawings["drawWJ"].Cast<CheckBox>().CurrentValue)
            {
                LeagueSharp.Common.Utility.DrawCircle(Game.CursorPos, 350, Color.Cyan);

                LeagueSharp.Common.Utility.DrawCircle(Yasuo.lastDash.from, 60, Color.BlueViolet);
                LeagueSharp.Common.Utility.DrawCircle(Yasuo.lastDash.to, 60, Color.BlueViolet);

                foreach (Yasuo.YasDash dash in Yasuo.dashes)
                {
                    if (dash.from.LSDistance(Game.CursorPos) < 1200)
                    {
                        var SA = Drawing.WorldToScreen(dash.from);
                        var SB = Drawing.WorldToScreen(dash.to);
                        Drawing.DrawLine(SA.X, SA.Y, SB.X, SB.Y, 3, Color.Green);
                    }
                }

            }


            /*   if ((int)NavMesh.GetCollisionFlags(Game.CursorPos) == 2 || (int)NavMesh.GetCollisionFlags(Game.CursorPos) == 64)
                Drawing.DrawCircle(Game.CursorPos, 70, Color.Green);
            if (map.isWall(Game.CursorPos.LSTo2D()))
                Drawing.DrawCircle(Game.CursorPos, 100, Color.Red);

            foreach (Polygon pol in map.poligs)
            {
                pol.Draw(Color.BlueViolet, 3);
            }

            foreach(Obj_AI_Base jun in MinionManager.GetMinions(Yasuo.Player.ServerPosition,700,MinionTypes.All,MinionTeam.Neutral))
            {
                Drawing.DrawCircle(jun.Position, 70, Color.Green);
                 SharpDX.Vector2 proj = map.getClosestPolygonProj(jun.ServerPosition.LSTo2D());
                 SharpDX.Vector2 posAfterE = jun.ServerPosition.LSTo2D() + (SharpDX.Vector2.Normalize(proj - jun.ServerPosition.LSTo2D() ) * 475);
                 Drawing.DrawCircle(posAfterE.To3D(), 50, Color.Violet);
            }

            foreach (MissileClient mis in skillShots)
            {
                Drawing.DrawCircle(mis.Position, 47, Color.Orange);
                Drawing.DrawCircle(mis.EndPosition, 100, Color.BlueViolet);
               Drawing.DrawCircle(mis.SpellCaster.Position, Yasuo.Player.BoundingRadius + mis.SData.LineWidth, Color.DarkSalmon);
                Drawing.DrawCircle(mis.StartPosition, 70, Color.Green);
            }*/

        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            //wall
            if (sender.IsValid<MissileClient>())
            {
                if (sender is MissileClient)
                {
                    MissileClient missle = (MissileClient)sender;
                    if (missle.SData.Name == "yasuowmovingwallmisl")
                    {
                        Yasuo.wall.setL(missle);
                    }

                    if (missle.SData.Name == "yasuowmovingwallmisr")
                    {
                        Yasuo.wall.setR(missle);
                    }
                }
            }

            if (sender.IsValid<MissileClient>() && sender.IsValid)
            {
                var s = sender as MissileClient;
                if (s != null)
                {
                    if ((s).Target.IsMe)
                    {
                        TargetSpellDetector.setParticle(s);
                    }
                }
            }
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            /* int i = 0;
             foreach (var lho in skillShots)
             {
                 if (lho.NetworkId == sender.NetworkId)
                 {
                     skillShots.RemoveAt(i);
                     return;
                 }
                 i++;
             }*/
        }


        private static void onStopCast(Obj_AI_Base obj, SpellbookStopCastEventArgs args)
        {
            if (obj.IsMe)
            {
                if (obj.IsValid && args.DestroyMissile && args.StopAnimation)
                {
                    Yasuo.isDashigPro = false;
                }
            }
        }

        public static void OnProcessSpell(Obj_AI_Base obj, GameObjectProcessSpellCastEventArgs arg)
        {
            if (obj.IsMe)
            {
                if (arg.SData.Name == "YasuoDashWrapper")//start dash
                {
                    Console.WriteLine("--- DAhs started---");
                    Yasuo.lastDash.from = Yasuo.Player.Position;
                    Yasuo.isDashigPro = true;
                    Yasuo.castFrom = Yasuo.Player.Position;
                    Yasuo.startDash = Environment.TickCount;
                }
            }
        }

        public static void OnLevelUp(Obj_AI_Base sender, LeagueSharp.Common.CustomEvents.Unit.OnLevelUpEventArgs args)
        {
            if (sender.NetworkId == Yasuo.Player.NetworkId)
            {
                if (!extra["autoLevel"].Cast<CheckBox>().CurrentValue)
                    return;
                if (extra["levUpSeq"].Cast<ComboBox>().CurrentValue == 0)
                    Yasuo.sBook.LevelUpSpell(Yasuo.levelUpSeq[args.NewLevel - 1].Slot);
                else if (extra["levUpSeq"].Cast<ComboBox>().CurrentValue == 1)
                    Yasuo.sBook.LevelUpSpell(Yasuo.levelUpSeq2[args.NewLevel - 1].Slot);
            }
        }



        private static void OnGameProcessPacket(GamePacketEventArgs args)
        {//28 16 176 ??184
            if (args.PacketData[0] == 41)//135no 100no 183no 34no 101 133 56yesss? 127 41yess
            {
                GamePacket gp = new GamePacket(args.PacketData);
                //Console.WriteLine(Encoding.UTF8.GetString(args.PacketData, 0, args.PacketData.Length));
                gp.Position = 1;
                if (gp.ReadInteger() == Yasuo.Player.NetworkId /*&&  Encoding.UTF8.GetString(args.PacketData, 0, args.PacketData.Length).Contains("Spell3")*/)
                {
                    Console.WriteLine("----");
                    Yasuo.lastDash.to = Yasuo.Player.Position;
                    Yasuo.isDashigPro = false;
                    Yasuo.time = Game.Time - Yasuo.startDash;
                }
                /* for (int i = 1; i < gp.Size() - 4; i++)
                 {
                     gp.Position = i;
                     if (gp.ReadInteger() == Yasuo.Player.NetworkId)
                     {
                         Console.WriteLine("Found: "+i);
                     }
                 }

                 Console.WriteLine("End dash");
                 Yasuo.Q.Cast(Yasuo.Player.Position);*/
            }

            /*if (args.PacketData[0] == 176) //135no 100no 183no 34no 101 133 56yesss? 127 41yess
            {
                GamePacket gp = new GamePacket(args.PacketData);
                //Console.WriteLine(Encoding.UTF8.GetString(args.PacketData, 0, args.PacketData.Length));
                gp.Position = 1;
                if (gp.ReadInteger() == Yasuo.Player.NetworkId)
                {
                    Console.WriteLine("--- DAhs started Packets---");
                    Yasuo.lastDash.from = Yasuo.Player.Position;
                    Yasuo.isDashigPro = true;
                    Yasuo.castFrom = Yasuo.Player.Position;
                    Yasuo.startDash = Game.Time;
                }
            }*/
        }

        private static void OnGameSendPacket(GamePacketEventArgs args)
        {
            /*if (args.PacketData[0] == 154) //135no 100no 183no 34no 101 133 56yesss? 127 41yess
            {
                var spell = Packet.C2S.Cast.Decoded(args.PacketData);
                if (spell.Slot == Yasuo.E.Slot)
                {
                    Console.WriteLine("--- DAhs started Packets---");
                    Yasuo.lastDash.from = Yasuo.Player.Position;
                    Yasuo.isDashigPro = true;
                    Yasuo.castFrom = Yasuo.Player.Position;
                    Yasuo.startDash = Game.Time;
                }
            }*/
        }



        private static void OnDeleteMissile(Skillshot skillshot, MissileClient missile)
        {
            if (skillshot.SpellData.SpellName == "VelkozQ")
            {
                var spellData = SpellDatabase.GetByName("VelkozQSplit");
                var direction = skillshot.Direction.LSPerpendicular();
                if (DetectedSkillshots.Count(s => s.SpellData.SpellName == "VelkozQSplit") == 0)
                {
                    for (var i = -1; i <= 1; i = i + 2)
                    {
                        var skillshotToAdd = new Skillshot(
                            DetectionType.ProcessSpell, spellData, Environment.TickCount, missile.Position.LSTo2D(),
                            missile.Position.LSTo2D() + i * direction * spellData.Range, skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                    }
                }
            }
        }

        private static void OnDetectSkillshot(Skillshot skillshot)
        {
            var alreadyAdded = false;

            foreach (var item in DetectedSkillshots)
            {
                if (item.SpellData.SpellName == skillshot.SpellData.SpellName &&
                    (item.Unit.NetworkId == skillshot.Unit.NetworkId &&
                     (skillshot.Direction).LSAngleBetween(item.Direction) < 5 &&
                     (skillshot.Start.LSDistance(item.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0)))
                {
                    alreadyAdded = true;
                }
            }

            //Check if the skillshot is from an ally.
            if (skillshot.Unit.Team == ObjectManager.Player.Team)
            {
                return;
            }

            //Check if the skillshot is too far away.
            if (skillshot.Start.LSDistance(ObjectManager.Player.ServerPosition.LSTo2D()) >
                (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000) * 1.5)
            {
                return;
            }

            //Add the skillshot to the detected skillshot list.
            if (!alreadyAdded)
            {
                //Multiple skillshots like twisted fate Q.
                if (skillshot.DetectionType == DetectionType.ProcessSpell)
                {
                    if (skillshot.SpellData.MultipleNumber != -1)
                    {
                        var originalDirection = skillshot.Direction;

                        for (var i = -(skillshot.SpellData.MultipleNumber - 1) / 2;
                            i <= (skillshot.SpellData.MultipleNumber - 1) / 2;
                            i++)
                        {
                            var end = skillshot.Start +
                                      skillshot.SpellData.Range *
                                      originalDirection.LSRotated(skillshot.SpellData.MultipleAngle * i);
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                                skillshot.Unit);

                            DetectedSkillshots.Add(skillshotToAdd);
                        }
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "UFSlash")
                    {
                        skillshot.SpellData.MissileSpeed = 1600 + (int)skillshot.Unit.MoveSpeed;
                    }

                    if (skillshot.SpellData.Invert)
                    {
                        var newDirection = -(skillshot.End - skillshot.Start).LSNormalized();
                        var end = skillshot.Start + newDirection * skillshot.Start.LSDistance(skillshot.End);
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.Centered)
                    {
                        var start = skillshot.Start - skillshot.Direction * skillshot.SpellData.Range;
                        var end = skillshot.Start + skillshot.Direction * skillshot.SpellData.Range;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "SyndraE" || skillshot.SpellData.SpellName == "syndrae5")
                    {
                        var angle = 60;
                        var edge1 =
                            (skillshot.End - skillshot.Unit.ServerPosition.LSTo2D()).LSRotated(
                                -angle / 2 * (float)Math.PI / 180);
                        var edge2 = edge1.LSRotated(angle * (float)Math.PI / 180);

                        foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                        {
                            var v = minion.ServerPosition.LSTo2D() - skillshot.Unit.ServerPosition.LSTo2D();
                            if (minion.Name == "Seed" && edge1.LSCrossProduct(v) > 0 && v.LSCrossProduct(edge2) > 0 &&
                                minion.LSDistance(skillshot.Unit) < 800 &&
                                (minion.Team != ObjectManager.Player.Team))
                            {
                                var start = minion.ServerPosition.LSTo2D();
                                var end = skillshot.Unit.ServerPosition.LSTo2D()
                                    .LSExtend(
                                        minion.ServerPosition.LSTo2D(),
                                        skillshot.Unit.LSDistance(minion) > 200 ? 1300 : 1000);

                                var skillshotToAdd = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                                    skillshot.Unit);
                                DetectedSkillshots.Add(skillshotToAdd);
                            }
                        }
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "AlZaharCalloftheVoid")
                    {
                        var start = skillshot.End - skillshot.Direction.LSPerpendicular() * 400;
                        var end = skillshot.End + skillshot.Direction.LSPerpendicular() * 400;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsQ")
                    {
                        var d1 = skillshot.Start.LSDistance(skillshot.End);
                        var d2 = d1 * 0.4f;
                        var d3 = d2 * 0.69f;


                        var bounce1SpellData = SpellDatabase.GetByName("ZiggsQBounce1");
                        var bounce2SpellData = SpellDatabase.GetByName("ZiggsQBounce2");

                        var bounce1Pos = skillshot.End + skillshot.Direction * d2;
                        var bounce2Pos = bounce1Pos + skillshot.Direction * d3;

                        bounce1SpellData.Delay =
                            (int)(skillshot.SpellData.Delay + d1 * 1000f / skillshot.SpellData.MissileSpeed + 500);
                        bounce2SpellData.Delay =
                            (int)(bounce1SpellData.Delay + d2 * 1000f / bounce1SpellData.MissileSpeed + 500);

                        var bounce1 = new Skillshot(
                            skillshot.DetectionType, bounce1SpellData, skillshot.StartTick, skillshot.End, bounce1Pos,
                            skillshot.Unit);
                        var bounce2 = new Skillshot(
                            skillshot.DetectionType, bounce2SpellData, skillshot.StartTick, bounce1Pos, bounce2Pos,
                            skillshot.Unit);

                        DetectedSkillshots.Add(bounce1);
                        DetectedSkillshots.Add(bounce2);
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsR")
                    {
                        skillshot.SpellData.Delay =
                            (int)(1500 + 1500 * skillshot.End.LSDistance(skillshot.Start) / skillshot.SpellData.Range);
                    }

                    if (skillshot.SpellData.SpellName == "JarvanIVDragonStrike")
                    {
                        var endPos = new Vector2();

                        foreach (var s in DetectedSkillshots)
                        {
                            if (s.Unit.NetworkId == skillshot.Unit.NetworkId && s.SpellData.Slot == SpellSlot.E)
                            {
                                endPos = s.End;
                            }
                        }

                        foreach (var m in ObjectManager.Get<Obj_AI_Minion>())
                        {
                            if (m.BaseSkinName == "jarvanivstandard" && m.Team == skillshot.Unit.Team &&
                                skillshot.IsDanger(m.Position.LSTo2D()))
                            {
                                endPos = m.Position.LSTo2D();
                            }
                        }

                        if (!endPos.IsValid())
                        {
                            return;
                        }

                        skillshot.End = endPos + 200 * (endPos - skillshot.Start).LSNormalized();
                        skillshot.Direction = (skillshot.End - skillshot.Start).LSNormalized();
                    }
                }

                if (skillshot.SpellData.SpellName == "OriannasQ")
                {
                    var endCSpellData = SpellDatabase.GetByName("OriannaQend");

                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, endCSpellData, skillshot.StartTick, skillshot.Start, skillshot.End,
                        skillshot.Unit);

                    DetectedSkillshots.Add(skillshotToAdd);
                }


                //Dont allow fow detection.
                if (skillshot.SpellData.DisableFowDetection && skillshot.DetectionType == DetectionType.RecvPacket)
                {
                    return;
                }
#if DEBUG
                Console.WriteLine(Environment.TickCount + "Adding new skillshot: " + skillshot.SpellData.SpellName);
#endif

                DetectedSkillshots.Add(skillshot);
            }
        }

    }
}
