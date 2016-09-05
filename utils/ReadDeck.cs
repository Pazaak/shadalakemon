using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using shandakemon.core;

namespace shandakemon.utils
{
    class ReadDeck
    {
        public static List<int> ReadIndexes(string path)
        {
            string[] buffer = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + @"/decks/" + path);
            List<int> output = new List<int>();

            foreach (string str in buffer)
            {
                string[] tokens = str.Split(' ');
                int times, index;
                if (Int32.TryParse(tokens[0], out times) && Int32.TryParse(tokens[1], out index))
                    while ( times-- > 0 ) output.Add(index);
            }

            return output;
        }

        public static LinkedList<card> DeckAssembler(string name, battler[] battlers, trainer[] trainers, energy[] energies)
        {
            LinkedList<card> output = new LinkedList<card>();
            List<int> deckIndexes = ReadIndexes(name);
            foreach (int index in deckIndexes)
            {
                if (index < 70) // Battler card
                    output.AddFirst(battlers[index - 1].DeepCopy());
                else if (index < 96) // Trainer card
                    output.AddFirst(trainers[index - 70].DeepCopy());
                else
                    output.AddFirst(energies[index - 96].DeepCopy());
            }
            return output;
        }

        public static string RandomDeck()
        {
            switch( Math.Abs(CRandom.RandomInt() % 5) )
            {
                case 0: return "2player.txt";
                case 1: return "blackout.txt";
                case 2: return "brushfire.txt";
                case 3: return "overgrowth.txt";
                case 4: return "zap!.txt";
                default: return "";
            }
        }
    }
}
