using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    // 0: Colorless
    // 1: Water
    // 2: Fire
    // 3: Grass
    // 4: Psychic
    // 5: Fighting
    // 6: Lightning
    public class energy : card
    {
        public int type, elem, quan;
        public string name;

        public energy(int type, int elem, int quan, string name)
        {
            this.type = type;
            this.elem = elem;
            this.quan = quan;
            this.name = name;
        }

        public int getSuperType()
        {
            return (2);
        }

        public override string ToString()
        {
            return name;
        }

        public string ToProduction()
        {
            string output = "";

            for (int i = 0; i < quan; i++)
                switch (elem)
                {
                    case 0: output += "C"; break;
                    case 1: output += "W"; break;
                    case 2: output += "F"; break;
                    case 3: output += "G"; break;
                    case 4: output += "P"; break;
                    case 5: output += "T"; break;
                    case 6: output += "L"; break;
                }
            return output;
        }
    }
}
