using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using shandakemon.core;

namespace shandakemon.AI
{
    public interface DuelAI
    {
        void SelectActive();
        void SelectBenched();
        void SelectMovement(Player opp);
        void PriceProcedure();
        void KnockoutProcedure();
        battler ChooseBattler(bool a, bool b);
        battler ChooseForDiscard(Player source, int quantity, bool max);
    }
}
