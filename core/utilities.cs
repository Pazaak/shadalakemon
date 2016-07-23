using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    /*
     * Class that holds several utilities to be used in different parts of the proyect
     */
    class utilities
    {
        // Given a number returns the associated string with the type
        public static string numToType(int type)
        {
            switch (type)
            {
                case 0: return "Normal";
                case 1: return "Water";
                case 2: return "Fire";
                case 3: return "Grass";
                case 4: return "Psychic";
                case 5: return "Fightning";
                case 6: return "Lightning";
                default: return "Any";

            }
        }

        // Given an status alignment, return the name of that status
        public static string numToStatus(int type)
        {
            switch (type)
            {
                case 0: return "normal";
                case 1: return "paralyzed";
                default: return "Doge";

            }
        }
    }
}
