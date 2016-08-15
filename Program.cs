using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using shandakemon.core;
using shandakemon.AI;
using Newtonsoft.Json;

namespace shandakemon
{
    class Program
    {
        static void Main(string[] args)
        {
            DBBuilder.energies();
            DBBuilder.pokemon();
            DBBuilder.trainers();

            string raw_energy = File.ReadAllText(Directory.GetCurrentDirectory() + @"\data\energies");
            string raw_pokemon = File.ReadAllText(Directory.GetCurrentDirectory() + @"\data\pokemon");
            string raw_trainers = File.ReadAllText(Directory.GetCurrentDirectory() + @"\data\trainers");

            energy[] energies = JsonConvert.DeserializeObject<energy[]>(raw_energy);
            battler[] battlers = JsonConvert.DeserializeObject<battler[]>(raw_pokemon);
            trainer[] trainers = JsonConvert.DeserializeObject<trainer[]>(raw_trainers);

            LinkedList<card> deck1 = new LinkedList<card>(); // Creates a deck
            LinkedList<card> deck2 = new LinkedList<card>();

            List<int> deckIndexes = utils.ReadDeck.ReadIndexes("player1.txt");
            foreach (int index in deckIndexes)
            {
                if (index < 70) // Battler card
                    deck1.AddFirst(battlers[index - 1].DeepCopy());
                else if (index < 96) // Trainer card
                    deck1.AddFirst(trainers[index - 70].DeepCopy());
                else
                    deck1.AddFirst(energies[index - 96].DeepCopy());
            }

            deckIndexes = utils.ReadDeck.ReadIndexes("overgrowth.txt");
            foreach (int index in deckIndexes)
            {
                if (index < 70) // Battler card
                    deck2.AddFirst(battlers[index - 1].DeepCopy());
                else if (index < 96) // Trainer card
                    deck2.AddFirst(trainers[index - 70].DeepCopy());
                else
                    deck2.AddFirst(energies[index - 96].DeepCopy());
            }

            Player player1 = new Player(1, deck1, 2);
            Player player2 = new Player(2, deck2, 2); // Creates the players
            SimpleAI p2ai = new SimpleAI(player2);
            player2.AddAIController(p2ai);

            duel dd = new duel(player1, player2); // Creates the duel
            dd.battleFlow(); // Starts the battle

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
