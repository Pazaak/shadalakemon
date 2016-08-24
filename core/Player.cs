using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using shandakemon.AI;

namespace shandakemon.core
{
    /* Holds information about the player
     *  id- Identifier of the player
     *  benched- List of battlers
     *  hand- List of cards in hand
     *  discarded- List of cards in the discard pile
     *  deck- List of cards in the deck
     *  prices- Array of price cards
     *  winCOndition- Determines if the player won
     *  isAI- Determines if the player is controlled by an AI
     */
    public class Player
    {
        public int id;
        public bool winCondition, isAI;
        public battler lastFront;
        public List<battler> benched;
        public List<card> hand, discarded;
        public LinkedList<card> deck;
        public card[] prices;
        public DuelAI controller;

        public Player(int id, LinkedList<card> deck, int nPrices)
        {
            this.id = id;
            this.deck = deck;
            // Initilization of the lists
            this.discarded = new List<card>();
            this.hand = new List<card>();
            this.prices = new card[nPrices];
            this.benched = new List<battler>();
            this.winCondition = false;
            this.isAI = false;
        }

        public override string ToString()
        {
            return "Player " + id;
        }

        public void AddAIController(DuelAI cont)
        {
            controller = cont;
            isAI = true;
        }

        // Shows visually the number of damage counters on each battler
        public string ShowDamage()
        {
            string output = "";

            for (int i = 0; i < benched.Count; i++)
                output += i + "- " + benched[i].ToString() + " " + benched[i].ShowDamage() + Environment.NewLine;
            return output;
        }

        // Method that suffles the deck
        public void shuffle()
        {
            utils.Logger.Report(ToString() + " shuffles its deck.");
            this.deck = new LinkedList<card>(this.deck.OrderBy(x => CRandom.RandomInt()));
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

        public card GetFromDeck(int index)
        {
            LinkedListNode<card> output = deck.First;
            for (int i = 0; i < index; i++) output = output.Next;
            return output.Value;
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
                deck.AddFirst(c1);
            hand.Clear();
            utils.Logger.Report(ToString() + " puts its hand on the deck.");
            shuffle();
            
        }

        // Utility method to represent the hand of the player
        public string ShowHand()
        {
            string output = "";
            for (int i = 0; i < hand.Count; i++)
                output += i + "- " + hand[i].ToString() + Environment.NewLine;
            return (output);
        }

        // Utility method to represent all the battlers
        public string ShowBattlers()
        {
            string output = "";

            for (int i = 0; i < benched.Count; i++)
                if (benched[i] != null)
                    output += i + "- " + benched[i].ToString() + Environment.NewLine;
            return (output);
        }

        // Utility method to represent only the benched battlers
        public string ShowBenched()
        {
            string output = "";
            for (int i = 1; i < benched.Count; i++)
                output += i + "- " + benched[i].ToString() + Environment.NewLine;
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
                discardEnergy(target, target.energies.First(), false);
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
            

            if (target == benched[0]) benched[0] = null;
            else benched.Remove(target);
            
        }

        // Takes a battler on the bench an place it on the front
        public void toFront(battler btl)
        {
            benched.Remove(btl);
            benched[0] = btl;
            utils.Logger.Report(btl.ToString() + " is put in the active position.");
        }

        // Eliminates the condition that determines if the battler was placed or evolved in this turn
        public void clearSickness()
        {
            foreach (battler bt in benched)
                bt.sumSick = false;
        }

        // Discard the selected energy card from the selected battler (must be done at the player side to use the discard pile)
        public void discardEnergy(battler source, energy output, bool verbose = true)
        {
            if (!output.proxy)
            {
                discarded.Add(output);
                source.energies.Remove(output);
                source.energyTotal[output.elem] -= output.quan;
            }
            else
            {
                discarded.Add(output.attached);
                source.energies.Remove(output);
                source.energyTotal[output.elem] -= output.quan;
            }
            if (verbose)
                utils.Logger.Report(source.ToString() + " had its energy " + output.ToString() + " discarded.");
        }

        // Eliminates one turn counter of all the conditions
        public void checkConditions()
        {
            foreach (var key in benched[0].conditions.Keys.ToList())
            {
                if (benched[0].conditions[key][0] == 1) // Only front can have conditions (?)
                    benched[0].conditions.Remove(key);
                else
                    benched[0].conditions[key][0]--;
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
        public string DisplayTypedEnergies(int elem)
        {
            string output = "";
            for (int i = 0; i < benched.Count; i++)
                output += i + "- " + benched[i].ShowEnergyByType(elem);
            return output;
        }

        // Makes the power active again
        public void ResetPowers()
        {
            foreach (battler bt in benched)
                if (bt.power != null) bt.power.active = true;
        }

        // Creates a list of the powers of all the battlers in play and indicates if any
        public bool ListPowers()
        {
            bool somethingToShow = false;

            for (int i = 0; i < benched.Count; i++)
            {
                if (benched[i].power != null && benched[i].power.active && benched[i].CanUsePowers())
                {
                    Console.WriteLine(i +"- "+ benched[i].ToString() + ": " + benched[i].power.name);
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
            battler temp = benched[0];
            temp.ToBench();
            benched[0] = benched[index];
            benched.RemoveAt(index);
            benched.Add(temp);

            utils.Logger.Report(temp.ToString() + " placed in the bench." + Environment.NewLine + benched[0].ToString() + " placed in the active area.");
        }

        // Method to select a price
        public void PriceProcedure()
        {
            if ( isAI )
            {
                this.controller.PriceProcedure();
                return;
            }

            bool[] prices = this.listPrices(); // Checks available prices
            string numPrices = "";
            for (int i = 0; i < prices.Length; i++)
                numPrices += prices[i] ? i + " " : "";

            Console.WriteLine("Select a price card to draw: " + numPrices);

            int index = utils.ConsoleParser.ReadNumber(prices.Length); // Asks for the price
            while (!prices[index])
            {
                Console.WriteLine("Invalid selection. Select a price card to draw: " + numPrices);
                index = utils.ConsoleParser.ReadNumber(prices.Length);
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
                Console.WriteLine(this.ToString()+" has no other pokémon on the battlefield.");
                Console.WriteLine(opponent.ToString() + " wins the game.");
                utils.Logger.Report(this.ToString() + " losses by lack of active pokémon.");
                opponent.winCondition = true;
                return;
            }

            if (isAI)
            {
                this.controller.KnockoutProcedure();
                return;
            }

            Console.WriteLine(this.ToString() + ": Select a benched pokemon"); // Ask for a battler to put in front
            Console.WriteLine(this.ShowBenched());

            int digit;
            do
            {
                digit = utils.ConsoleParser.ReadNumber(this.benched.Count - 1); // Choose the battler to put in front
                if (digit == 0)
                    Console.WriteLine("Not valid input. Try again.");
            }
            while (digit == 0);

            this.toFront(this.benched[digit]);
        }

        // Paralysis check
        public void ParalysisCheck()
        {
            if (this.benched[0].status == 1) // Heal if paralyzed
            {
                this.benched[0].status = 0;
                Console.WriteLine(this.benched[0].ToString() + " is not longer paralyzed.");
                utils.Logger.Report(this.benched[0].ToString() + " is not longer paralyzed.");
            }
        }

        // Sleep check
        public void SleepCheck()
        {
            if (this.benched[0].status == 2) // Try to heal if asleep
            {
                if (CRandom.RandomInt() < 0)
                {
                    this.benched[0].status = 0;
                    utils.Logger.Report(this.ToString() + " wins the coin flip.");
                    Console.WriteLine(this.benched[0].ToString() + " awakes.");
                    utils.Logger.Report(this.benched[0].ToString() + " awakes.");
                }
                else
                    Console.WriteLine(this.benched[0].ToString() + " still sleeping.");
            }
        }
        
        // Poison check
        public void PoisonCheck(Player p2)
        {
            if (this.benched[0].status == 10) // Poison check
            {
                Console.WriteLine(this.benched[0].ToString() + " takes damage due poisoning.");
                utils.Logger.Report(this.benched[0].ToString() + " takes damage due poisoning.");
                effects.damage(Constants.TNone, 10, this.benched[0], null, this, p2);
            }

            if (this.benched[0].status == 11) // Strongly Poison check
            {
                Console.WriteLine(this.benched[0].ToString() + " takes damage due poisoning.");
                utils.Logger.Report(this.benched[0].ToString() + " takes damage due poisoning.");
                effects.damage(Constants.TNone, 20, this.benched[0], null, this, p2);
            }
        }

        // Discard a trainer
        public void CardToDiscard(card target)
        {
            this.discarded.Add(target);
            this.hand.Remove(target);
            utils.Logger.Report(target.ToString() + " discarded from hand.");
        }

        // Discard hand
        public void DiscardHand()
        {
            foreach (card ca in hand)
                discarded.Add(ca);
            hand.Clear();
        }

        // Show deck
        public string ShowDeck()
        {
            LinkedListNode<card> actual = deck.First;
            string output = "";
            for (int i = 0; i < deck.Count; i++)
            {
                output += i + "- " + actual.Value.ToString()+Environment.NewLine;
                actual = actual.Next;
            }
            return output;
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
        public battler SelectBattler(int digit = -1)
        {
            if ( digit == -1)
            {
                Console.WriteLine("Select an active pokémon:");
                Console.WriteLine(this.ShowBattlers());
                digit = utils.ConsoleParser.ReadNumber(benched.Count-1);
            }
            
            return benched[digit];
        }

        // Search deck for a card
        public card SearchDeck(int superType)
        {
            int digit;
            card output;
            do
            {
                Console.WriteLine("Select a card from the deck:");
                Console.WriteLine(this.ShowDeck());
                Int32.TryParse(Console.ReadLine(), out digit);
                output = this.GetFromDeck(digit);
            } while (superType != -1 && output.getSuperType() != superType);
            return output;
        }

        // Search the discard file for a card
        public card SearchPile(int superType = -1)
        {
            if (this.discarded.Count == 0)
            {
                Console.WriteLine("Discard pile is empty.");
                return null;
            }

            if (superType != -1) // Check if there are card of that type
            {
                bool flag = false;
                foreach (card ca in this.discarded)
                    if (ca.getSuperType() == superType)
                    {
                        flag = true;
                        break;
                    }

                if (!flag)
                {
                    Console.WriteLine("There's no card of the given type in the discard pile.");
                    return null;
                }
            }

            int digit;
            do
            {
                Console.WriteLine("Choose a card from the discard pile:");
                Console.WriteLine(this.ShowDiscardPile());
                Int32.TryParse(Console.ReadLine(), out digit);
            } while (superType != -1 && this.discarded[digit].getSuperType() != superType);
            return this.discarded[digit];
        }

        // Searches a determined type of card in the hand
        public card SearchHand(int superType)
        {
            int digit;
            do
            {
                Console.WriteLine("Select a card from the hand:");
                Console.WriteLine(this.ShowHand());
                digit = utils.ConsoleParser.ReadNumber(hand.Count - 1);
            } while (this.hand[digit].getSuperType() != superType);
            return this.hand[digit];
        }
    }
}
