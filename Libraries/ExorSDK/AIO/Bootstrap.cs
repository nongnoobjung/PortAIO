using ExorSDK.Champions.Anivia;
using ExorSDK.Champions.Caitlyn;
using ExorSDK.Champions.Darius;
using ExorSDK.Champions.DrMundo;
using ExorSDK.Champions.Jhin;
using ExorSDK.Champions.Nautilus;
using ExorSDK.Champions.Nunu;
using ExorSDK.Champions.Olaf;
using ExorSDK.Champions.Renekton;
using ExorSDK.Champions.Ryze;
using ExorSDK.Champions.Sivir;
using ExorSDK.Champions.Tryndamere;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;

namespace ExorSDK.Core
{
    /// <summary>
    ///     The bootstrap class.
    /// </summary>
    internal class Bootstrap
    {
        /// <summary>
        ///     Tries to load the champion which is being currently played.
        /// </summary>
        public static void LoadChampion()
        {
            switch (GameObjects.Player.ChampionName)
            {
                case "Sivir":
                    new Sivir().OnLoad();
                    break;
                case "Caitlyn":
                    new Caitlyn().OnLoad();
                    break;
                case "Anivia":
                    new Anivia().OnLoad();
                    break;
                case "Darius":
                    new Darius().OnLoad();
                    break;
                case "Nautilus":
                    new Nautilus().OnLoad();
                    break;
                case "Nunu":
                    new Nunu().OnLoad();
                    break;
                case "Olaf":
                    new Olaf().OnLoad();
                    break;
                case "Renekton":
                    new Renekton().OnLoad();
                    break;
                case "Tryndamere":
                    new Tryndamere().OnLoad();
                    break;
                case "Ryze":
                    new Ryze().OnLoad();
                    break;
                case "Jhin":
                    new Jhin().OnLoad();
                    break;
                case "DrMundo":
                    new DrMundo().OnLoad();
                    break;
                default:
                    Vars.IsLoaded = false;
                    break;
            }
        }
    }
}