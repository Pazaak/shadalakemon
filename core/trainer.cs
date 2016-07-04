using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
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
