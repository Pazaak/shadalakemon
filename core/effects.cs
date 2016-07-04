using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    class effects
    {
        public static int move_selector(int type, int quantity, battler target, int selector)
        {
            switch (selector)
            {
                case 0:
                    return(damage(type, quantity, target));
                default:
                    return (-1);
            }
        }

        public static int damage(int type, int quantity, battler target)
        {
            // TODO: Elemental weakness and resistance
            int output = quantity;

            target.damage += output;

            return output;
        }
    }
}
