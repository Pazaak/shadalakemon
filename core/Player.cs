using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    /* Holds information about the player
     *  id- Identifier of the player
     *  front- The battler positioned in front
     *  benched- List of backup battlers
     *  hand- List of cards in hand
     *  discarded- List of cards in the discard pile
     *  deck- List of cards in the deck
     *  prices- Array of price cards
     *  winCOndition- Determines if the player won
     */
    public class Player
    {
        public int id;
        public bool winCondition;
        public battler front, lastFront;
        public List<battler> benched;
        public List<card> hand, discarded, deck;
        public card[] prices;

        public Player(int id, List<card> deck, int nPrices)
        {
            this.id = id;
            this.deck = deck;
            // Initilization of the lists
            this.discarded = new List<card>();
            this.hand = new List<card>();
            this.prices = new card[nPrices];
            this.benched = new List<battler>();
            this.winCondition = false;
        }

        public override string ToString()
        {
            return "Player " + id;
        }

        // Shows visually the number of damage counters on each battler
        public string ShowDamage()
        {
            string output = "1- " + front.ToString() + " " + front.ShowDamage()+Environment.NewLine;

            for (int i = 0; i < benched.Count; i++)
                output += (i + 2) + "- " + benched[i].ToString() + " " + benched[i].ShowDamage() + Environment.NewLine;
            return output;
        }

        // Method that suffles the deck
        public void shuffle()
        {
            utils.Logger.Report(ToString() + " shuffles its deck.");
            this.deck = new List<card>(this.deck.OrderBy(x => CRandom.RandomInt()));
        }

        // Method to draw 'times' number of cards
        public bool draw(int times)
        {
            if (deck.Count < times) // Check if there are enough cards in the deck to draw
                return false;
            for (int i = 0; i < times; i++)
            {
                hand.Add(deck.First());
                deck.Remove(deck.First());
            }

            utils.Logger.Report(ToString() + " draws " + times + (times == 1 ? " card." : " cards."));
            return true;
        }

        // Method to draw the selected price
        public void drawPrice(int index)
        {
            hand.Add(prices[index]);
            prices[index] = null;
            utils.Logger.Report(ToString() + " draws price number " + index + ".");
        }

        // Method that puts the selected number of cards in the price area
        public void putPrices()
        {
            for (int i = 0; i < prices.Length; i++)
            {
                this.prices[i] = deck.First();
                deck.Remove(deck.First());
            }

            utils.Logger.Report(ToString() + " puts " + prices.Length + " price cards.");
        }

        // Method that shuffles the hand in the library
        public void ShuffleHand()
        {
            if (hand.Count == 0)
            {
                shuffle();
                return; // No cards in hand
            }

            foreach (card c1 in hand)
                deck.Add(c1);
            hand.Clear();
            utils.Logger.Report(ToString() + " puts its hand on the deck.");
            shuffle();
            
        }

        // Utility method to represent the hand of the player
        public string writeHand()
        {
            string output = "";
            for (int i = 0; i < hand.Count; i++)
                output += (i + 1) + "- " + hand[i].ToString() + Environment.NewLine;
            return (output);
        }

        // Utility method to represent all the battlers
        public string writeBattlers()
        {
            string output = "";
            if (front != null)
                output = "1- " + front.ToString() + Environment.NewLine;

            for (int i = 0; i < benched.Count; i++)
                if (benched[i] != null)
                    output += (i + 2) + "- " + benched[i].ToString() + Environment.NewLine;
            return (output);
        }

        // Utility method to represent only the benched battlers
        public string writeBenched()
        {
            string output = "";
            for (int i = 0; i < benched.Count; i++)
                output += (i + 1) + "- " + benched[i].ToString() + Environment.NewLine;
            return (output);
        }

        // Check which prices are already taken
        public bool[] listPrices()
        {
            bool[] output = new bool[prices.Length];
            for (int i = 0; i < prices.Length; i++)
                if (prices[i] != null)
                    output[i] = true;
            return output;
        }

        // Sends the front battler to the discard pile
        public void ToDiscard(battler target)
        {
            while (target.energies.Count != 0) // First discard all the energy cards
            {
                discardEnergy(target, 0, false);
            }

            if (target.prevolution != null)
                discarded.Add(target.prevolution);
            target.prevolution = null;

            target.clear();
            if (target.proxy)
            {
                discarded.Add(target.attached);
                utils.Logger.Report(target.attached.ToString() + " is discarded.");
            }
            else
            {
                discarded.Add(target);
                utils.Logger.Report(target.ToString() + " is discarded.");
            }
            

            if (target == front) front = null;
            else benched.Remove(target);
            
        }

        // Takes a battler on the bench an place it on the front
        public void toFront(int index)
        {
            front = benched[index];
            benched.RemoveAt(index);
            utils.Logger.Report(front.ToString() + " is put in the active position.");
        }

        // Eliminates the condition that determines if the battler was placed or evolved in this turn
        public void clearSickness()
        {
            front.sumSick = false;
            foreach (battler bt in benched)
                bt.sumSick = false;
        }

        // Discard the selected energy card from the selected battler (must be done at the player side to use the discard pile)
        public void discardEnergy(battler source, int energy_index, bool verbose = true)
        {
            energy output = source.energies[energy_index];
            if (!output.proxy)
            {
                discarded.Add(output);
                source.energies.RemoveAt(energy_index);
                source.energyTotal[output.elem] -= output.quan;
            }
            else
            {
                discarded.Add(output.attached);
                source.energies.RemoveAt(energy_index);
                source.energyTotal[output.elem] -= output.quan;
            }
            if (verbose)
                utils.Logger.Report(source.ToString() + " had its energy " + output.ToString() + " discarded.");
        }

        // Eliminates one turn counter of all the conditions
        public void checkConditions()
        {
            foreach (var key in front.conditions.Keys.ToList())
            {
                if (front.conditions[key][0] == 1) // Only front can have conditions (?)
                    front.conditions.Remove(key);
                else
                    front.conditions[key][0]--;
            }
        }

        // Indicates if the initial hand is valid (has basic pokemon)
        public bool checkInitialHand()
        {
            foreach (card inst in hand)
            {
                if (inst.getSuperType() == 0)
                {
                    battler temp = (battler)inst;
                    if (temp.type == 0) return true;
                }
            }
            return false;
        }

        // Shows the attached energies of the selected type
        public void DisplayTypedEnergies(int elem)
        {
            Console.WriteLine("1- "+front.ShowEnergyByType(elem));
            for (int i = 0; i < benched.Count; i++)
                Console.WriteLine((i + 2) + "- " + benched[i].ShowEnergyByType(elem));
        }

        // Makes the power active again
        public void ResetPowers()
        {
            if (front.power != null) front.power.active = true;
            foreach (battler bt in benched)
                if (bt.power != null) bt.power.active = true;
        }

        // Creates a list of the powers of all the battlers in play and indicates if any
        public bool ListPowers()
        {
            bool somethingToShow = false;
            if (front.power != null && front.power.active && front.CanUsePowers()) 
            {
                Console.WriteLine("1- " + front.ToString() + ": " + front.power.name);
                somethingToShow = true;
            }

            for (int i = 0; i < benched.Count; i++)
            {
                if (benched[i].power != null && benched[i].power.active && front.CanUsePowers())
                {
                    Console.WriteLine((i + 2) +"- "+ benched[i].ToString() + ": " + benched[i].power.name);
                    somethingToShow = true;
                }
                
            }

            if (!somethingToShow)
            {
                Console.WriteLine("There are no pokemon with powers in play");
                return false;
            }

            return true;
        }

        // Exchanges the position of the front battler with a benched battler
        public void ExchangePosition(int index)
        {
            battler temp = front;
            temp.ToBench();
            front = benched[index];
            benched.RemoveAt(index);
            benched.Add(temp);

            utils.Logger.Report(temp.ToString() + " placed in the bench." + Environment.NewLine + front.ToString() + " placed in the active area.");
        }

        // Method to select a price
        public void PriceProcedure()
        {
            bool[] prices = this.listPrices(); // Checks available prices
            string numPrices = "";
            for (int i = 0; i < prices.Length; i++)
                numPrices += prices[i] ? (i + 1) + " " : "";

            Console.WriteLine("Select a price card to draw: " + numPrices);

            int index = Convert.ToInt16(Console.ReadKey().KeyChar) - 49; // Asks for the price
            while (!prices[index])
            {
                Console.WriteLine("Invalid selection. Select a price card to draw: " + numPrices);
                index = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;
            }

            this.drawPrice(index);  // Draws the price

            prices = this.listPrices(); // Checks if the are not more prices to draw
            bool accu = false;
            for (int i = 0; i < prices.Length; i++)
                accu = accu || prices[i];

            if (!accu)
            {
                Console.WriteLine("No more price cards. Player " + this.id + " wins."); // Makes the player win
                utils.Logger.Report(this.ToString() + " has no more price cards. " + this.ToString() + " wins.");
                winCondition = true;
            }
        }

        // Procedure to select a new front battler
        public void KnockoutProcedure(Player opponent)
        {
            if (benched.Count == 0)
            {
                Console.WriteLine("There is no other pokémon on the battlefield.");
                opponent.winCondition = true;
                return;
            }

            Console.WriteLine(this.ToString() + ": Select a benched pokemon"); // Ask for a battler to put in front
            Console.WriteLine(this.writeBenched());

            this.toFront(Convert.ToInt16(Console.ReadKey().KeyChar) - 49);
        }

        // Paralysis check
        public void ParalysisCheck()
        {
            if (this.front.status == 1) // Heal if paralyzed
            {
                this.front.status = 0;
                Console.WriteLine(this.front.ToString() + " is not longer paralyzed.");
                utils.Logger.Report(this.front.ToString() + " is not longer paralyzed.");
            }
        }

        // Sleep check
        public void SleepCheck()
        {
            if (this.front.status == 2) // Try to heal if asleep
            {
                if (CRandom.RandomInt() < 0)
                {
                    this.front.status = 0;
                    utils.Logger.Report(this.ToString() + " wins the coin flip.");
                    Console.WriteLine(this.front.ToString() + " awakes.");
                    utils.Logger.Report(this.front.ToString() + " awakes.");
                }
                else
                    Console.WriteLine(this.front.ToString() + " still sleeping.");
            }
        }
        
        // Poison check
        public void PoisonCheck(Player p2)
        {
            if (this.front.status == 10) // Poison check
            {
                Console.WriteLine(this.front.ToString() + " takes damage due poisoning.");
                utils.Logger.Report(this.front.ToString() + " takes damage due poisoning.");
                effects.damage(Constants.TNone, 10, this.front, null, this, p2);
            }

            if (this.front.status == 11) // Strongly Poison check
            {
                Console.WriteLine(this.front.ToString() + " takes damage due poisoning.");
                utils.Logger.Report(this.front.ToString() + " takes damage due poisoning.");
                effects.damage(Constants.TNone, 20, this.front, null, this, p2);
            }
        }

        // Discard a trainer
        public void TrainerToDiscard(card target)
        {
            this.discarded.Add(target);
            this.hand.Remove(target);
        }

        // Show deck
        public void ShowDeck()
        {
            for (int i = 0; i < deck.Count; i++)
                Console.WriteLine(i +"- "+ deck[i].ToString());
        }

        // Show discard pile
        public string ShowDiscardPile()
        {
            string output = "";
            for (int i = 0; i < discarded.Count; i++)
                output += i + "- " + discarded[i].ToString() + Environment.NewLine;
            return output;

        }

        // Select a battler
        public battler SelectBattler()
        {
            int digit;
            Console.WriteLine("Select an active pokémon:");
            Console.WriteLine(this.writeBattlers());
            Int32.TryParse(Console.ReadLine(), out digit);

            if (digit == 1)
                return front;
            else
                return benched[digit - 2];
        }
    }
}
