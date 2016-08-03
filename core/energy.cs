using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    /* This class holds information about the type of the energy
     *  type- Indicates of which type the energy card is
     *  elem- Indicates which type of energy the card gives
     *      0- Colorless
     *      1- Water
     *      2- Fire
     *      3- Grass
     *      4- Psychic
     *      5- Fighting
     *      6- Lightning
     *  quan- Indicates how many energies of the selected type it provides
     *  name- Name of the energy card
     */
    public class energy : card
    {
        public int type, elem, quan;
        public string name;
        public bool proxy;
        public card attached;

        public energy(int type, int elem, int quan, string name, card attached = null)
        {
            this.type = type;
            this.elem = elem;
            this.quan = quan;
            this.name = name;
            if (attached == null)
                this.proxy = false;
            else
            {
                this.proxy = true;
                this.attached = attached;
            }
        }

        public int getSuperType()
        {
            return (2);
        }

        public override string ToString()
        {
            return name;
        }

        // Indicates in a string which is the production of the energy object
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

        public energy DeepCopy()
        {
            return new energy(this.type, this.elem, this.quan, this.name);
        }
    }
}
