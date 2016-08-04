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
        public int type, effect;
        public int[] parameters;
        public string name;

        public trainer(int type, int effect, int[] parameters, string name)
        {
            this.type = type;
            this.effect = effect;
            this.parameters = parameters;
            this.name = name;
        }

        public void execute(Player source_controller, Player target_controller)
        {
            Console.WriteLine(source_controller.ToString() + " uses " + this.ToString());
            utils.Logger.Report(source_controller.ToString() + " uses " + this.ToString());
            effects.trainers(source_controller, target_controller, this, effect, parameters);
        }

        public int getSuperType()
        {
            return (1);
        }

        public override string ToString()
        {
            return this.name;
        }

        public trainer DeepCopy()
        {
            int[] neoParameters = new int[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                neoParameters[i] = parameters[i];

            return new trainer(type, effect, neoParameters, name);
        }
    }
}
