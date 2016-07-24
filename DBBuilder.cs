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

            File.WriteAllText(System.IO.Directory.GetCurrentDirectory()+@"\data\energies", ser);
        }

        public static void pokemon()
        {
            movement[] moveset = new movement[1]; // Movement list
            moveset[0] = new movement(new int[7] { 0, 1, 0, 0, 0, 0, 0 }, 1, 0, "Slap", 20, 0); // Creates a movement

            movement[] moveset2 = new movement[2];
            moveset2[0] = new movement(new int[7] { 0, 1, 0, 0, 0, 0, 0 }, 1, 2, "Bubble", 10, 1);
            moveset2[1] = new movement(new int[7] { 1, 1, 0, 0, 0, 0, 0 }, 1, 3, "Withdraw", Legacies.fog, 2);

            movement[] moveset1 = new movement[2];
            moveset1[0] = new movement(new int[7] { 0, 2, 0, 0, 0, 0, 0 }, 1, 1, "Recover", 1, 0);
            moveset1[1] = new movement(new int[7] { 2, 1, 0, 0, 0, 0, 0 }, 1, 2, "Star Freeze", 20, 1);

            movement[] moveset3 = new movement[2];
            moveset3[0] = new movement(new int[7] { 1, 1, 0, 0, 0, 0, 0 }, 1, 3, "Withdraw", Legacies.fog, 2);
            moveset3[1] = new movement(new int[7] { 2, 1, 0, 0, 0, 0, 0 }, 1, 0, "Bite", 40, 0);

            movement[] moveset4 = new movement[1];
            moveset4[0] = new movement(new int[7] { 0, 3, 0, 0, 0, 0, 0 }, 1, 4, "Hydro Pump", 40, 20);

            movement[] poliwag = new movement[1];
            poliwag[0] = new movement(new int[7] { 0, 1, 0, 0, 0, 0, 0 }, 1, 4, "Water Gun", 10, 20);

            movement[] poliwhirl = new movement[2];
            poliwhirl[0] = new movement(new int[7] { 0, 2, 0, 0, 0, 0, 0 }, 1, 5, "Amnesia", 0, 0);
            poliwhirl[1] = new movement(new int[7] { 1, 2, 0, 0, 0, 0, 0 }, 1, 6, "Double Slap", 30, 2);

            movement[] poliwrath = new movement[2];
            poliwrath[0] = new movement(new int[7] { 1, 2, 0, 0, 0, 0, 0 }, 1, 4, "Water Gun", 30, 20);
            poliwrath[1] = new movement(new int[7] { 2, 2, 0, 0, 0, 0, 0 }, 1, 7, "Whirlpool", 40, 1);

            Power rainDance = new Power("Rain Dance", 0, 1, 0); // Creates a power

            battler[] listBattlers = new battler[69];
            listBattlers[1] = new battler(1, Constants.TWater, 100, Constants.TElectric, 2, 0, 0, 3, "Blastoise", 9, 8, moveset4, rainDance);
            listBattlers[12] = new battler(1, Constants.TWater, 90, Constants.TGrass, 2, 0, 0, 3, "Poliwrath", 62, 61, poliwrath, null);
            listBattlers[37] = new battler(1, Constants.TWater, 60, Constants.TGrass, 2, 0, 0, 1, "Poliwhirl", 61, 60, poliwhirl, null);
            listBattlers[41] = new battler(1, Constants.TWater, 70, Constants.TElectric, 2, 0, 0, 1, "Wartortle", 8, 7, moveset3, null);
            listBattlers[58] = new battler(0, Constants.TWater, 40, Constants.TGrass, 2, 0, 0, 1, "Poliwag", 60, -1, poliwag, null);
            listBattlers[62] = new battler(0, Constants.TWater, 40, Constants.TElectric, 2, 0, 0, 1, "Squirtle", 7, -1, moveset2, null);
            listBattlers[63] = new battler(1, Constants.TWater, 60, Constants.TElectric, 2, 0, 0, 1, "Starmie", 121, 120, moveset1, null);
            listBattlers[64] = new battler(0, Constants.TWater, 40, Constants.TElectric, 2, 0, 0, 1, "Staryu", 120, -1, moveset, null);
            

            MemoryStream stream1 = new MemoryStream();
            string ser = JsonConvert.SerializeObject(listBattlers, Formatting.Indented);

            File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + @"\data\pokemon", ser);
        }
    }
}
