using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    /*
     * Holds the information regarding a power
     *  effect- Index of the effect
     *  parameter1, parameter2- Parameters of the effect
     *  name- Name of the power
     *  active- States if the power can be used or not
     */
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

        // Executes the effect
        public void Execute(Player source_controller, battler source)
        {
            utils.Logger.Report(source.ToString() + " uses " + this.name + ".");
            effects.power_selector(source_controller, source, effect, parameter1, parameter2);
        }

        public Power DeepCopy()
        {
            return new Power(this.name, this.effect, this.parameter1, this.parameter2);
        }
    }
}
