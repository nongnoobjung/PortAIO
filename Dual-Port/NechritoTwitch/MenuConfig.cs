#region

using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

#endregion

namespace Nechrito_Twitch
{
    internal class MenuConfig
    {
        public static Menu Config, combo, harass, lane, steal, draw, misc, ExploitMenu;
        public static string MenuName = "Nechrito Twitch";

        public static void LoadMenu()
        {
            Config = MainMenu.AddMenu(MenuName, MenuName);

            combo = Config.AddSubMenu("Combo", "Combo");
            combo.Add("UseW", new CheckBox("Use W"));
            combo.Add("KsE", new CheckBox("Ks E"));

            harass = Config.AddSubMenu("Harass", "Harass");
            harass.Add("harassW", new CheckBox("Use W"));
            harass.Add("ESlider", new Slider("E Stack When Out Of AA Range", 0, 0, 6));

            lane = Config.AddSubMenu("Lane", "Lane");
            lane.Add("laneW", new CheckBox("Use W"));

            steal = Config.AddSubMenu("Steal", "Steal");
            steal.Add("StealEpic", new CheckBox("Dragon & Baron"));
            steal.Add("StealBuff", new CheckBox("Steal Redbuff"));

            draw = Config.AddSubMenu("Draw", "Draw");
            draw.Add("dind", new CheckBox("Dmg Indicator"));

            misc = Config.AddSubMenu("Misc", "Misc");
            misc.Add("QRecall", new KeyBind("QRecall", false, KeyBind.BindTypes.HoldActive, 'T'));

            ExploitMenu = Config.AddSubMenu("ExploitMenu", "ExploitMenu");
            ExploitMenu.Add("Exploit", new CheckBox("Exploits", false));
            ExploitMenu.Add("EAA", new CheckBox("E AA Q", false));

        }
        // Menu Items
        public static bool StealEpic => steal["StealEpic"].Cast<CheckBox>().CurrentValue;
        public static bool StealBuff => steal["StealBuff"].Cast<CheckBox>().CurrentValue;

        public static bool UseW => combo["UseW"].Cast<CheckBox>().CurrentValue;
        public static bool KsE => combo["KsE"].Cast<CheckBox>().CurrentValue;

        public static bool LaneW => lane["laneW"].Cast<CheckBox>().CurrentValue;

        public static bool HarassW => harass["harassW"].Cast<CheckBox>().CurrentValue;

        public static bool Dind => draw["dind"].Cast<CheckBox>().CurrentValue;

        public static bool Exploit => ExploitMenu["Exploit"].Cast<CheckBox>().CurrentValue;

        public static bool EAA => ExploitMenu["EAA"].Cast<CheckBox>().CurrentValue;

        public static bool QRecall => misc["QRecall"].Cast<KeyBind>().CurrentValue;

        public static int ESlider => harass["ESlider"].Cast<Slider>().CurrentValue;
    }
}
