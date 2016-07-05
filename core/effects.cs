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
            int output = quantity;
            if (type == target.weak_elem) output *= target.weak_mod;
            else if (type == target.res_elem) output -= target.res_mod;

            target.damage += output;

            return output;
        }
    }
}
