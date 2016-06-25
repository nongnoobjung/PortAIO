namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = SharpDX.Color;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;
    using Spell = LeagueSharp.Common.Spell;
    using Damage = LeagueSharp.Common.Damage;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Smite : IPlugin
    {
        #region Constants

        /// <summary>
        ///     The smite range
        /// </summary>
        public const float SmiteRange = 570f;

        #endregion

        #region Static Fields

        public static Obj_AI_Minion Minion;

        private static readonly string[] SmiteObjects =
            {
                "SRU_Red", "SRU_Blue", "SRU_Dragon_Water", "SRU_Dragon_Fire",
                "SRU_Dragon_Earth", "SRU_Dragon_Air", "SRU_Dragon_Elder",
                "SRU_Baron", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak",
                "SRU_RiftHerald", "SRU_Krug", "TT_Spiderboss", "TT_NGolem",
                "TT_NWolf", "TT_NWraith"
            };

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether the combo mode is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if combo mode is active; otherwise, <c>false</c>.
        /// </value>
        public bool ComboModeActive
            =>
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        public Spell SmiteSpell { get; set; }

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The stage.
        /// </value>
        public int Stage { get; set; }

        #endregion

        #region Properties

        private Menu Menu { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player => ObjectManager.Player;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var smiteSlot = this.Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));

            if (smiteSlot == null)
            {
                return;
            }

            var smiteMenu = rootMenu.AddSubMenu("Smite", "Smite");
            {
                smiteMenu.Add("ElSmite.Activated", new KeyBind("Smite Activated", true, KeyBind.BindTypes.PressToggle, 'M'));
                smiteMenu.Add("Smite.Ammo", new CheckBox("Save 1 smite charge"));

                if (Game.MapId == GameMapId.SummonersRift)
                {
                    smiteMenu.AddGroupLabel("Big Mobs");
                    smiteMenu.Add("SRU_Dragon_Air", new CheckBox("Air Dragon"));
                    smiteMenu.Add("SRU_Dragon_Earth", new CheckBox("Earth Dragon"));
                    smiteMenu.Add("SRU_Dragon_Fire", new CheckBox("Fire Dragon"));
                    smiteMenu.Add("SRU_Dragon_Water", new CheckBox("Water Dragon"));
                    smiteMenu.Add("SRU_Dragon_Elder", new CheckBox("Elder Dragon"));
                    smiteMenu.Add("SRU_Baron", new CheckBox("Baron"));
                    smiteMenu.Add("SRU_Red", new CheckBox("Red buff"));
                    smiteMenu.Add("SRU_Blue", new CheckBox("Blue buff"));
                    smiteMenu.Add("SRU_RiftHerald", new CheckBox("Rift Herald"));
                    smiteMenu.AddSeparator();
                    smiteMenu.AddGroupLabel("Small Mobs");
                    smiteMenu.Add("SRU_Gromp", new CheckBox("Gromp", false));
                    smiteMenu.Add("SRU_Murkwolf", new CheckBox("Wolves", false));
                    smiteMenu.Add("SRU_Krug", new CheckBox("Krug", false));
                    smiteMenu.Add("SRU_Razorbeak", new CheckBox("Chicken camp", false));
                    smiteMenu.Add("Sru_Crab", new CheckBox("Crab", false));
                }

                if (Game.MapId == GameMapId.TwistedTreeline)
                {
                    smiteMenu.AddGroupLabel("Mobs");
                    smiteMenu.Add("TT_Spiderboss", new CheckBox("Vilemaw Enabled"));
                    smiteMenu.Add("TT_NGolem", new CheckBox("Golem Enabled"));
                    smiteMenu.Add("TT_NWolf", new CheckBox("Wolf Enabled"));
                    smiteMenu.Add("TT_NWraith", new CheckBox("Wraith Enabled"));
                }

                //Champion Smite
                smiteMenu.AddGroupLabel("Champion smite");
                smiteMenu.Add("ElSmite.KS.Activated", new CheckBox("Use smite to killsteal"));
                smiteMenu.Add("ElSmite.KS.Combo", new CheckBox("Use smite in combo"));

                //Drawings
                smiteMenu.AddGroupLabel("Drawings");
                smiteMenu.Add("ElSmite.Draw.Range", new CheckBox("Draw smite Range"));
                smiteMenu.Add("ElSmite.Draw.Text", new CheckBox("Draw smite text"));
                smiteMenu.Add("ElSmite.Draw.Damage", new CheckBox("Draw smite Damage", false));
            }

            this.Menu = smiteMenu;
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public void Load()
        {
            try
            {
                var smiteSlot = this.Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));

                if (smiteSlot != null)
                {
                    this.SmiteSpell = new Spell(smiteSlot.Slot, SmiteRange, DamageType.True);

                    Drawing.OnDraw += this.OnDraw;
                    Game.OnUpdate += this.OnUpdate;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }
        }

        #endregion

        #region Methods

        private void OnDraw(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead)
                {
                    return;
                }

                var smiteActive = getKeyBindItem(this.Menu, "ElSmite.Activated");
                var drawSmite = getCheckBoxItem(this.Menu, "ElSmite.Draw.Range");
                var drawText = getCheckBoxItem(this.Menu, "ElSmite.Draw.Text");
                var playerPos = Drawing.WorldToScreen(this.Player.Position);
                var drawDamage = getCheckBoxItem(this.Menu, "ElSmite.Draw.Damage");


                if (smiteActive && this.SmiteSpell != null)
                {
                    if (drawText && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) == SpellState.Ready)
                    {
                        Drawing.DrawText(
                            playerPos.X - 70,
                            playerPos.Y + 40,
                            System.Drawing.Color.GhostWhite,
                            "Smite active");
                    }

                    if (drawText && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) != SpellState.Ready)
                    {
                        Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, System.Drawing.Color.Red, "Smite cooldown");
                    }

                    if (drawDamage && this.SmiteDamage() != 0)
                    {
                        var minions =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(
                                    m =>
                                    m.Team == GameObjectTeam.Neutral && m.LSIsValidTarget()
                                    && SmiteObjects.Contains(m.CharData.BaseSkinName));

                        foreach (var minion in minions.Where(m => m.IsHPBarRendered))
                        {
                            var hpBarPosition = minion.HPBarPosition;
                            var maxHealth = minion.MaxHealth;
                            var sDamage = this.SmiteDamage();
                            var x = this.SmiteDamage() / maxHealth;
                            var barWidth = 0;

                            switch (minion.CharData.BaseSkinName)
                            {
                                case "SRU_RiftHerald":
                                    barWidth = 145;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 17),
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 30),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + (float)(barWidth * x),
                                        hpBarPosition.Y - 5,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Dragon_Air":
                                case "SRU_Dragon_Water":
                                case "SRU_Dragon_Fire":
                                case "SRU_Dragon_Elder":
                                case "SRU_Dragon_Earth":
                                    barWidth = 145;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 22),
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 30),
                                        2f,
                                        System.Drawing.Color.Orange);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + (float)(barWidth * x),
                                        hpBarPosition.Y - 5,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Red":
                                case "SRU_Blue":
                                    barWidth = 145;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 20),
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 30),
                                        2f,
                                        System.Drawing.Color.Orange);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + (float)(barWidth * x),
                                        hpBarPosition.Y - 5,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Baron":
                                    barWidth = 194;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 18 + (float)(barWidth * x), hpBarPosition.Y + 20),
                                        new Vector2(hpBarPosition.X + 18 + (float)(barWidth * x), hpBarPosition.Y + 35),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + (float)(barWidth * x),
                                        hpBarPosition.Y - 3,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Gromp":
                                    barWidth = 87;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Murkwolf":
                                    barWidth = 75;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "Sru_Crab":
                                    barWidth = 61;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 8),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Razorbeak":
                                    barWidth = 75;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "SRU_Krug":
                                    barWidth = 81;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        System.Drawing.Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        System.Drawing.Color.Chartreuse,
                                        sDamage.ToString(CultureInfo.InvariantCulture));
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (drawText && this.SmiteSpell != null)
                    {
                        Drawing.DrawText(
                            playerPos.X - 70,
                            playerPos.Y + 40,
                            System.Drawing.Color.Red,
                            "Smite not active!");
                    }
                }

                var smiteSpell = this.SmiteSpell;
                if (smiteSpell != null)
                {
                    if (smiteActive && drawSmite
                        && this.Player.Spellbook.CanUseSpell(smiteSpell.Slot) == SpellState.Ready)
                    {
                        Render.Circle.DrawCircle(this.Player.Position, SmiteRange, System.Drawing.Color.Green);
                    }

                    if (drawSmite && this.Player.Spellbook.CanUseSpell(smiteSpell.Slot) != SpellState.Ready)
                    {
                        Render.Circle.DrawCircle(this.Player.Position, SmiteRange, System.Drawing.Color.Red);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead || this.SmiteSpell == null || !getKeyBindItem(this.Menu, "ElSmite.Activated"))
                {
                    return;
                }

                foreach (var minion in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            o =>
                            ObjectManager.Player.Position.LSDistance(o.ServerPosition) <= 950f
                            && o.Team == GameObjectTeam.Neutral && !o.CharData.BaseSkinName.ToLower().Contains("barrel")
                            && !o.CharData.BaseSkinName.ToLower().Contains("mini")
                            && !o.CharData.BaseSkinName.ToLower().Contains("respawn") 
                            && getCheckBoxItem(this.Menu, o.CharData.BaseSkinName)
                            && o.IsHPBarRendered && o.IsVisible
                            ))
                {
                    if (this.SmiteSpell.IsReady())
                    {
                        if (minion.LSIsValidTarget(SmiteRange))
                        {
                            if (this.Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite) > minion.Health)
                            {
                                this.SmiteSpell.Cast(minion);
                            }
                        }
                    }

                    return;
                }

                if (getCheckBoxItem(this.Menu, "Smite.Ammo") && this.Player.GetSpell(this.SmiteSpell.Slot).Ammo == 1)
                {
                    return;
                }

                if (getCheckBoxItem(this.Menu, "ElSmite.KS.Combo") && this.Player.GetSpell(this.SmiteSpell.Slot).Name.ToLower() == "s5_summonersmiteduel" && this.ComboModeActive && this.SmiteSpell.IsReady())
                {
                    var smiteComboEnemy =
                        HeroManager.Enemies.FirstOrDefault(hero => !hero.IsZombie && hero.LSIsValidTarget(500f));
                    if (smiteComboEnemy != null)
                    {
                        this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, smiteComboEnemy);
                    }
                }

                if (this.Player.GetSpell(this.SmiteSpell.Slot).Name.ToLower() != "s5_summonersmiteplayerganker")
                {
                    return;
                }

                if (getCheckBoxItem(this.Menu, "ElSmite.KS.Activated") && this.SmiteSpell.IsReady())
                {
                    var kSableEnemy =
                        HeroManager.Enemies.FirstOrDefault(
                            hero =>
                            !hero.IsZombie && hero.LSIsValidTarget(SmiteRange)
                            && this.SmiteSpell.GetDamage(hero) >= hero.Health);

                    if (kSableEnemy != null)
                    {
                        this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, kSableEnemy);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }
        }

        private float SmiteDamage()
        {
            try
            {
                return this.Player.Spellbook.GetSpell(this.SmiteSpell.Slot).State == SpellState.Ready
                           ? (float)this.Player.GetSummonerSpellDamage(Minion, Damage.SummonerSpell.Smite)
                           : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e}");
            }

            return 0;
        }

        #endregion
    }
}