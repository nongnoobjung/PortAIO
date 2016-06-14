using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;

namespace Nechrito_Twitch
{
    internal class MenuConfig
    {
        public static Menu Config, combo, harass, lane, draw, steal, misc, ExploitMenu;
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

            misc = Config.AddSubMenu("Misc", "Misc");
            misc.Add("QRecall", new KeyBind("QRecall", false, KeyBind.BindTypes.HoldActive, 'T'));

            ExploitMenu = Config.AddSubMenu("ExploitMenu", "ExploitMenu");
            ExploitMenu.Add("Exploit", new CheckBox("Exploits", false));
            ExploitMenu.AddLabel("Will Instant Q After Kill");
            ExploitMenu.Add("EAA", new CheckBox("E AA Q", false));
            ExploitMenu.AddLabel("Will cast E if killable by E + AA then Q");

            draw = Config.AddSubMenu("Draw", "Draw");
            draw.Add("dind", new CheckBox("Dmg Indicator"));
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

        // Menu Items
        public static bool UseW => getCheckBoxItem(combo, "UseW");
        public static bool KsE => getCheckBoxItem(combo, "KsE");
        public static bool LaneW => getCheckBoxItem(lane, "laneW");
        public static bool StealEpic => getCheckBoxItem(steal, "StealEpic");
        public static bool StealBuff => getCheckBoxItem(steal, "StealBuff");
        public static bool HarassW => getCheckBoxItem(harass, "harassW");
        public static bool Dind => getCheckBoxItem(draw, "dind");
        public static bool QRecall => getKeyBindItem(misc, "QRecall");
        public static int ESlider => getSliderItem(harass, "ESlider");
        public static bool Exploit => getCheckBoxItem(draw, "Exploit");
        public static bool EAA => getCheckBoxItem(draw, "EAA");

    }
}
