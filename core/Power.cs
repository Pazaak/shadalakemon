using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    public class Power
    {
        public int parameter1, parameter2, effect;
        public string name;
        public bool active;

        public Power(string name, int effect, int parameter1, int parameter2)
        {
            this.name = name;
            this.effect = effect;
            this.parameter1 = parameter1;
            this.parameter2 = parameter2;
            this.active = true;
        }

        public void Execute(Player source_controller, battler source)
        {
            effects.power_selector(source_controller, source, effect, parameter1, parameter2);
        }
    }
}
