using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Menus;
using System;

namespace NechritoRiven.Event
{
    class Anim : Core.Core
    {
        public static void OnPlay(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) return;
            var t = 0;
            switch (args.Animation) // Logic from Fluxy
            {
                case "Spell1a":
                    lastQ = Utils.GameTimeTickCount;
                    t = 291;
                    Qstack = 2;
                    break;
                case "Spell1b":
                    lastQ = Utils.GameTimeTickCount;
                    t = 291;
                    Qstack = 3;
                    break;
                case "Spell1c": // q3?
                    lastQ = Utils.GameTimeTickCount;
                    t = 343;
                    Qstack = 1;
                    break;
                case "Spell2":
                    t = 170;
                    break;
                case "Spell3":
                    if (MenuConfig.Burst || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                        MenuConfig.FastHarass || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                        Usables.CastYoumoo();
                    break;
                case "Spell4a":
                    t = 0;
                    lastR = Utils.GameTimeTickCount;
                    break;
                case "Spell4b":
                    t = 150;
                    var target = TargetSelector.SelectedTarget;
                    if (Spells.Q.IsReady() && target.LSIsValidTarget()) ForceCastQ(target);
                    break;
            }

            if (t != 0 && (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None))
            {
                Orbwalker.ResetAutoAttack();
                EloBuddy.SDK.Core.DelayAction(CancelAnimation, t - MenuConfig.Qld - (Game.Ping - MenuConfig.Qd));
            }
        }
        private static void CancelAnimation()
        {
            if (MenuConfig.QReset)
            {
                EloBuddy.Player.DoEmote(Emote.Dance);
            }
            else if (MenuConfig.Qstrange && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                if (MenuConfig.AnimDance) EloBuddy.Player.DoEmote(Emote.Dance);
                if (MenuConfig.AnimLaugh) EloBuddy.Player.DoEmote(Emote.Laugh);
                if (MenuConfig.AnimTaunt) EloBuddy.Player.DoEmote(Emote.Taunt);
                if (MenuConfig.AnimTalk) EloBuddy.Player.DoEmote(Emote.Joke);
            }
            Orbwalker.ResetAutoAttack();
        }
    }
}
