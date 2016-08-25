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
        void DiscardEnergy(Player caller, Player t_cont, battler target, int type, int quantity);
        void DeactivateMovement(Player caller, battler target);
        void Wheel(Player target);
        void ChangeWeakness(Player opp, battler target);
        void ChangeResistance(Player opp, battler target);
        void CastMovement(Player opp);
        bool MainPhase(Player opp);
        battler ChooseBattler(bool a, bool b);
        battler ChooseForDiscard(Player source, int quantity, bool max);
    }
}
