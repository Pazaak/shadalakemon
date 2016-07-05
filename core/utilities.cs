using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    class utilities
    {

        public static string numToType(int type)
        {
            switch (type)
            {
                case 0: return "Colorless";
                case 1: return "Water";
                case 2: return "Fire";
                case 3: return "Grass";
                case 4: return "Psychic";
                case 5: return "Fightning";
                case 6: return "Lightning";
                default: return "Doge";

            }
        }
    }
}
