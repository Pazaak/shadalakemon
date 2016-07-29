using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    /* This class holds information about the movements and effects
     *  cost- An array that contains the number of energies of each type needed to execute it
     *  usable- Indicates if the movement is usable
     *  type- Indicates of which type the movement is
     *  quantity1, quantity2- Parameters of the effect
     *  effect- Index of the effect
     *  name- Name of the movement
     */
    public class movement
    {
        public int[] cost;
        public bool usable;
        public int[] parameters;
        public int effect;
        public string name;

        public movement(int[] cost,int effect, string name, int[] parameters)
        {
            this.cost = cost;
            this.parameters = parameters;
            this.effect = effect;
            this.name = name;
            this.usable = false;
        }

        // Call of execution
        public void execute(Player source_controller, Player target_controller, battler source, battler target, bool costless = false)
        {
            utils.Logger.Report(source.ToString() + " uses " + this.name + ".");
            effects.move_selector(source_controller, target_controller, source, target, this, effect, parameters, costless);
        }

        // Outputs an string with the information of the movement
        public override string ToString()
        {
            string collector = "";

            for (int i = 0; i < cost[1]; i++)
                collector += "{W}";

            for (int i = 0; i < cost[2]; i++)
                collector += "{F}";

            for (int i = 0; i < cost[3]; i++)
                collector += "{G}";

            for (int i = 0; i < cost[4]; i++)
                collector += "{P}";

            for (int i = 0; i < cost[5]; i++)
                collector += "{T}";

            for (int i = 0; i < cost[6]; i++)
                collector += "{L}";

            for (int i = 0; i < cost[0]; i++)
                collector += "{C}";

            return (collector + " " + this.name);
        }

        public movement DeepCopy()
        {
            int[] neoCost = new int[cost.Length];

            for (int i = 0; i < neoCost.Length; i++)
                neoCost[i] = cost[i];

            int[] neoParameters = null;
            if (parameters != null)
            {
                neoParameters = new int[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                    neoParameters[i] = parameters[i];
            }

            return new movement(neoCost, this.effect, this.name, neoParameters);
        }
    }
}
