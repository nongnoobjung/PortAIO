#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;

#endregion

namespace Nechrito_Twitch // Namespace, if we'd put this class in a folder it'd be "Nechrito_Twitch.FOLDER_NAME
{
    internal class Program // It's internal since nothing else will have access to this class. 
    {
        private static readonly AIHeroClient Player = ObjectManager.Player; // We're only going to read off of the given API "Player"
        private static readonly HpBarIndicator Indicator = new HpBarIndicator(); // Loads in HpBarIndicator.cs into Program.cs, which makes us able to make use of it

        /// <summary>
        ///  String with all jungle monsters we're going to use in Jungleclear
        /// </summary>
        private static readonly string[] Monsters =
        {
            "SRU_Red", "SRU_Gromp", "SRU_Krug", "SRU_Razorbeak", "SRU_Murkwolf"
        };

        /// <summary>
        /// Strings with all dragon names, Baron & RiftHerald
        /// </summary>
        private static readonly string[] Dragons =
        {
            "SRU_Dragon_Air", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Earth", "SRU_Dragon_Elder", "SRU_Baron",
            "SRU_RiftHerald"
        };

        /// <summary>
        /// Gives us the Damage of Twitch's E spell
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static float GetDamage(Obj_AI_Base target)
        {
            return Spells._e.GetDamage(target);
        }

        // Initializes the loading process

        public static void OnGameLoad()
        {
            if (Player.ChampionName != "Twitch") return; // If our player name isn't Twitch, we will return and we will not load the script

            // Printing chat with our message when starting the loading process
            Chat.Print(
                "<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Nechrito Twitch</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Version: 7</font></b>");
            Chat.Print(
                "<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Update</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Exploit</font></b>");


            Recall(); // Loads our Recall void
            Drawing.OnEndScene += Drawing_OnEndScene; // With this we can draw health bar and damages
            Game.OnUpdate += Game_OnUpdate; // Initialises our update method
            MenuConfig.LoadMenu(); // Loads our Menu
            Spells.Initialise();   // Initialises our Spells
        }

        /// <summary>
        /// Our Update Method
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnUpdate(EventArgs args)
        {
            AutoE(); // Updates our "AutoE" void 

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Exploit(); // Updates the Exploit void 
                Combo(); // Update our combo method
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass(); // Update our harass method

            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear(); // Update our laneclear method AND Jungleclear method
                JungleClear();
            }
        }

        private static void Exploit()
        {
            var target = TargetSelector.GetTarget(Player.AttackRange, DamageType.Physical); // Looks for a target within AA Range.
            if (target == null || !target.LSIsValidTarget() || target.IsInvulnerable) return; //If target isn't defined or a valid target within AA Range or target is in zhonyas, return

            if (!MenuConfig.Exploit) return; // If Exploit in Menu is "Off", return.
            if (!Spells._q.IsReady()) return;// if Twitch's Q isn't ready, return.

            if (Spells._e.IsReady() && MenuConfig.EAA) // If Twitch's E is ready and EAA Menu is "On", do the following code within the brackets.
            {
                if (target.Health <= Player.LSGetAutoAttackDamage(target, true) * (1.15) + GetDamage(target) * (1.35)) // If our targets healh is less than AA + Twitch's E Damage, do the following
                {
                    if (!target.IsFacing(Player) && target.Distance(Player) >= Player.AttackRange - 50) // Return if our target isn't facing us and he's at AA range - 50
                    {
                        Chat.Print("Exploit Will NOT Interrupt Combo!!"); // Prints in chat
                        return; // Will return 
                    }
                    // Here we could use an else statement, but it would be redudant and a waste.
                    Spells._e.Cast(); // Will cast Twitch's E spell
                    Chat.Print("Casting E to then cast AA Q"); // Delays the message with 0.5 seconds (500 milliseconds)
                }

                // FIGURE OUT HOW TO DO THS BETTER!
                // WITH THIS, WE CAN E AA AA Q IF WE CAN'T DO E AA Q! = 100% will be successful!
                else if (target.Health <= Player.LSGetAutoAttackDamage(target, true) * 2 * (1.15) + GetDamage(target) * (1.35))
                {
                    if (!target.IsFacing(Player) && target.Distance(Player) >= Player.AttackRange - 50) // Return if our target isn't facing us and he's at AA range - 50
                    {
                        Chat.Print("Exploit Will NOT Interrupt Combo!!"); // Prints in chat
                        return; // Will return 
                    }
                    // Here we could use an else statement, but it would be redudant and a waste.
                    Spells._e.Cast(); // Will cast Twitch's E spell
                    Chat.Print("Casting E to then cast AA Q"); // Delays the message with 0.5 seconds (500 milliseconds)
                }
            }

            if (!(target.Health < Player.LSGetAutoAttackDamage(target, true) * (1.035)) || !Player.Spellbook.IsAutoAttacking) return; // Returns if our targets health is less than AA dmg and we aren't attacking

            Spells._q.Cast(); // Will cast Twitch's Q spell
            do // begins a "do" loop
            {
                // Delays the message with 0.5s 
                LeagueSharp.Common.Utility.DelayAction.Add(500, () => Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Exploit Active</font></b><b><font color=\"#FFFFFF\">]</font></b>"));
            } while (Spells._q.Cast()); // Will loop this message during the time we cast Q.
        }

        // Combo
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Spells._w.Range, DamageType.Physical); // Gets a target from Twitch's W spell Range
            if (target == null || !target.LSIsValidTarget() || target.IsInvulnerable) return; // If the target isn't defined, not a valid target within our range or just Invulnerable, return

            if (!MenuConfig.UseW) return; // If our Combo W Menu is "Off", return
            if (target.Health < Player.LSGetAutoAttackDamage(target, true) * 2) return; // If our targets health is less than Twitch's AA * 2, return (We wont cast W if killable by 2 AA
            var wPred = Spells._w.GetPrediction(target).CastPosition; // Twitch's W prediction for our Combo

            if (Spells._w.IsReady()) // if Twitch's W spell is ready
            {
                Spells._w.Cast(wPred); // Cast to the given prediction
            }
        }

        // Harass
        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Spells._e.Range, DamageType.Physical); // Searches for targets within Twitch's E spell range
            if (target == null || target.IsDead || !target.LSIsValidTarget() || target.IsInvulnerable) return; // If the target isn't defined, not a valid target within our range or just Invulnerable, return

            // If our target isn't in AA range and the target has the amount of E stacks we gave it in our Menu & Twitch's mana is above 49% & Twitch's E is ready
            if (!Orbwalking.InAutoAttackRange(target) && target.GetBuffCount("twitchdeadlyvenom") >= MenuConfig.ESlider && Player.ManaPercent >= 50 && Spells._e.IsReady())
            {
                Spells._e.Cast(); // Cast Twitch's E spell
            }

            if (!MenuConfig.HarassW) return; // If Harass => Use W is "Off", return

            var wPred = Spells._w.GetPrediction(target).CastPosition; // Twitch's W spell prediction

            if (target.LSIsValidTarget(Spells._w.Range) && Spells._w.IsReady()) // if Twitch's target is valid within W range & Twitch's W spell is ready
            {
                Spells._w.Cast(wPred); // Cast Twitch's W spell to the given prediction
            }
        }

        // Laneclear
        private static void LaneClear()
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) return; // If we aren't pressing our laneclear keybind, return

            var minions = MinionManager.GetMinions(Player.ServerPosition, 800); // searches for minions within 800 units

            if (minions == null) return; // If there aren't any | they aren't defined, return

            var wPrediction = Spells._w.GetCircularFarmLocation(minions); // Twitch's W spell prediction for minions

            if (!MenuConfig.LaneW) return; // If our Lane => Use W is "Off" return

            if (!Spells._w.IsReady()) return; // if Twitch's W spell isn't ready, return

            if (wPrediction.MinionsHit >= 4) // If the prediction can hit 4 or more minions
            {
                Spells._w.Cast(wPrediction.Position); // Cast Twitch's W spell to the given prediction
            }
        }

        // Jungle
        private static void JungleClear()
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) return; // If we aren't pressing the laneclear keybind, return
            if (Player.Level == 1) return; // If our player level is 1, return. This is so we can prevent stealing mobs from our jungler

            // Gets jungle mobs within Twitch's W spell range, prioritizes the max health mob. 
            var mobs = MinionManager.GetMinions(Player.Position, Spells._w.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            var wPrediction = Spells._w.GetCircularFarmLocation(mobs); // Twitch's prediction for casting W in jungle
            if (mobs.Count == 0) return; // If there aren't any mobs, return (This isn't really neccessary) 
            if (MenuConfig.JungleW) // If Jungle => Use W is "On" do the following code within the brackets
            {
                if (wPrediction.MinionsHit >= 3 && Player.ManaPercent >= 20) // If prediction can hit 3 or more mobs & Twitch's mana is above 19%
                {
                    Spells._w.Cast(wPrediction.Position); // Cast Twitch's W spell to the given prediction
                }
            }

            if (!MenuConfig.JungleE) return; // If Jungle => Use E is "Off", return

            foreach (var m in ObjectManager.Get<Obj_AI_Base>().Where(x => Monsters.Contains(x.CharData.BaseSkinName) && !x.IsDead)) // Looks for mobs that isn't dead
            {
                if (m.Health < Spells._e.GetDamage(m)) // if the mobs health is less than Twitch's E spell
                {
                    Spells._e.Cast(m); // Execute mob with Twitch's E spell
                }
            }
        }

        private static void Recall()
        {
            Spellbook.OnCastSpell += (sender, eventArgs) => // Subscribes to the event(?)
            {
                if (!MenuConfig.QRecall) return; // If Misc => Q Recall is "Off", return
                if (!Spells._q.IsReady() || !Spells._recall.IsReady()) return; // If Twitch's Q isn't ready or Recall isn't ready, return
                if (eventArgs.Slot != SpellSlot.Recall) return; // If the even-slot isn't Recall, return

                Spells._q.Cast(); // Cast Twitch's Q spell

                // Delays the action with Q Delay + 0.3 seconds, then cast Recall
                LeagueSharp.Common.Utility.DelayAction.Add((int)Spells._q.Delay + 300, () => ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Recall));
                eventArgs.Process = false; // It's as return or bool, the code has been executed 
            };
        }


        private static void AutoE()
        {
            // Looks for mobs within Twitch's E spell range, prioritizes the mob with most health
            var mob = MinionManager.GetMinions(Spells._e.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (Player.Level == 1) return; // if Twitch's level is 1, return. We don't want to auto steal jungle from our jungler.

            // If Menu => Steal => Steal Dragon & Baron is "On" execute the following code within the brackets
            if (MenuConfig.StealEpic)
            {
                // Searches through our list above "Dragons"
                foreach (var m in ObjectManager.Get<Obj_AI_Base>().Where(x => Dragons.Contains(x.CharData.BaseSkinName) && !x.IsDead))
                {
                    if (m.Health < Spells._e.GetDamage(m)) // If monster is found and targets health is less than Twitch's E spell
                    {
                        Spells._e.Cast(m); // Execute with Twitch's E spell
                    }
                }
            }

            // If Menu => Steal => Steal Redbuff, do the following code within the brackets
            if (MenuConfig.StealBuff)
            {
                foreach (var m in mob)
                {
                    // If base skin name is SRU_Red (Redbuff)
                    if (m.CharData.BaseSkinName.Contains("SRU_Red")) continue;
                    if (Spells._e.IsKillable(m)) // If Redbuff is killable
                        Spells._e.Cast(); // Kill Redbuff with Twitch's E spell
                }
            }

            if (!MenuConfig.KsE) return; // If Menu => Combo => Killsecure E is "Off", return

            // Searches for enemies that are valid within Twitch's E spell range & Is killable by Twitch's E spell
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.LSIsValidTarget(Spells._e.Range) && Spells._e.IsKillable(enemy)))
            {
                Spells._e.Cast(enemy); // Executes enemy with Twitch's E spell
            }
        }


        private static void Drawing_OnEndScene(EventArgs args)
        {
            // if there are valid targts & target isn't dead execute the following code within the brackets
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
            {
                if (!MenuConfig.Dind) continue;

                Indicator.unit = enemy;
                Indicator.drawDmg(GetDamage(enemy), new ColorBGRA(255, 204, 0, 170));
            }
        }
    }
}