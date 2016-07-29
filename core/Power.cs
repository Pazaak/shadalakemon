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
        public int[] parameters;
        public int effect;
        public string name;
        public bool active;

        public Power(string name, int effect, int[] parameters)
        {
            this.name = name;
            this.effect = effect;
            this.parameters = parameters;
            this.active = true;
        }

        // Executes the effect
        public void Execute(Player source_controller, battler source)
        {
            utils.Logger.Report(source.ToString() + " uses " + this.name + ".");
            effects.power_selector(source_controller, source, effect, parameters);
        }

        public Power DeepCopy()
        {
            int[] neoParameters = null;

            if (parameters != null)
            {
                neoParameters = new int[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                    neoParameters[i] = parameters[i];
            }

            return new Power(this.name, this.effect, neoParameters);
        }
    }
}
