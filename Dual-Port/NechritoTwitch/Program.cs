using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;

namespace Nechrito_Twitch
{
    internal class Program
    {
        private static readonly AIHeroClient Player = ObjectManager.Player;
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();

        private static readonly string[] Monsters =
        {
           "SRU_Red", "SRU_Gromp", "SRU_Krug","SRU_Razorbeak","SRU_Murkwolf"
        };

        private static readonly string[] Dragons =
        {
            "SRU_Dragon_Air","SRU_Dragon_Fire","SRU_Dragon_Water","SRU_Dragon_Earth","SRU_Dragon_Elder","SRU_Baron","SRU_RiftHerald"
        };

        private static float GetDamage(Obj_AI_Base target)
        {
            return Spells._e.GetDamage(target);
        }

        public static void OnGameLoad()
        {
            if (Player.ChampionName != "Twitch") return;

            Chat.Print(
                "<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Nechrito Twitch</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Version: 5½</font></b>");
            Chat.Print(
                "<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Update</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Rework & Exploit</font></b>");

            
            Recall();
            Drawing.OnEndScene += Drawing_OnEndScene;
            Game.OnUpdate += Game_OnUpdate;
            MenuConfig.LoadMenu();
            Spells.Initialise();
        }

        private static void Game_OnUpdate(EventArgs args)
        {
           AutoE();
           Exploit();

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
                LaneClear();
                JungleClear();
            }
        }

        private static void Exploit()
        {
            var target = TargetSelector.GetTarget(Player.AttackRange, DamageType.Physical);
            if (target == null || !target.IsValidTarget() || target.IsInvulnerable) return;

            if (!MenuConfig.Exploit) return;
            if (!Spells._q.IsReady()) return;

            if (Spells._e.IsReady() && MenuConfig.EAA)
            {
                if (!target.IsFacing(Player) && target.Distance(Player) >= Player.AttackRange - 50)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(500, ()=> Chat.Print("Exploit Will NOT Interrupt "));
                    return;
                }

                if (target.Health <= Player.LSGetAutoAttackDamage(target) * 1.275 + GetDamage(target))
                {
                    Spells._e.Cast();
                    LeagueSharp.Common.Utility.DelayAction.Add(500, ()=> Chat.Print("Casting E to then cast AA Q"));
                }
            }

            if (!(target.Health < Player.GetAutoAttackDamage(target)) || !Player.Spellbook.IsAutoAttacking) return;

            Spells._q.Cast();
            do
            {
                LeagueSharp.Common.Utility.DelayAction.Add(500, () => Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Exploit Active</font></b><b><font color=\"#FFFFFF\">]</font></b>"));
                // Game.PrintChat("Casting Q");
            } while (Spells._q.Cast());
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Spells._w.Range, DamageType.Physical);
            if (target == null || !target.IsValidTarget() || target.IsInvulnerable) return;

            

            if (!MenuConfig.UseW) return;
            if (target.Health < Player.LSGetAutoAttackDamage(target, true) * 2) return;
            var wPred = Spells._w.GetPrediction(target).CastPosition;

            if (Spells._w.IsReady())
            {
                Spells._w.Cast(wPred);
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Spells._e.Range, DamageType.Physical);

            if (target == null || target.IsDead || !target.LSIsValidTarget() || target.IsInvulnerable) return;
            if (!Orbwalking.InAutoAttackRange(target) && target.GetBuffCount("twitchdeadlyvenom") >= MenuConfig.ESlider && Player.ManaPercent >= 50 && Spells._e.IsReady())
            {
                Spells._e.Cast();
            }

            if (!MenuConfig.HarassW) return;
            var wPred = Spells._w.GetPrediction(target).CastPosition;

            if (target.IsValidTarget(Spells._w.Range) && Spells._w.IsReady())
            {
                Spells._w.Cast(wPred);
            }
        }

        private static void LaneClear()
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) return;

            var minions = MinionManager.GetMinions(Player.ServerPosition, 800);

            if (minions == null) return;

            var wPrediction = Spells._w.GetCircularFarmLocation(minions);

            if (!MenuConfig.LaneW) return;

            if (!Spells._w.IsReady()) return;

            if (wPrediction.MinionsHit >= 4)
            {
                Spells._w.Cast(wPrediction.Position);
            }
        }

        private static void JungleClear()
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) return;

            var mobs = MinionManager.GetMinions(Player.Position, Spells._w.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            var wPrediction = Spells._w.GetCircularFarmLocation(mobs);
            if (mobs.Count == 0) return;

            if (wPrediction.MinionsHit >= 3 && Player.ManaPercent >= 20)
            {
                Spells._w.Cast(wPrediction.Position);
            }

            foreach (var m in ObjectManager.Get<Obj_AI_Base>().Where(x => Monsters.Contains(x.CharData.BaseSkinName) && !x.IsDead))
            {
                if (m.Health < Spells._e.GetDamage(m))
                {
                    Spells._e.Cast(m);
                }
            }
        }

        private static void Recall()
        {
            Spellbook.OnCastSpell += (sender, eventArgs) =>
            {
                if(!MenuConfig.QRecall) return;
                if (!Spells._q.IsReady() || !Spells._recall.IsReady()) return;
                if (eventArgs.Slot != SpellSlot.Recall) return;

                Spells._q.Cast();
                LeagueSharp.Common.Utility.DelayAction.Add((int)Spells._q.Delay + 300, () => ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Recall));
                eventArgs.Process = false;
            };
        }


        private static void AutoE()
        {
            var mob = MinionManager.GetMinions(Spells._e.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (MenuConfig.StealEpic)
            {
                foreach (var m in ObjectManager.Get<Obj_AI_Base>().Where(x => Dragons.Contains(x.CharData.BaseSkinName) && !x.IsDead))
                {
                    if (m.Health < Spells._e.GetDamage(m))
                    {
                        Spells._e.Cast(m);
                    }
                }
            }

            if (MenuConfig.StealBuff)
            {
                foreach (var m in mob)
                {
                    if (m.CharData.BaseSkinName.Contains("SRU_Red") && MenuConfig.StealBuff) continue;
                    if (Spells._e.IsKillable(m))
                        Spells._e.Cast();
                }
            }
            
            if (!MenuConfig.KsE) return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.LSIsValidTarget(Spells._e.Range) && Spells._e.IsKillable(enemy)))
            {
                Spells._e.Cast(enemy);
            }
        }


        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
            {
                if (!MenuConfig.Dind) continue;

                Indicator.unit = enemy;
                Indicator.drawDmg(GetDamage(enemy), new ColorBGRA(255, 204, 0, 170));
            }
        }
    }
}
