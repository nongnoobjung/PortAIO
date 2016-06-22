using ExorAIO.Champions.Anivia;
using ExorAIO.Champions.Ashe;
using ExorAIO.Champions.Caitlyn;
using ExorAIO.Champions.Darius;
using ExorAIO.Champions.DrMundo;
using ExorAIO.Champions.Graves;
using ExorAIO.Champions.Jax;
using ExorAIO.Champions.Jhin;
using ExorAIO.Champions.Nautilus;
using ExorAIO.Champions.Nunu;
using ExorAIO.Champions.Olaf;
using ExorAIO.Champions.Pantheon;
using ExorAIO.Champions.Quinn;
using ExorAIO.Champions.Renekton;
using ExorAIO.Champions.Ryze;
using ExorAIO.Champions.Sivir;
using ExorAIO.Champions.Tryndamere;
using ExorAIO.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;

namespace ExorAIO.Core
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
                case "Ashe":
                    new Ashe().OnLoad();
                    break;
                case "Graves":
                    new Graves().OnLoad();
                    break;
                case "Karma":
                    new ExorAIO.Champions.Karma.Karma().OnLoad();
                    break;
                case "Jax":
                    new Jax().OnLoad();
                    break;
                case "Quinn":
                    new Quinn().OnLoad();
                    break;
                case "Pantheon":
                    new Pantheon().OnLoad();
                    break;
                default:
                    Vars.IsLoaded = false;
                    break;
            }
        }
    }
}