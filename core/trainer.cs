using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    /*
     * Class that holds the data relative to trainer cards
     *  type- The type of trainer card it is
     *  effect- The index of the effect it triggers
     *  quantity- Parameter for the effect
     *  name- Name of the card
     */
    class trainer : card
    {
        public int type, effect, quantity;
        string name;

        public trainer(int type, int effect, int quantity, string name)
        {
            this.type = type;
            this.effect = effect;
            this.quantity = quantity;
            this.name = name;
        }

        public void execute() { }

        public int getSuperType()
        {
            return (1);
        }
    }
}
