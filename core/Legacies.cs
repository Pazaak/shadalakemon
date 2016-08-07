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
        public static int lowThreshold = 5; // Prevents damage below the given threshold
        public static int damageReduction = 6; // Reduce the damage of done by the opponent

        // Pokebodies
        public static int energyBurn = 100; // Energy burn
        public static int clefairyDoll = 101; // Kinda a pokebody
        public static int counter = 102; // Counter when damaged

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
                case 5:
                    return " is protected against weak attacks.";
                case 6:
                    return " has its defenses increased.";
                default:
                    return " PANIC PANIC PANIC.";
            }
        }
    }

    
}
