#region

using System;
using System.Linq;
using Infected_Twitch.Core;
using Infected_Twitch.Menus;
using EloBuddy;
using LeagueSharp.SDK;
using SharpDX;

#endregion

namespace Infected_Twitch.Event
{
    internal class DrawDmg : Core.Core
    {
        private static readonly HpBarIndicator DrawHpBar = new HpBarIndicator();
        public static void Draw(EventArgs args)
        {
            if(Player.IsDead || !MenuConfig.DrawDmg) return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.LSIsValidTarget(1200) && !x.IsZombie))
            {
                var easyKill = Spells.E.IsReady() && Dmg.Executable(enemy)
                      ? new ColorBGRA(0, 255, 0, 120)
                      : new ColorBGRA(255, 255, 0, 120);

                DrawHpBar.unit = enemy;
                DrawHpBar.drawDmg(Dmg.EDamage(enemy), easyKill);
            }
        }
    }
}
