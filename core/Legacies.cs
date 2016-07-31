using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    // Specifies the indexes of the effects that the front battle may be suffering
    public static class Legacies
    {
        // Conditions
        public static int fog = 0; // fog effects
        public static int deacMov1 = 1; // Movement 1 deactivated
        public static int deacMov2 = 2; // Movement 2 deactivated
        public static int deacMov3 = 3; // Movement 2 deactivated
        public static int destinyBound = 4; // Kill the opposite if killed
        public static int barrier = 5; // protected from all effects

        // Pokebodies
        public static int energyBurn = 100; // Energy burn

        public static string IndexToLegacy(int legacy)
        {
            switch (legacy)
            {
                case 0:
                    return " is protected from damage.";
                case 1:
                    return " has its first attack deactivated.";
                case 2:
                    return " has its second attack deactivated.";
                case 3:
                    return " has its third attack deactivated.";
                case 4:
                    return " is under the effects of Destiny Bound";
                default:
                    return " PANIC PANIC PANIC.";
            }
        }
    }

    
}
