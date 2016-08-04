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
            energy[] listEnergies = new energy[7];
            listEnergies[0] = new energy(1, 0, 2, "Double Colorless Energy");
            listEnergies[1] = new energy(0, 5, 1, "Fighting Energy");
            listEnergies[2] = new energy(0, 2, 1, "Fire Energy");
            listEnergies[3] = new energy(0, 3, 1, "Grass Energy");
            listEnergies[4] = new energy(0, 6, 1, "Electric Energy");
            listEnergies[5] = new energy(0, 4, 1, "Psychic Energy");
            listEnergies[6] = new energy(0, 1, 1, "Water Energy");

            MemoryStream stream1 = new MemoryStream();
            string ser = JsonConvert.SerializeObject(listEnergies, Formatting.Indented);

            File.WriteAllText(System.IO.Directory.GetCurrentDirectory()+@"\data\energies", ser);
        }

        public static void pokemon()
        {
            #region watertype
            movement[] moveset = new movement[1]; // Movement list
            moveset[0] = new movement(new int[7] { 0, 1, 0, 0, 0, 0, 0 }, 0, "Slap", new int[1] { 20 }); // Creates a movement

            movement[] moveset2 = new movement[2];
            moveset2[0] = new movement(new int[7] { 0, 1, 0, 0, 0, 0, 0 }, 2, "Bubble", new int[2] { 10, 1 });
            moveset2[1] = new movement(new int[7] { 1, 1, 0, 0, 0, 0, 0 }, 3, "Withdraw", new int[2] { Legacies.fog, 2 });

            movement[] moveset1 = new movement[2];
            moveset1[0] = new movement(new int[7] { 0, 2, 0, 0, 0, 0, 0 }, 1, "Recover", new int[2] { 1, 0 });
            moveset1[1] = new movement(new int[7] { 2, 1, 0, 0, 0, 0, 0 }, 2, "Star Freeze", new int[2] { 20, 1 });

            movement[] moveset3 = new movement[2];
            moveset3[0] = new movement(new int[7] { 1, 1, 0, 0, 0, 0, 0 }, 3, "Withdraw", new int[2] { Legacies.fog, 2 });
            moveset3[1] = new movement(new int[7] { 2, 1, 0, 0, 0, 0, 0 }, 0, "Bite", new int[1] { 40 });

            movement[] moveset4 = new movement[1];
            moveset4[0] = new movement(new int[7] { 0, 3, 0, 0, 0, 0, 0 }, 4, "Hydro Pump", new int[3] { 40, 20, Constants.TWater });

            movement[] poliwag = new movement[1];
            poliwag[0] = new movement(new int[7] { 0, 1, 0, 0, 0, 0, 0 }, 4, "Water Gun", new int[3] { 10, 20, Constants.TWater });

            movement[] poliwhirl = new movement[2];
            poliwhirl[0] = new movement(new int[7] { 0, 2, 0, 0, 0, 0, 0 }, 5, "Amnesia", null);
            poliwhirl[1] = new movement(new int[7] { 1, 2, 0, 0, 0, 0, 0 }, 6, "Double Slap", new int[2] { 30, 2 });

            movement[] poliwrath = new movement[2];
            poliwrath[0] = new movement(new int[7] { 1, 2, 0, 0, 0, 0, 0 }, 4, "Water Gun", new int[3] { 30, 20, Constants.TWater });
            poliwrath[1] = new movement(new int[7] { 2, 2, 0, 0, 0, 0, 0 }, 7, "Whirlpool", new int[2] { 40, 1 });

            movement[] magikarp = new movement[2];
            magikarp[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, "Tackle", new int[1] { 10 });
            magikarp[1] = new movement(new int[7] { 0, 1, 0, 0, 0, 0, 0 }, 8, "Flail", null);

            movement[] gyarados = new movement[2];
            gyarados[0] = new movement(new int[7] { 0, 3, 0, 0, 0, 0, 0 }, 0, "Dragon Rage", new int[1] { 50 });
            gyarados[1] = new movement(new int[7] { 0, 4, 0, 0, 0, 0, 0 }, 2, "Bubble Beam", new int[2] { 40, 1 });

            movement[] seel = new movement[1];
            seel[0] = new movement(new int[7] { 0, 1, 0, 0, 0, 0, 0 }, 0, "Headbutt", new int[1] { 10 });

            movement[] dewgong = new movement[2];
            dewgong[0] = new movement(new int[7] { 1, 2, 0, 0, 0, 0, 0 }, 0, "Aurora Beam", new int[1] { 50 });
            dewgong[1] = new movement(new int[7] { 2, 2, 0, 0, 0, 0, 0 }, 2, "Ice Beam", new int[2] { 30, 1 });
            #endregion

            #region normaltype
            movement[] rattata = new movement[1];
            rattata[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, "Bite", new int[1] { 20 });

            movement[] pidgey = new movement[1];
            pidgey[0] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 0 }, 9, "Whirlwind", new int[1] { 10 });

            movement[] dodou = new movement[1];
            dodou[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 6, "Fury Attack", new int[2] { 10, 2 });

            movement[] raticate = new movement[2];
            raticate[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, "Bite", new int[1] { 20 });
            raticate[1] = new movement(new int[7] { 3, 0, 0, 0, 0, 0, 0 }, 10, "Super Fang", null);

            movement[] porygon = new movement[2];
            porygon[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 11, "Conversion 1", null);
            porygon[1] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 0 }, 12, "Conversion 2", null);

            movement[] farfetchd = new movement[2];
            farfetchd[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 13, "Leek Slap", null);
            farfetchd[1] = new movement(new int[7] { 3, 0, 0, 0, 0, 0, 0 }, 0, "Pot Smash", new int[1] { 30 });

            movement[] dratini = new movement[1];
            dratini[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, "Pound", new int[1] { 10 });

            movement[] pidgeotto = new movement[2];
            pidgeotto[0] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 0 }, 9, "Whirlwind", new int[1] { 20 });
            pidgeotto[1] = new movement(new int[7] { 3, 0, 0, 0, 0, 0, 0 }, 14, "Mirror Move", new int[1] { 1 });

            movement[] dragonair = new movement[2];
            dragonair[0] = new movement(new int[7] { 3, 0, 0, 0, 0, 0, 0 }, 6, "Slam", new int[2] { 30, 2 });
            dragonair[1] = new movement(new int[7] { 4, 0, 0, 0, 0, 0, 0 }, 7, "Hyper Beam", new int[2] { 20, 1 });

            movement[] clefairy = new movement[2];
            clefairy[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 16, "Sing", new int[1] { 2 });
            clefairy[1] = new movement(new int[7] { 3, 0, 0, 0, 0, 0, 0 }, 15, "Metronome", null);

            movement[] chansey = new movement[2];
            chansey[0] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 0 }, 3, "Scrunch", new int[2] { Legacies.fog, 2 });
            chansey[1] = new movement(new int[7] { 4, 0, 0, 0, 0, 0, 0 }, 17, "Double-Edge", new int[2] { 80, 80 });
            #endregion

            #region firetype
            movement[] vulpix = new movement[1];
            vulpix[0] = new movement(new int[7] { 0, 0, 2, 0, 0, 0, 0 }, 2, "Confuse Ray", new int[2] { 10, 3 });

            movement[] ponyta = new movement[2];
            ponyta[0] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 0 }, 0, "Smash Kick", new int[1] { 20 });
            ponyta[1] = new movement(new int[7] { 0, 0, 2, 0, 0, 0, 0 }, 0, "Flame Tail", new int[1] { 30 });

            movement[] charmander = new movement[2];
            charmander[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, "Scratch", new int[1] { 10 });
            charmander[1] = new movement(new int[7] { 1, 0, 1, 0, 0, 0, 0 }, 18, "Ember", new int[3] { 30, Constants.TFire, 1 });

            movement[] magmar = new movement[2];
            magmar[0] = new movement(new int[7] { 0, 0, 2, 0, 0, 0, 0 }, 0, "Fire Punch", new int[1] { 30 });
            magmar[1] = new movement(new int[7] { 1, 0, 2, 0, 0, 0, 0 }, 18, "Flamethrower", new int[3] { 50, Constants.TFire, 1 });

            movement[] growlithe = new movement[1];
            growlithe[0] = new movement(new int[7] { 1, 0, 1, 0, 0, 0, 0 }, 0, "Flare", new int[1] { 20 });

            movement[] charmeleon = new movement[2];
            charmeleon[0] = new movement(new int[7] { 3, 0, 0, 0, 0, 0, 0 }, 0, "Slash", new int[1] { 30 });
            charmeleon[1] = new movement(new int[7] { 1, 0, 2, 0, 0, 0, 0 }, 18, "Flamethrower", new int[3] { 50, Constants.TFire, 1 });

            movement[] arcanine = new movement[2];
            arcanine[0] = new movement(new int[7] { 1, 0, 2, 0, 0, 0, 0 }, 18, "Flamethrower", new int[3] { 50, Constants.TFire, 1 });
            arcanine[1] = new movement(new int[7] { 2, 0, 2, 0, 0, 0, 0 }, 17, "Take Down", new int[2] { 80, 30 });

            movement[] ninetales = new movement[2];
            ninetales[0] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 0 }, 19, "Lure", null);
            ninetales[1] = new movement(new int[7] { 0, 0, 4, 0, 0, 0, 0 }, 18, "Fire Blast", new int[3] { 80, Constants.TFire, 1 });

            movement[] charizard = new movement[1];
            charizard[0] = new movement(new int[7] { 0, 0, 4, 0, 0, 0, 0 }, 18, "Fire Spin", new int[3] { 100, Constants.TFire, 2 });
            #endregion

            #region grasstype
            movement[] weedle = new movement[1];
            weedle[0] = new movement(new int[7] { 0, 0, 0, 1, 0, 0, 0 }, 2, "Poison Sting", new int[2] { 10, 10 });

            movement[] tangela = new movement[2];
            tangela[0] = new movement(new int[7] { 1, 0, 0, 1, 0, 0, 0 }, 2, "Bind", new int[2] { 20, 1 });
            tangela[1] = new movement(new int[7] { 0, 0, 0, 3, 0, 0, 0 }, 20, "Poison Sting", new int[2] { 20, 10 });

            movement[] nidoranm = new movement[1];
            nidoranm[0] = new movement(new int[7] { 0, 0, 0, 1, 0, 0, 0 }, 6, "Horn Hazard", new int[2] { 30, 1 });

            movement[] metapod = new movement[2];
            metapod[0] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 0 }, 3, "Stiffen", new int[2] { Legacies.fog, 2 });
            metapod[1] = new movement(new int[7] { 0, 0, 0, 2, 0, 0, 0 }, 2, "Stun Spore", new int[2] { 20, 1 });

            movement[] koffing = new movement[1];
            koffing[0] = new movement(new int[7] { 0, 0, 0, 2, 0, 0, 0 }, 21, "Foul Gas", new int[3] { 10, 10, 3 });

            movement[] caterpie = new movement[1];
            caterpie[0] = new movement(new int[7] { 0, 0, 0, 1, 0, 0, 0 }, 2, "String Shot", new int[2] { 10, 1 });

            movement[] bulbasaur = new movement[1];
            bulbasaur[0] = new movement(new int[7] { 0, 0, 0, 2, 0, 0, 0 }, 22, "Leech Seed", new int[2] { 20, 10 });

            movement[] nidorino = new movement[2];
            nidorino[0] = new movement(new int[7] { 2, 0, 0, 1, 0, 0, 0 }, 6, "Double Kick", new int[2] { 30, 2 });
            nidorino[1] = new movement(new int[7] { 2, 0, 0, 2, 0, 0, 0 }, 0, "Horn Drill", new int[1] { 50 });

            movement[] kakuna = new movement[2];
            kakuna[0] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 0 }, 3, "Stiffen", new int[2] { Legacies.fog, 2 });
            kakuna[1] = new movement(new int[7] { 0, 0, 0, 2, 0, 0, 0 }, 2, "Poisonpowder", new int[2] { 20, 10 });

            movement[] ivysaur = new movement[2];
            ivysaur[0] = new movement(new int[7] { 2, 0, 0, 1, 0, 0, 0 }, 0, "Vine Whip", new int[1] { 30 });
            ivysaur[1] = new movement(new int[7] { 0, 0, 0, 3, 0, 0, 0 }, 20, "Poisonpowder", new int[2] { 20, 10 });

            movement[] beedrill = new movement[2];
            beedrill[0] = new movement(new int[7] { 3, 0, 0, 0, 0, 0, 0 }, 6, "Twineedle", new int[2] { 30, 2 });
            beedrill[1] = new movement(new int[7] { 0, 0, 0, 3, 0, 0, 0 }, 2, "Poison Sting", new int[2] { 40, 10 });

            movement[] venusaur = new movement[1];
            venusaur[0] = new movement(new int[7] { 0, 0, 0, 4, 0, 0, 0 }, 0, "Solar Beam", new int[1] { 60 });

            movement[] nidoking = new movement[2];
            nidoking[0] = new movement(new int[7] { 2, 0, 0, 1, 0, 0, 0 }, 23, "Thrash", new int[3] { 30, 10, 10 });
            nidoking[1] = new movement(new int[7] { 0, 0, 0, 3, 0, 0, 0 }, 20, "Toxic", new int[2] { 20, 11 });
            #endregion

            #region pyschictype
            movement[] gastly = new movement[2];
            gastly[0] = new movement(new int[7] { 0, 0, 0, 0, 1, 0, 0 }, 16, "Sleeping Gas", new int[1] { 2 });
            gastly[1] = new movement(new int[7] { 1, 0, 0, 0, 1, 0, 0 }, 24, "Destiny Bond", new int[4] { Constants.TPsychic, 1, Legacies.destinyBound, 2 });

            movement[] drowzee = new movement[2];
            drowzee[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, "Pound", new int[1] { 10 });
            drowzee[1] = new movement(new int[7] { 0, 0, 0, 0, 2, 0, 0 }, 2, "Confuse Ray", new int[2] { 10, 3 });

            movement[] abra = new movement[1];
            abra[0] = new movement(new int[7] { 0, 0, 0, 0, 1, 0, 0 }, 2, "Psyshock", new int[2] { 10, 1 });

            movement[] kadabra = new movement[2];
            kadabra[0] = new movement(new int[7] { 0, 0, 0, 0, 2, 0, 0 }, 1, "Recover", new int[2] { 1, 0 });
            kadabra[1] = new movement(new int[7] { 1, 0, 0, 0, 2, 0, 0 }, 0, "Super Psy", new int[1] { 50 });

            movement[] jynx = new movement[2];
            jynx[0] = new movement(new int[7] { 0, 0, 0, 0, 1, 0, 0 }, 6, "Doubleslap", new int[2] { 10, 2 });
            jynx[1] = new movement(new int[7] { 1, 0, 0, 0, 2, 0, 0 }, 25, "Meditate", new int[1] { 20 });

            movement[] haunter = new movement[2];
            haunter[0] = new movement(new int[7] { 0, 0, 0, 0, 1, 0, 0 }, 26, "Hypnosis", new int[1] { 2 });
            haunter[1] = new movement(new int[7] { 0, 0, 0, 0, 2, 0, 0 }, 27, "Dream Eater", new int[2] { 50, 2 });

            movement[] mewtwo = new movement[2];
            mewtwo[0] = new movement(new int[7] { 1, 0, 0, 0, 1, 0, 0 }, 28, "Psychic", new int[1] { 10 });
            mewtwo[1] = new movement(new int[7] { 0, 0, 0, 0, 2, 0, 0 }, 24, "Barrier", new int[4] { Constants.TPsychic, 1, Legacies.barrier, 2 });

            movement[] alakazam = new movement[1];
            alakazam[0] = new movement(new int[7] { 0, 0, 0, 0, 3, 0, 0 }, 2, "Confuse Ray", new int[2] { 30, 3 });
            #endregion

            #region fightingtype
            movement[] sandshrew = new movement[1];
            sandshrew[0] = new movement(new int[7] { 0, 0, 0, 0, 0, 1, 0 }, 29, "Sand-attack", new int[3] { 10, Legacies.blinded, 2 });

            movement[] onix = new movement[2];
            onix[0] = new movement(new int[7] { 0, 0, 0, 0, 0, 1, 0 }, 0, "Rock Throw", new int[1] { 10 });
            onix[1] = new movement(new int[7] { 0, 0, 0, 0, 0, 2, 0 }, 30, "Harden", new int[3] { Legacies.lowThreshold, 2, 30 });

            movement[] machop = new movement[1];
            machop[0] = new movement(new int[7] { 0, 0, 0, 0, 0, 1, 0 }, 0, "Low Kick", new int[1] { 20 });

            movement[] digglet = new movement[2];
            digglet[0] = new movement(new int[7] { 0, 0, 0, 0, 0, 1, 0 }, 0, "Dig", new int[1] { 10 });
            digglet[1] = new movement(new int[7] { 0, 0, 0, 0, 0, 2, 0 }, 0, "Mud Slap", new int[1] { 30 });

            movement[] machoke = new movement[2];
            machoke[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 2, 0 }, 31, "Karate Chop", new int[1] { 50 });
            machoke[1] = new movement(new int[7] { 2, 0, 0, 0, 0, 2, 0 }, 17, "Submission", new int[2] { 60, 20 });

            movement[] dugtrio = new movement[2];
            dugtrio[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 2, 0 }, 0, "Slash", new int[1] { 40 });
            dugtrio[1] = new movement(new int[7] { 0, 0, 0, 0, 0, 4, 0 }, 32, "Earthquake", new int[2] { 70, 10 });

            movement[] machamp = new movement[1];
            machamp[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 3, 0 }, 0, "Seismic Toss", new int[1] { 60 });

            movement[] hitmonchan = new movement[2];
            hitmonchan[0] = new movement(new int[7] { 0, 0, 0, 0, 0, 1, 0 }, 0, "Jab", new int[1] { 20 });
            hitmonchan[1] = new movement(new int[7] { 1, 0, 0, 0, 0, 2, 0 }, 0, "Special Punch", new int[1] { 40 });
            #endregion

            #region electrictype
            movement[] voltorb = new movement[1];
            voltorb[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, "Tackle", new int[1] { 10 });

            movement[] pikachu = new movement[2];
            pikachu[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 0 }, 0, "Gnaw", new int[1] { 10 });
            pikachu[1] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 1 }, 23, "Thunder Jolt", new int[3] { 30, 0, 10 });

            movement[] magnemite = new movement[2];
            magnemite[0] = new movement(new int[7] { 0, 0, 0, 0, 0, 0, 1 }, 2, "Thunder Wave", new int[2] { 10, 1 });
            magnemite[1] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 1 }, 33, "Selfdestruct", new int[3] { 40, 10, 40 });

            movement[] electrode = new movement[1];
            electrode[0] = new movement(new int[7] { 0, 0, 0, 0, 0, 0, 3 }, 23, "Electric Shock", new int[3] { 50, 0, 10 });

            movement[] electabuzz = new movement[2];
            electabuzz[0] = new movement(new int[7] { 0, 0, 0, 0, 0, 0, 1 }, 2, "Thundershock", new int[2] { 10, 1 });
            electabuzz[1] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 1 }, 23, "Thunderpunch", new int[3] { 30, 10, 10 });

            movement[] zapdos = new movement[2];
            zapdos[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 3 }, 23, "Thunder", new int[3] { 60, 0, 30 });
            zapdos[1] = new movement(new int[7] { 0, 0, 0, 0, 0, 0, 4 }, 18, "Thunderbolt", new int[3] { 100, Constants.TNone, -1 });

            movement[] raichu = new movement[2];
            raichu[0] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 1 }, 34, "Agility", new int[3] { 20, Legacies.barrier, 2 });
            raichu[1] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 3 }, 23, "Thunder", new int[3] { 60, 0, 30 });

            movement[] magneton = new movement[2];
            magneton[0] = new movement(new int[7] { 1, 0, 0, 0, 0, 0, 2 }, 2, "Thunder Wave", new int[2] { 30, 1 });
            magneton[1] = new movement(new int[7] { 2, 0, 0, 0, 0, 0, 2 }, 33, "Selfdestruct", new int[3] { 80, 20, 80 });

            #endregion

            Power rainDance = new Power("Rain Dance", 0, new int[1] { Constants.TWater }); // Creates a power
            Power energyTrans = new Power("Energy Trans", 1, new int[1] { Constants.TGrass });
            Power damageSwap = new Power("Damage Swap", 2, null);
            Power buzzap = new Power("Buzzap", 3, new int[1] { 2 });

            battler[] listBattlers = new battler[69];
            listBattlers[0] = new battler(1, Constants.TPsychic, 80, Constants.TPsychic, 2, Constants.TNone, 0, 3, "Alakazam", 65, 64, alakazam, damageSwap);
            listBattlers[1] = new battler(1, Constants.TWater, 100, Constants.TElectric, 2, -1, 0, 3, "Blastoise", 9, 8, moveset4, rainDance);
            listBattlers[2] = new battler(0, Constants.TNormal, 120, Constants.TFighting, 2, Constants.TPsychic, 30, 1, "Chansey", 113, -1, chansey, null);
            listBattlers[3] = new battler(1, Constants.TFire, 120, Constants.TWater, 2, Constants.TFighting, 30, 3, "Charizard", 6, 5, charizard, null, new int[1] { Legacies.energyBurn });
            listBattlers[4] = new battler(0, Constants.TNormal, 40, Constants.TFighting, 2, Constants.TPsychic, 30, 1, "Clefairy", 35, -1, clefairy, null);
            listBattlers[5] = new battler(1, Constants.TWater, 100, Constants.TGrass, 2, Constants.TFighting, 30, 3, "Gyarados", 130, 129, gyarados, null);
            listBattlers[6] = new battler(0, Constants.TFighting, 70, Constants.TPsychic, 2, Constants.TNone, 0, 2, "Hitmonchan", 107, -1, hitmonchan, null);
            listBattlers[7] = new battler(1, Constants.TFighting, 100, Constants.TPsychic, 2, Constants.TNone, 0, 3, "Machamp", 68, 67, machamp, null, new int[2] { Legacies.counter, 10 });
            listBattlers[8] = new battler(1, Constants.TElectric, 60, Constants.TFighting, 2, Constants.TNone, 0, 1, "Magneton", 82, 81, magneton, null);
            listBattlers[9] = new battler(0, Constants.TPsychic, 60, Constants.TPsychic, 2, Constants.TNone, 0, 3, "Mewtwo", 150, -1, mewtwo, null);
            listBattlers[10] = new battler(1, Constants.TGrass, 90, Constants.TPsychic, 2, Constants.TNone, 0, 3, "Nidoking", 34, 33, nidoking, null);
            listBattlers[11] = new battler(1, Constants.TFire, 80, Constants.TWater, 2, Constants.TNone, 0, 1, "Ninetales", 38, 37, ninetales, null);
            listBattlers[12] = new battler(1, Constants.TWater, 90, Constants.TGrass, 2, -1, 0, 3, "Poliwrath", 62, 61, poliwrath, null);
            listBattlers[13] = new battler(1, Constants.TElectric, 80, Constants.TFighting, 2, Constants.TNone, 0, 1, "Raichu", 26, 25, raichu, null);
            listBattlers[14] = new battler(1, Constants.TGrass, 100, Constants.TFire, 2, Constants.TNone, 0, 2, "Venusaur", 3, 2, venusaur, energyTrans);
            listBattlers[15] = new battler(0, Constants.TElectric, 90, Constants.TNone, 1, Constants.TFighting, 30, 3, "Zapdos", 145, -1, zapdos, null);
            listBattlers[16] = new battler(1, Constants.TGrass, 80, Constants.TFire, 2, Constants.TFighting, 30, 0, "Beedrill", 15, 14, beedrill, null);
            listBattlers[17] = new battler(1, Constants.TNormal, 80, Constants.TNone, 1, Constants.TPsychic, 30, 2, "Dragonair", 148, 147, dragonair, null);
            listBattlers[18] = new battler(1, Constants.TFighting, 70, Constants.TGrass, 2, Constants.TElectric, 30, 2, "Dugtrio", 51, 50, dugtrio, null);
            listBattlers[19] = new battler(0, Constants.TElectric, 70, Constants.TFighting, 2, Constants.TNone, 0, 2, "Electabuzz", 125, -1, electabuzz, null);
            listBattlers[20] = new battler(1, Constants.TElectric, 80, Constants.TFighting, 2, Constants.TNone, 0, 1, "Electrode", 101, 100, electrode, buzzap);
            listBattlers[21] = new battler(1, Constants.TNormal, 60, Constants.TElectric, 2, Constants.TFighting, 30, 1, "Pidgeotto", 17, 16, pidgeotto, null);
            listBattlers[22] = new battler(1, Constants.TFire, 100, Constants.TWater, 2, Constants.TNone, 0, 3, "Arcanine", 59, 58, arcanine, null);
            listBattlers[23] = new battler(1, Constants.TFire, 80, Constants.TWater, 2, Constants.TNone, 0, 1, "Charmeleon", 5, 4, charmeleon, null);
            listBattlers[24] = new battler(1, Constants.TWater, 80, Constants.TElectric, 2, -1, 0, 3, "Dewgong", 87, 86, dewgong, null);
            listBattlers[25] = new battler(0, Constants.TNormal, 40, Constants.TNone, 1, Constants.TPsychic, 30, 1, "Dratini", 147, -1, dratini, null);
            listBattlers[26] = new battler(0, Constants.TNormal, 50, Constants.TElectric, 2, Constants.TFighting, 30, 1, "Farfetch'd", 83, -1, farfetchd, null);
            listBattlers[27] = new battler(0, Constants.TFire, 60, Constants.TWater, 2, Constants.TNone, 0, 1, "Growlithe", 58, -1, growlithe, null);
            listBattlers[28] = new battler(1, Constants.TPsychic, 60, Constants.TNone, 1, Constants.TFighting, 30, 1, "Haunter", 93, 92, haunter, null);
            listBattlers[29] = new battler(1, Constants.TGrass, 60, Constants.TFire, 2, Constants.TNone, 0, 1, "Ivysaur", 2, 1, ivysaur, null);
            listBattlers[30] = new battler(0, Constants.TPsychic, 70, Constants.TPsychic, 2, Constants.TNone, 0, 2, "Jynx", 124, -1, jynx, null);
            listBattlers[31] = new battler(1, Constants.TPsychic, 60, Constants.TPsychic, 2, Constants.TNone, 0, 3, "Kadabra", 64, 63, kadabra, null);
            listBattlers[32] = new battler(1, Constants.TGrass, 80, Constants.TFire, 2, Constants.TNone, 0, 2, "Kakuna", 14, 13, kakuna, null);
            listBattlers[33] = new battler(1, Constants.TFighting, 80, Constants.TPsychic, 2, Constants.TNone, 0, 3, "Machoke", 67, 66, machoke, null);
            listBattlers[34] = new battler(0, Constants.TWater, 30, Constants.TElectric, 2, -1, 0, 1, "Magikarp", 129, -1, magikarp, null);
            listBattlers[35] = new battler(0, Constants.TFire, 50, Constants.TWater, 2, Constants.TNone, 0, 2, "Magmar", 126, -1, magmar, null);
            listBattlers[36] = new battler(1, Constants.TGrass, 60, Constants.TPsychic, 2, Constants.TNone, 0, 1, "Nidorino", 33, 32, nidorino, null);
            listBattlers[37] = new battler(1, Constants.TWater, 60, Constants.TGrass, 2, -1, 0, 1, "Poliwhirl", 61, 60, poliwhirl, null);
            listBattlers[38] = new battler(0, Constants.TNormal, 30, Constants.TFighting, 2, Constants.TPsychic, 30, 1, "Porygon", 137, -1, porygon, null);
            listBattlers[39] = new battler(1, Constants.TNormal, 60, Constants.TFighting, 2, Constants.TPsychic, 30, 1, "Raticate", 20, 19, raticate, null);
            listBattlers[40] = new battler(0, Constants.TWater, 60, Constants.TElectric, 2, -1, 0, 1, "Seel", 86, -1, seel, null);
            listBattlers[41] = new battler(1, Constants.TWater, 70, Constants.TElectric, 2, -1, 0, 1, "Wartortle", 8, 7, moveset3, null);
            listBattlers[42] = new battler(0, Constants.TPsychic, 30, Constants.TPsychic, 2, Constants.TNone, 0, 0, "Abra", 63, -1, abra, null);
            listBattlers[43] = new battler(0, Constants.TGrass, 40, Constants.TFire, 2, Constants.TNone, 0, 1, "Bulbasaur", 1, -1, bulbasaur, null);
            listBattlers[44] = new battler(0, Constants.TGrass, 40, Constants.TFire, 2, Constants.TNone, 0, 1, "Caterpie", 10, -1, caterpie, null);
            listBattlers[45] = new battler(0, Constants.TFire, 50, Constants.TWater, 2, Constants.TNone, 0, 1, "Charmander", 4, -1, charmander, null);
            listBattlers[46] = new battler(0, Constants.TFighting, 30, Constants.TGrass, 2, Constants.TElectric, 30, 0, "Digglet", 50, -1, digglet, null);
            listBattlers[47] = new battler(0, Constants.TNormal, 50, Constants.TElectric, 2, Constants.TFighting, 30, 0, "Dodou", 84, -1, dodou, null);
            listBattlers[48] = new battler(0, Constants.TPsychic, 50, Constants.TPsychic, 2, Constants.TNone, 0, 1, "Drowzee", 96, -1, drowzee, null);
            listBattlers[49] = new battler(0, Constants.TPsychic, 30, Constants.TNone, 1, Constants.TFighting, 30, 0, "Gastly", 92, -1, gastly, null);
            listBattlers[50] = new battler(0, Constants.TGrass, 50, Constants.TPsychic, 2, Constants.TNone, 0, 1, "Koffing", 109, -1, koffing, null);
            listBattlers[51] = new battler(0, Constants.TFighting, 50, Constants.TPsychic, 2, Constants.TNone, 0, 1, "Machop", 66, -1, machop, null);
            listBattlers[52] = new battler(0, Constants.TElectric, 40, Constants.TFighting, 2, Constants.TNone, 0, 1, "Magnemite", 81, -1, magnemite, null);
            listBattlers[53] = new battler(1, Constants.TGrass, 70, Constants.TFire, 2, Constants.TNone, 0, 2, "Metapod", 11, 10, metapod, null);
            listBattlers[54] = new battler(0, Constants.TGrass, 40, Constants.TPsychic, 2, Constants.TNone, 0, 1, "Nidoran M", 32, -1, nidoranm, null);
            listBattlers[55] = new battler(0, Constants.TFighting, 90, Constants.TGrass, 2, Constants.TNone, 0, 3, "Onix", 95, -1, onix, null);
            listBattlers[56] = new battler(0, Constants.TNormal, 40, Constants.TElectric, 2, Constants.TFighting, 30, 1, "Pidgey", 16, -1, pidgey, null);
            listBattlers[57] = new battler(0, Constants.TElectric, 40, Constants.TFighting, 2, Constants.TNone, 0, 1, "Pikachu", 25, -1, pikachu, null);
            listBattlers[58] = new battler(0, Constants.TWater, 40, Constants.TGrass, 2, -1, 0, 1, "Poliwag", 60, -1, poliwag, null);
            listBattlers[59] = new battler(0, Constants.TFire, 40, Constants.TWater, 2, Constants.TNone, 0, 1, "Ponyta", 77, -1, ponyta, null);
            listBattlers[60] = new battler(0, Constants.TNormal, 30, Constants.TFighting, 2, Constants.TPsychic, 30, 0, "Rattata", 19, -1, rattata, null);
            listBattlers[61] = new battler(0, Constants.TFighting, 40, Constants.TGrass, 2, Constants.TElectric, 30, 1, "Sandshrew", 27, -1, sandshrew, null);
            listBattlers[62] = new battler(0, Constants.TWater, 40, Constants.TElectric, 2, -1, 0, 1, "Squirtle", 7, -1, moveset2, null);
            listBattlers[63] = new battler(1, Constants.TWater, 60, Constants.TElectric, 2, -1, 0, 1, "Starmie", 121, 120, moveset1, null);
            listBattlers[64] = new battler(0, Constants.TWater, 40, Constants.TElectric, 2, -1, 0, 1, "Staryu", 120, -1, moveset, null);
            listBattlers[65] = new battler(0, Constants.TGrass, 50, Constants.TFire, 2, Constants.TNone, 0, 2, "Tangela", 114, -1, tangela, null);
            listBattlers[66] = new battler(0, Constants.TElectric, 40, Constants.TFighting, 2, Constants.TNone, 0, 1, "Voltorb", 100, -1, voltorb, null);
            listBattlers[67] = new battler(0, Constants.TFire, 50, Constants.TWater, 2, Constants.TNone, 0, 1, "Vulpix", 37, -1, vulpix, null);
            listBattlers[68] = new battler(0, Constants.TGrass, 40, Constants.TFire, 2, Constants.TNone, 0, 1, "Weedle", 13, -1, weedle, null);


            MemoryStream stream1 = new MemoryStream();
            string ser = JsonConvert.SerializeObject(listBattlers, Formatting.Indented);

            File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + @"\data\pokemon", ser);
        }

        public static void trainers()
        {
            trainer[] trainers = new trainer[26];
            trainers[0] = new trainer(0, 0, new int[2] { 10, Legacies.clefairyDoll }, "Clefairy Doll");
            trainers[1] = new trainer(0, 1, new int[2] { 2, -1 }, "Computer Search");

            MemoryStream stream1 = new MemoryStream();
            string ser = JsonConvert.SerializeObject(trainers, Formatting.Indented);

            File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + @"\data\trainers", ser);
        }
    }
}
