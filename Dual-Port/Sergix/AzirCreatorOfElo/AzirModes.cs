using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir_Creator_of_Elo
{
    class AzirModes : Modes
    {
        public JumpLogic jump;
        public AzirModes(AzirMain azir)
        {
            jump = new JumpLogic(azir);
        }
        public override void Update(AzirMain azir)
        {

            base.Update(azir);



            if (Menu._jumpMenu["fleekey"].Cast<KeyBind>().CurrentValue)
            {
                Jump(azir);
            }

            if (Menu._jumpMenu["inseckey"].Cast<KeyBind>().CurrentValue)
            {
                Insec(azir);
            }
        }
        public void Insec(AzirMain azir)
        {
            var ts = TargetSelector.GetTarget(900, DamageType.Magical);
            if (ts != null)
            {
                jump.insec(ts);

            }
        }
        public void Jump(AzirMain azir)
        {
            jump.updateLogic(Game.CursorPos);

        }

        public override void Harash(AzirMain azir)
        {

            var wCount = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Ammo;
            var useQ = Menu._harashMenu["HQ"].Cast<CheckBox>().CurrentValue;
            var useW = Menu._harashMenu["HW"].Cast<CheckBox>().CurrentValue;
            var savew = Menu._harashMenu["HW2"].Cast<CheckBox>().CurrentValue;
            var nSoldiersToQ = Menu._harashMenu["hSoldiersToQ"].Cast<Slider>().CurrentValue;
            base.Harash(azir);
            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            if (target != null)
            {

                checkauto(azir, target);
                if (target.Distance(azir.Hero.ServerPosition) < 450)
                {
                    var pred = azir.Spells.W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        if (useW && !(savew && (wCount == 0)))
                            azir.Spells.W.Cast(pred.CastPosition);
                    }
                }
                else
                {
                    if (azir.Spells.Q.Level > 0 && azir.Spells.Q.IsReady())
                        if (useW)
                            azir.Spells.W.Cast(azir.Hero.Position.LSExtend(target.ServerPosition, 450));
                }
                azir.Spells.castQ(azir, target, useQ, nSoldiersToQ);
            }
        }
        public override void Laneclear(AzirMain azir)
        {
            var useQ = Menu._laneClearMenu["LQ"].Cast<CheckBox>().CurrentValue;
            var useW = Menu._laneClearMenu["LW"].Cast<CheckBox>().CurrentValue;
            base.Laneclear(azir);
            var minion = MinionManager.GetMinions(azir.Spells.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minion != null)
            {
                checkautoMin(azir, minion);
                if (azir.Spells.W.IsInRange(minion))
                {
                    var pred = azir.Spells.W.GetPrediction(minion);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        if (useW)
                            azir.Spells.W.Cast(pred.CastPosition);
                    }
                    if (azir.soldierManager.SoldiersAttacking(azir) == false && azir.soldierManager.ActiveSoldiers.Count > 0)
                    {
                        pred = azir.Spells.Q.GetPrediction(minion);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            if (useQ)
                                azir.Spells.Q.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }
        public override void Jungleclear(AzirMain azir)
        {
            var useW = Menu._JungleClearMenu["JW"].Cast<CheckBox>().CurrentValue;
            base.Jungleclear(azir);
            var minion = MinionManager.GetMinions(azir.Spells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minion != null)
            {
                checkautoMin(azir, minion);
                if (azir.Spells.W.IsInRange(minion))
                {
                    var pred = azir.Spells.W.GetPrediction(minion);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        if (useW)
                            azir.Spells.W.Cast(pred.CastPosition);
                    }


                }
            }
        }
        public void checkauto(AzirMain azir, AIHeroClient target)
        {
            foreach (var soldier in azir.soldierManager.ActiveSoldiers)
            {
                if (soldier.LSDistance(target) <= 325)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, target);
                }
            }
        }
        public void checkautoMin(AzirMain azir, Obj_AI_Base target)
        {
            foreach (var soldier in azir.soldierManager.ActiveSoldiers)
            {
                if (soldier.LSDistance(target) <= 325)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, target);
                }
            }
        }
        public override void Combo(AzirMain azir)
        {

            var useQ = Menu._comboMenu["CQ"].Cast<CheckBox>().CurrentValue;
            var useW = Menu._comboMenu["CW"].Cast<CheckBox>().CurrentValue;
            var nSoldiersToQ = Menu._comboMenu["SoldiersToQ"].Cast<Slider>().CurrentValue;
            base.Combo(azir);
            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            if (target != null)
            {
                checkauto(azir, target);
                if (target.Distance(azir.Hero.ServerPosition) < 450)
                {
                    if (target.isRunningOfYou())
                    {
                        var pos = LeagueSharp.Common.Prediction.GetPrediction(target, 0.8f).UnitPosition;
                        azir.Spells.W.Cast(pos);
                    }
                    else
                    {
                        var pred = azir.Spells.W.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.Medium)
                        {
                            if (useW)
                                azir.Spells.W.Cast(pred.CastPosition);
                        }
                    }
                }
                else
                {
                    if (azir.Spells.Q.Level > 0 && azir.Spells.Q.IsReady())
                        if (useW)
                            azir.Spells.W.Cast(azir.Hero.Position.LSExtend(target.ServerPosition, 450));
                }
                azir.Spells.castQ(azir, target, useQ, nSoldiersToQ);

            }

        }
    }
}
