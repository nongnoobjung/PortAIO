#region

using LeagueSharp.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

#endregion

namespace Infected_Twitch.Menus
{
    internal class MenuConfig
    {
        private static Menu Menu { get; set; }

        public static Menu comboMenu, harassMenu, laneMenu, miscMenu, killstealMenu, trinketMenu, drawMenu, exploitMenu, jungleMenu;

        public static void Load()
        {
            Menu = MainMenu.AddMenu("Infected Twitch", "InfectedTwitch");


            // Combo
            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("ComboW", new CheckBox("Use W", true));
            comboMenu.Add("UseYoumuu", new CheckBox("Use Youmuu", true));
            comboMenu.Add("UseBotrk", new CheckBox("Use Blade Of The Ruined King", true));


            // Harass
            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("HarassW", new CheckBox("Use W", true));
            harassMenu.Add("HarassE", new Slider("E At Max E Range", 4, 0, 6));


            // Lane
            laneMenu = Menu.AddSubMenu("Lane", "Lane");
            laneMenu.Add("LaneW", new CheckBox("Use W", true));


            // Jungle
            jungleMenu = Menu.AddSubMenu("Jungle", "Jungle");
            jungleMenu.Add("JungleW", new CheckBox("Use W", true));
            jungleMenu.Add("JungleE", new CheckBox("Use E", true));


            // Misc
            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("DrawDmg", new CheckBox("Damage Indicator", true));
            miscMenu.Add("EBeforeDeath", new CheckBox("Use E Before Death", true));
            miscMenu.Add("StealEpic", new CheckBox("Steal Herald, Baron & Dragons", true));
            miscMenu.Add("StealRed", new CheckBox("Steal Redbuff", true));
            miscMenu.Add("QRecall", new KeyBind("Q Recall", false, KeyBind.BindTypes.HoldActive, 'B'));


            // Killsteal
            killstealMenu = Menu.AddSubMenu("Killsteal", "Killsteal");
            killstealMenu.Add("KillstealE", new CheckBox("Killsecure E", true));
            killstealMenu.Add("KillstealIgnite", new CheckBox("Killsecure Ignite", true));


            // Trinket
            trinketMenu = Menu.AddSubMenu("Trinket", "Trinket");
            trinketMenu.Add("BuyTrinket", new CheckBox("Buy Trinket", true));
            trinketMenu.Add("TrinketList", new ComboBox("Choose Trinket", 1, "Oracle Alternation", "Farsight Alternation"));


            // Exploit / Mechanic
            exploitMenu = Menu.AddSubMenu("Exploit", "Exploit");
            exploitMenu.AddLabel("Note: This is safe to use!");
            exploitMenu.Add("UseExploit", new CheckBox("Exploit", true));
            exploitMenu.AddLabel("Will try E AA Q / E AA AA Q");
            exploitMenu.Add("EAAQ", new CheckBox("E AA Q", true));

        }

       
        // List
        public static int TrinketList => miscMenu["TrinketList"].Cast<ComboBox>().CurrentValue;


        // Keybind
        public static bool QRecall => miscMenu["QRecall"].Cast<KeyBind>().CurrentValue;


        // Slider
        public static int HarassE => harassMenu["HarassE"].Cast<Slider>().CurrentValue;


        // Bool
        public static bool ComboW => comboMenu["ComboW"].Cast<CheckBox>().CurrentValue;
        public static bool HarassW => harassMenu["HarassW"].Cast<CheckBox>().CurrentValue;
        public static bool LaneW => laneMenu["LaneW"].Cast<CheckBox>().CurrentValue;
        public static bool JungleW => jungleMenu["JungleW"].Cast<CheckBox>().CurrentValue;
        public static bool JungleE => jungleMenu["JungleE"].Cast<CheckBox>().CurrentValue;
        public static bool KillstealE => killstealMenu["KillstealE"].Cast<CheckBox>().CurrentValue;
        public static bool KillstealIgnite => killstealMenu["KillstealIgnite"].Cast<CheckBox>().CurrentValue;
        public static bool UseBotrk => comboMenu["UseBotrk"].Cast<CheckBox>().CurrentValue;
        public static bool UseYoumuu => comboMenu["UseYoumuu"].Cast<CheckBox>().CurrentValue;
        public static bool BuyTrinket => trinketMenu["BuyTrinket"].Cast<CheckBox>().CurrentValue;
        public static bool UseExploit => exploitMenu["UseExploit"].Cast<CheckBox>().CurrentValue;
        public static bool Eaaq => exploitMenu["EAAQ"].Cast<CheckBox>().CurrentValue;
        public static bool StealEpic => miscMenu["StealEpic"].Cast<CheckBox>().CurrentValue;
        public static bool StealRed => miscMenu["StealRed"].Cast<CheckBox>().CurrentValue;
        public static bool DrawDmg => miscMenu["DrawDmg"].Cast<CheckBox>().CurrentValue;
        public static bool EBeforeDeath => miscMenu["EBeforeDeath"].Cast<CheckBox>().CurrentValue;

    }
}
