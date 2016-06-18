using EloBuddy.SDK.Menu.Values;
using ExorSDK.Utilities;
using LeagueSharp.SDK;

namespace ExorSDK.Champions.Jhin
{
    /// <summary>
    ///     The menu class.
    /// </summary>
    internal class Menus
    {
        /// <summary>
        ///     Sets the menu.
        /// </summary>
        public static void Initialize()
        {
            /// <summary>
            ///     Sets the menu for the Q.
            /// </summary>
            Vars.QMenu = Vars.Menu.AddSubMenu("q", "Use Q to:");
            {
                Vars.QMenu.Add("combo", new CheckBox("Combo", true));
                Vars.QMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.QMenu.Add("lasthit", new Slider("LastHit / if Mana >= x%", 0, 0, 101));
                Vars.QMenu.Add("harass", new Slider("Harass / if Mana >= x%", 50, 0, 101));
                Vars.QMenu.Add("clear", new Slider("Clear / if Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the W.
            /// </summary>
            Vars.WMenu = Vars.Menu.AddSubMenu("w", "Use W to:");
            {
                Vars.WMenu.Add("logical", new CheckBox("Logical", true));
                Vars.WMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.WMenu.Add("laneclear", new Slider("LaneClear / if Mana >= x%", 50, 0, 101));
            }

            /// <summary>
            ///     Sets the menu for the W Whitelist.
            /// </summary>
            Vars.WhiteListMenu = Vars.Menu.AddSubMenu("whitelist", "W: Whitelist Menu");
            {
                foreach (var target in GameObjects.EnemyHeroes)
                {
                    Vars.WhiteListMenu.Add(target.ChampionName.ToLower(), new CheckBox($"Use against: {target.ChampionName}", true));
                }
            }

            /// <summary>
            ///     Sets the menu for the E.
            /// </summary>
            Vars.EMenu = Vars.Menu.AddSubMenu("e", "Use E to:");
            {
                Vars.EMenu.Add("logical", new CheckBox("Logical", true));
                Vars.EMenu.Add("gapcloser", new CheckBox("Anti-Gapcloser", true));
            }

            /// <summary>
            ///     Sets the menu for the R.
            /// </summary>
            Vars.RMenu = Vars.Menu.AddSubMenu("r", "Use R to:");
            {
                Vars.RMenu.Add("combo", new CheckBox("Combo", true));
                Vars.RMenu.Add("killsteal", new CheckBox("KillSteal", true));
                Vars.RMenu.Add("nearmouse", new CheckBox("Focus the enemy nearest to your cursor"));
                Vars.RMenu.AddGroupLabel("- You need to manually start the Ultimate. -");
            }

            /// <summary>
            ///     Sets the menu for the R Whitelist.
            /// </summary>
            Vars.WhiteList2Menu = Vars.Menu.AddSubMenu("whitelist", "R: Whitelist Menu");
            {
                foreach (var target in GameObjects.EnemyHeroes)
                {
                    Vars.WhiteList2Menu.Add(target.ChampionName.ToLower(), new CheckBox($"Use against: {target.ChampionName}", true));
                }
            }

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = Vars.Menu.AddSubMenu("drawings", "Drawings");
            {
                Vars.DrawingsMenu.Add("q", new CheckBox("Q Range"));
                Vars.DrawingsMenu.Add("w", new CheckBox("W Range"));
                Vars.DrawingsMenu.Add("e", new CheckBox("E Range"));
                Vars.DrawingsMenu.Add("r", new CheckBox("R Range"));
            }
        }
    }
}