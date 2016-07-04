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
        int type, damage, effect;
        string name;

        public movement(int[] cost, int type, int damage, int effect, string name)
        {
            this.cost = cost;
            this.type = type;
            this.damage = damage;
            this.effect = effect;
            this.name = name;
            this.usable = false;
        }

        public int execute(battler target)
        {
            return(effects.move_selector(type, damage, target ,effect));
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

            return (collector + " " + this.name + " " + this.damage);
        }
    }
}
