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

            do
            {
                LinkedList<card> deck1 = utils.ReadDeck.DeckAssembler(utils.ReadDeck.RandomDeck(), battlers, trainers, energies); // Creates a deck
                LinkedList<card> deck2 = utils.ReadDeck.DeckAssembler(utils.ReadDeck.RandomDeck(), battlers, trainers, energies);

                Player player1 = new Player(1, deck1, 2);
                Player player2 = new Player(2, deck2, 2); // Creates the players

                Console.WriteLine("Do you want to spy the opponent's hand? (y/n):");
                SimpleAI p2ai = new SimpleAI(player2, utils.ConsoleParser.ReadYesNo());
                player2.AddAIController(p2ai);

                duel dd = new duel(player1, player2); // Creates the duel
                dd.battleFlow(); // Starts the battle

                Console.WriteLine("Do you want to exit? (y/n):");
            }
            while (!utils.ConsoleParser.ReadYesNo());
        }
    }
}
