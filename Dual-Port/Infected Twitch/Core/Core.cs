#region

using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using EloBuddy.SDK;

#endregion

namespace Infected_Twitch.Core
{
    internal class Core
    {
        public static bool HasPassive => Player.HasBuff("TwitchHideInShadows");
        public static AIHeroClient Player => ObjectManager.Player;
        public static AIHeroClient Target => TargetSelector.GetTarget(1200, DamageType.Physical);

        /// <summary>
        ///  String with all jungle monsters we're going to use in Jungleclear
        /// </summary>
        public static readonly string[] Monsters =
        {
            "SRU_Red", "SRU_Gromp", "SRU_Krug", "SRU_Razorbeak", "SRU_Murkwolf"
        };

        /// <summary>
        /// Strings with all dragon names, Baron & RiftHerald
        /// </summary>
        public static readonly string[] Dragons =
        {
            "SRU_Dragon_Air", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Earth", "SRU_Dragon_Elder", "SRU_Baron",
            "SRU_RiftHerald"
        };

    }
}
