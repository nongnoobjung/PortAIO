using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace ARAMDetFull
{
    class SummonerItems
    {
        private AIHeroClient player;
        private Spellbook sumBook;
        private SpellSlot ignite;
        private SpellSlot smite;


        public enum ItemIds
        {
            //MuramanaDe = 3043,
            Muramana = 3042,
            Tiamat = 3077,
            Hydra = 3074,
            MercScim = 3139,
            Hextech = 3146,
            SwordOD = 3131,
            Ghostblade = 3142,
            BotRK = 3153,
            Cutlass = 3144,

            Omen = 3143
        }

        public SummonerItems(AIHeroClient myHero)
        {
            player = myHero;
            sumBook = player.Spellbook;
            ignite = player.GetSpellSlot("summonerdot");
            smite = player.GetSpellSlot("SummonerSmite");
        }

        public void castIgnite(AIHeroClient target)
        {
            if (ignite != SpellSlot.Unknown && sumBook.CanUseSpell(ignite) == SpellState.Ready)
                sumBook.CastSpell(ignite, target);
        }

        public void cast(ItemIds item)
        {
            var itemId = (int)item;
            if (LeagueSharp.Common.Items.CanUseItem(itemId))
                LeagueSharp.Common.Items.UseItem(itemId);
        }

        public void cast(ItemIds item, Obj_AI_Base target)
        {
            var itemId = (int)item;
            if (LeagueSharp.Common.Items.CanUseItem(itemId))
                LeagueSharp.Common.Items.UseItem(itemId, target);
        }
    }
}