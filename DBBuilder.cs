﻿using System;
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

            movement[] magikarp = new movement[2];
            magikarp[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 1, 0, "Tackle", 10, 0);
            magikarp[1] = new movement(new int[7] { 0, 1, 0, 0, 0, 0, 0 }, 1, 8, "Flail", 0, 0);

            movement[] gyarados = new movement[2];
            gyarados[0] = new movement(new int[7] { 0, 3, 0, 0, 0, 0, 0 }, 1, 0, "Dragon Rage", 50, 0);
            gyarados[1] = new movement(new int[7] { 0, 4, 0, 0, 0, 0, 0 }, 1, 2, "Bubble Beam", 40, 1);

            movement[] seel = new movement[1];
            seel[0] = new movement(new int[7] { 0, 1, 0, 0, 0, 0, 0 }, 1, 0, "Headbutt", 10, 0);

            movement[] dewgong = new movement[2];
            dewgong[0] = new movement(new int[7] { 1, 2, 0, 0, 0, 0, 0 }, 1, 0, "Aurora Beam", 50, 0);
            dewgong[1] = new movement(new int[7] { 2, 2, 0, 0, 0, 0, 0 }, 1, 2, "Ice Beam", 30, 1);

            movement[] rattata = new movement[1];
            rattata[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, 0, "Bite", 20, 0);

            movement[] pidgey = new movement[1];
            pidgey[0] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 0 }, 0, 9, "Whirlwind", 10, 0);

            movement[] dodou = new movement[1];
            dodou[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, 6, "Fury Attack", 10, 2);

            movement[] raticate = new movement[2];
            raticate[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, 0, "Bite", 20, 0);
            raticate[1] = new movement(new int[7] { 3, 0, 0, 0, 0, 0, 0 }, 0, 10, "Super Fang", 0, 0);

            movement[] porygon = new movement[2];
            porygon[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, 11, "Conversion 1", 0, 0);
            porygon[1] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 0 }, 0, 12, "Conversion 2", 0, 0);

            movement[] farfetchd = new movement[2];
            farfetchd[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, 13, "Leek Slap", 30, 1);
            farfetchd[1] = new movement(new int[7] { 3, 0, 0, 0, 0, 0, 0 }, 0, 0, "Pot Smash", 30, 0);

            movement[] dratini = new movement[1];
            dratini[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, 0, "Pound", 10, 0);

            movement[] pidgeotto = new movement[2];
            pidgeotto[0] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 0 }, 0, 9, "Whirlwind", 20, 0);
            pidgeotto[1] = new movement(new int[7] { 3, 0, 0, 0, 0, 0, 0 }, 0, 14, "Mirror Move", 1, 0);

            movement[] dragonair = new movement[2];
            dragonair[0] = new movement(new int[7] { 3, 0, 0, 0, 0, 0, 0 }, 0, 6, "Slam", 30, 2);
            dragonair[1] = new movement(new int[7] { 4, 0, 0, 0, 0, 0, 0 }, 0, 7, "Hyper Beam", 20, 1);

            Power rainDance = new Power("Rain Dance", 0, 1, 0); // Creates a power

            battler[] listBattlers = new battler[69];
            listBattlers[1] = new battler(1, Constants.TWater, 100, Constants.TElectric, 2, -1, 0, 3, "Blastoise", 9, 8, moveset4, rainDance);
            listBattlers[5] = new battler(1, Constants.TWater, 100, Constants.TGrass, 2, Constants.TFighting, 30, 3, "Gyarados", 130, 129, gyarados, null);
            listBattlers[12] = new battler(1, Constants.TWater, 90, Constants.TGrass, 2, -1, 0, 3, "Poliwrath", 62, 61, poliwrath, null);
            listBattlers[17] = new battler(1, Constants.TNormal, 80, Constants.TNone, 1, Constants.TPsychic, 30, 2, "Dragonair", 148, 147, dragonair, null);
            listBattlers[22] = new battler(1, Constants.TNormal, 60, Constants.TElectric, 2, Constants.TFighting, 30, 1, "Pidgeotto", 17, 16, pidgeotto, null);
            listBattlers[24] = new battler(1, Constants.TWater, 80, Constants.TElectric, 2, -1, 0, 3, "Dewgong", 87, 86, dewgong, null);
            listBattlers[25] = new battler(0, Constants.TNormal, 40, Constants.TNone, 1, Constants.TPsychic, 30, 1, "Dratini", 147, -1, dratini, null);
            listBattlers[26] = new battler(0, Constants.TNormal, 50, Constants.TElectric, 2, Constants.TFighting, 30, 1, "Farfetch'd", 83, -1, farfetchd, null);
            listBattlers[34] = new battler(0, Constants.TWater, 30, Constants.TElectric, 2, -1, 0, 1, "Magikarp", 129, -1, magikarp, null);
            listBattlers[37] = new battler(1, Constants.TWater, 60, Constants.TGrass, 2, -1, 0, 1, "Poliwhirl", 61, 60, poliwhirl, null);
            listBattlers[38] = new battler(0, Constants.TNormal, 30, Constants.TFighting, 2, Constants.TPsychic, 30, 1, "Porygon", 137, -1, porygon, null);
            listBattlers[39] = new battler(1, Constants.TNormal, 60, Constants.TFighting, 2, Constants.TPsychic, 30, 1, "Raticate", 20, 19, raticate, null);
            listBattlers[40] = new battler(0, Constants.TWater, 60, Constants.TElectric, 2, -1, 0, 1, "Seel", 86, -1, seel, null);
            listBattlers[41] = new battler(1, Constants.TWater, 70, Constants.TElectric, 2, -1, 0, 1, "Wartortle", 8, 7, moveset3, null);
            listBattlers[47] = new battler(0, Constants.TNormal, 50, Constants.TElectric, 2, Constants.TFighting, 30, 0, "Dodou", 84, -1, dodou, null);
            listBattlers[56] = new battler(0, Constants.TNormal, 40, Constants.TElectric, 2, Constants.TFighting, 30, 1, "Pidgey", 16, -1, pidgey, null);
            listBattlers[58] = new battler(0, Constants.TWater, 40, Constants.TGrass, 2, -1, 0, 1, "Poliwag", 60, -1, poliwag, null);
            listBattlers[60] = new battler(0, Constants.TNormal, 30, Constants.TFighting, 2, Constants.TPsychic, 30, 0, "Rattata", 19, -1, rattata, null);
            listBattlers[62] = new battler(0, Constants.TWater, 40, Constants.TElectric, 2, -1, 0, 1, "Squirtle", 7, -1, moveset2, null);
            listBattlers[63] = new battler(1, Constants.TWater, 60, Constants.TElectric, 2, -1, 0, 1, "Starmie", 121, 120, moveset1, null);
            listBattlers[64] = new battler(0, Constants.TWater, 40, Constants.TElectric, 2, -1, 0, 1, "Staryu", 120, -1, moveset, null);
            

            MemoryStream stream1 = new MemoryStream();
            string ser = JsonConvert.SerializeObject(listBattlers, Formatting.Indented);

            File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + @"\data\pokemon", ser);
        }
    }
}
