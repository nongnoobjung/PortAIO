using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace YasuoSharpV2
{
    
    public class TargetedSpell
    {
        public AIHeroClient owner;
        public SpellDataInst spellInst;
        public SpellSlot slot;
        public String name;
        public String missleName;
    }

    public class TargetedMissle
    {
        public MissileClient missle;
        public int blockBelowHP;
    }

    public class TargetedSpellManager
    {
        public static List<TargetedSpell> blockTargetedSpells = new List<TargetedSpell>();

        public static List<TargetedMissle> targatedMissales = new List<TargetedMissle>(); 


        public static void setUp(Menu m)
        {
            m.AddGroupLabel("Targeted spells");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsEnemy))
            {
                m.AddLabel(enemy.ChampionName);
                foreach (var spell in enemy.Spellbook.Spells.Where(spl => (spl.SData.TargettingType == SpellDataTargetType.Unit || spl.SData.TargettingType == SpellDataTargetType.SelfAndUnit) && spl.SData.MissileSpeed >300))
                {

                    if(spell.Slot != SpellSlot.Q && spell.Slot != SpellSlot.W && spell.Slot != SpellSlot.E && spell.Slot != SpellSlot.R)
                        continue;


                    //All targeted spells
                    blockTargetedSpells.Add(new TargetedSpell()
                    {
                        owner = enemy,
                        spellInst = spell,
                        slot = spell.Slot,
                        name = spell.SData.DisplayName,
                        missleName = spell.SData.MissileBoneName
                    });

                    m.Add("block_" + enemy.ChampionName + "_" + spell.Name, new Slider(" " + enemy.ChampionName + ": "+ spell.Slot + ": % HP", 100));
                }
            }
        }

        public static int blockSpellOnHP(String champName, String spellName)
        {
            var mItemName = "block_" + champName + "_" + spellName;
            if (YasuoSharp.smartW[mItemName] != null)
            {
                return YasuoSharp.smartW[mItemName].Cast<Slider>().CurrentValue;
            }
            return 0;
        }

        public static void deleteActiveSpell(int netId)
        {
        }

        public static void addActiveSpell(MissileClient spell)
        {
        }

    }

}
