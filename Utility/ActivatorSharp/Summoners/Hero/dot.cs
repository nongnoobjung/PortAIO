using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

namespace Activators.Summoners
{
    internal class dot : CoreSum
    {
        internal override string Name => "summonerdot";
        internal override string DisplayName => "Ignite";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => 600f;
        internal override int Duration => 100;

        public override void OnTick(EventArgs args)
        {
            if (!Menu["use" + Name].Cast<CheckBox>().CurrentValue || !IsReady())
                return;

            foreach (var tar in Activator.Heroes)
            {
                if (!tar.Player.LSIsValidTarget(600))
                {
                    continue;
                }

                if (Activator.smenu[Parent.UniqueMenuId + "useon" + tar.Player.NetworkId] == null)
                {
                    continue;
                }

                if (tar.Player.HasBuff("kindredrnodeathbuff") || tar.Player.IsZombie || tar.Player.HasBuff("summonerdot"))
                {
                    continue;
                }

                if (!Activator.smenu[Parent.UniqueMenuId + "useon" + tar.Player.NetworkId].Cast<CheckBox>().CurrentValue)
                    continue;

                // ignite damagerino
                var ignotedmg = (float) Player.GetSummonerSpellDamage(tar.Player, Damage.SummonerSpell.Ignite);

                // killsteal ignite
                if (Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 0)
                {
                    if (tar.Player.Health <= ignotedmg)
                        UseSpellOn(tar.Player);
                }

                // combo ignite
                if (Menu["mode" + Name].Cast<ComboBox>().CurrentValue == 1)
                {
                    var totaldmg = 0d;
                    var finaldmg = 0d;
                    switch (Player.ChampionName)
                    {
                        case "Ahri":
                            if (!tar.Player.HasBuffOfType(BuffType.Charm) &&
                                Menu["ii" + Player.ChampionName].Cast<CheckBox>().CurrentValue &&
                                Player.GetSpell(SpellSlot.E).State != SpellState.NotLearned)
                                continue;
                            break;
                        case "Cassiopeia":
                            if (!tar.Player.HasBuffOfType(BuffType.Poison) &&
                                Menu["ii" + Player.ChampionName].Cast<CheckBox>().CurrentValue &&
                                Player.GetSpell(SpellSlot.E).State != SpellState.NotLearned)
                                continue;

                            totaldmg += Player.GetSpell(SpellSlot.E).State == SpellState.Ready
                                ? Player.LSGetSpellDamage(tar.Player, SpellSlot.E) * 3
                                : 0;

                            break;
                        case "Diana":
                            if (!tar.Player.HasBuff("dianamoonlight") &&
                                Menu["ii" + Player.ChampionName].Cast<CheckBox>().CurrentValue &&
                                Player.GetSpell(SpellSlot.Q).State != SpellState.NotLearned)
                                continue;

                            totaldmg += Player.GetSpell(SpellSlot.E).State == SpellState.Ready
                                ? Player.LSGetSpellDamage(tar.Player, SpellSlot.R)
                                : 0;
                            break;
                    }

                    // aa dmg
                    totaldmg += Orbwalking.InAutoAttackRange(tar.Player)
                        ? Player.LSGetAutoAttackDamage(tar.Player, true) * 3
                        : 0;

                    // combo damge
                    totaldmg +=
                        Data.Somedata.DamageLib.Sum(
                            entry =>
                                Player.GetSpell(entry.Value).IsReady(2)
                                    ? entry.Key(Player, tar.Player, Player.GetSpell(entry.Value).Level - 1)
                                    : 0);

                    finaldmg = totaldmg * Menu["idmgcheck"].Cast<Slider>().CurrentValue / 100;

                    if (finaldmg + ignotedmg >= tar.Player.Health)
                    {
                        var nearTurret =
                            ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(
                                x => !x.IsDead && x.IsValid && x.Team == tar.Player.Team && tar.Player.LSDistance(x.Position) <= 1250);
                        
                        if (nearTurret != null && Menu["itu"].Cast<CheckBox>().CurrentValue && Player.Level <= Menu["igtu"].Cast<Slider>().CurrentValue)
                        {
                            if (Player.CountAlliesInRange(750) == 0 && (totaldmg + ignotedmg / 1.85) < tar.Player.Health)
                                continue;
                        }

                        if (Orbwalking.InAutoAttackRange(tar.Player) && tar.Player.CountAlliesInRange(350) > 1)
                        {
                            if (totaldmg + ignotedmg / 2.5 >= tar.Player.Health)
                                continue;
                        }

                        if (tar.Player.Level <= 4 &&
                            tar.Player.InventoryItems.Any(item => item.Id == (ItemId) 2003 || item.Id == (ItemId) 2010))
                        {
                            continue;
                        }

                        UseSpellOn(tar.Player, true);
                    }
                }
            }
        }
    }
}
