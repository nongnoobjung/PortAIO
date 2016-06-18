#region

using System;
using EloBuddy;
using LeagueSharp.SDK;

#endregion

namespace Infected_Twitch.Core
{
    internal class Dmg
    {
        public static int IgniteDmg = 50 + 20 * GameObjects.Player.Level;


        public static float EDamage(Obj_AI_Base target)
        {
            if (!Spells.E.IsReady()) return 0;
            if (target == null || !target.LSIsValidTarget()) return 0;
            if (target.IsInvulnerable || target.HasBuff("KindredRNoDeathBuff") || target.HasBuffOfType(BuffType.SpellShield)) return 0;

            float eDmg = 0;

            eDmg = eDmg + ERaw(target) + Passive(target);

            if (GameObjects.Player.HasBuff("SummonerExhaust")) eDmg = eDmg * 0.6f;

            return eDmg;
        }

        public static float ERaw(Obj_AI_Base target)
        {
            return (float)GameObjects.Player.CalculateDamage(target, DamageType.True,
                EStackDamage[Spells.E.Level - 1] * Stacks(target) +
                0.2 * GameObjects.Player.FlatMagicDamageMod +
                0.25 * GameObjects.Player.FlatPhysicalDamageMod +
                EBaseDamage[Spells.E.Level - 1]);
        }

        public static float[] EStackDamage = { 15f, 20f, 25f, 30f, 35f };

        public static float[] EBaseDamage = { 20, 35, 50, 65, 80 };

        public static float Passive(Obj_AI_Base target)
        {
            float dmg = 6;

            if (GameObjects.Player.Level > 16) dmg = 6;
            if (GameObjects.Player.Level > 12) dmg = 5;
            if (GameObjects.Player.Level > 8) dmg = 4;
            if (GameObjects.Player.Level > 4) dmg = 3;
            if (GameObjects.Player.Level > 0) dmg = 2;

            return dmg * Stacks(target) * PassiveTime(target) - target.HPRegenRate * PassiveTime(target);
        }

        public static float Stacks(Obj_AI_Base target)
        {
            return target.GetBuffCount("TwitchDeadlyVenom");
        }

        public static float PassiveTime(Obj_AI_Base target)
        {
            if (!target.HasBuff("twitchdeadlyvenom")) return 0;

            return Math.Max(0, target.GetBuff("twitchdeadlyvenom").EndTime) - Game.Time;
        }

        public static bool Executable(AIHeroClient target)
        {
            return !target.IsInvulnerable && target.Health < EDamage(target);
        }
    }
}