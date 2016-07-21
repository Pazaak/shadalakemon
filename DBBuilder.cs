using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using shandakemon.core;
using Newtonsoft.Json;

namespace shandakemon
{
    public class DBBuilder
    {
        public static void energies()
        {
            energy[] listEnergies = new energy[8];
            listEnergies[0] = new energy(1, 0, 2, "Double Colorless Energy");
            listEnergies[1] = new energy(0, 5, 1, "Fighting Energy");
            listEnergies[2] = new energy(0, 5, 1, "Fighting Energy");
            listEnergies[3] = new energy(0, 2, 1, "Fire Energy");
            listEnergies[4] = new energy(0, 3, 1, "Grass Energy");
            listEnergies[5] = new energy(0, 6, 1, "Electric Energy");
            listEnergies[6] = new energy(0, 4, 1, "Psychic Energy");
            listEnergies[7] = new energy(0, 1, 1, "Water Energy");

            MemoryStream stream1 = new MemoryStream();
            string ser = JsonConvert.SerializeObject(listEnergies, Formatting.Indented);

            
        }
    }
}
