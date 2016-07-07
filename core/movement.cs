using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    public class movement
    {
        public int[] cost;
        public bool usable;
        int type, quantity1, quantity2, effect;
        string name;

        public movement(int[] cost, int type, int effect, string name, int quantity1, int quantity2)
        {
            this.cost = cost;
            this.type = type;
            this.quantity1 = quantity1;
            this.quantity2 = quantity2;
            this.effect = effect;
            this.name = name;
            this.usable = false;
        }

        public void execute(Player source_controller, Player target_controller, battler source, battler target)
        {
            effects.move_selector(source_controller, target_controller, source, target, type, effect, quantity1, quantity2);
        }

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

            return (collector + " " + this.name + " " + this.quantity1);
        }
    }
}
