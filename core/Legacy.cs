using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    class Legacy
    {
        string name;
        int phase, effect, quantity1, quantity2;
        battler source;

        // Phases:
        // 0: Opp. Battle phase

        public Legacy(string name, int phase, int effect, battler source, int quantity1, int quantity2)
        {
            this.name = name;
            this.phase = phase;
            this.effect = effect;
            this.quantity1 = quantity1;
            this.quantity2 = quantity2;
            this.source = source;
        }

        public void execute()
        {
            effects.legacy_selector(effect, source, quantity1, quantity2);
        }
    }
}
