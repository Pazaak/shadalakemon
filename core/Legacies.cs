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
        public static int deacMov = 1; // Movement deactivated
        public static int destinyBound = 2; // Kill the opposite if killed
        public static int barrier = 3; // protected from all effects
        public static int blinded = 4; // opponent throws a coin before attacks

        // Pokebodies
        public static int energyBurn = 100; // Energy burn

        public static string IndexToLegacy(int legacy)
        {
            switch (legacy)
            {
                case 0:
                    return " is protected from damage.";
                case 1:
                    return " has a movement deactivated.";
                case 2:
                    return " is under the effects of Destiny Bound.";
                case 3:
                    return " is protected from attack effects.";
                case 4:
                    return " is blinded.";
                default:
                    return " PANIC PANIC PANIC.";
            }
        }
    }

    
}
