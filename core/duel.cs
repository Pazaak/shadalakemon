using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    class duel
    {
        Player player1;
        Player player2;

        public duel(Player p1, Player p2)
        {
            player1 = p1;
            player2 = p2;
        }

        public bool battleFlow()
        {
            this.initialPhase();

            while (true)
            {
                
                // Player 1
                switch (mainPhase(player1))
                {
                    case -1: return false;
                    case 0: attackPhase(player1, player2); break;
                    case 1: break;
                }
                    
                if (endPhase(player1, player2))
                    return true;

                betweenTurns(player1, player2);

                // Player 2
                switch (mainPhase(player2))
                {
                    case -1: return false;
                    case 0: attackPhase(player2, player1); break;
                    case 1: break;
                }

                if (endPhase(player2, player1))
                    return false;

                betweenTurns(player2, player1);
            }
        }

        public void attackPhase(Player att, Player def)
        {
            Console.WriteLine("Player " + att.id);
            Console.WriteLine("Active pokemon: ");
            Console.WriteLine(att.front.BattleDescription());
            Console.WriteLine("Select a movement: ");
            att.front.execute(Convert.ToInt16(Console.ReadKey().KeyChar)-49, att, def, def.front);
        }

        // Return codes:
        // -1: p1 lost
        // 0: Normal exit
        // 1: No attack exit
        public int mainPhase(Player p1)
        {
            bool energyLimit = false;
            if (!p1.draw(1))
            {
                Console.WriteLine("Player "+p1.id+" cannot draw any more cards. Player "+p1.id+" losses.");
                return -1;
            }
            Console.WriteLine("Player " + p1.id + " play cards. 0 to advance to attack phase");

            while (true)
            {
                Console.WriteLine(p1.writeHand());
                int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;
                if (digit == -1)
                {
                    if (p1.front.status == 1) // Paralyzed
                        return 1;
                    else
                        return 0;
                }

                card selected = p1.hand[digit];
                if (selected.getSuperType() == 0)
                {
                    battler newBattler = (battler)selected;
                    switch (newBattler.type)
                    {
                        case 0: // Basic pokemon
                            if (p1.benched.Count < 5)
                            {
                                p1.hand.Remove(selected);
                                p1.benched.Add(newBattler);
                                Console.WriteLine(selected.ToString() + " selected as benched pokemon");
                            }
                            else
                                Console.WriteLine("You cannot bench more pokemon");
                            break;
                        case 1: // Evolution pokemon
                            bool done = false;
                            while (!done)
                            {
                                Console.WriteLine("Select the pokemon to evolve: ");
                                Console.WriteLine(p1.writeBattlers());
                                digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50;
                                if (digit == -2)
                                {
                                    done = true;
                                    break;
                                }

                                battler target;

                                if ( digit == -1 )
                                    target = p1.front;
                                else
                                    target = p1.benched[digit];

                                if (target.id != newBattler.evolvesFrom)
                                    Console.WriteLine("That pokemon cannot evolve in the selected one");
                                else if (target.sumSick)
                                    Console.WriteLine("You cannot evolve a pokemon that was benched this turn");
                                else
                                {
                                    newBattler.evolve(target);
                                    if (digit == -1)
                                        p1.front = newBattler;
                                    else
                                        p1.benched[digit] = newBattler;
                                    p1.hand.Remove(selected);
                                    done = true;
                                    Console.WriteLine(target.ToString() + " evolved into " + newBattler.ToString());
                                }

                            }
                            break;



                    }
                        
                }
                else if (selected.getSuperType() == 1)
                {
                    // TODO: Execute trainers
                }
                else // Energy
                {
                    if (energyLimit)
                        Console.WriteLine("You can only attach one energy per turn.");
                    else
                    {
                        energy attEnergy = (energy) selected;
                        Console.WriteLine("Please select the pokemon or press 0 to exit:");
                        Console.WriteLine(p1.writeBattlers());

                        digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;
                        if (digit == 0)
                        {
                            p1.front.attachEnergy(attEnergy);
                            p1.hand.Remove(selected);
                            energyLimit = true;
                            Console.WriteLine("Energy attached to " + p1.front.ToString());
                        }
                        else if (digit > 0)
                        {
                            p1.benched[digit - 1].attachEnergy(attEnergy);
                            p1.hand.Remove(selected);
                            energyLimit = true;
                            Console.WriteLine("Energy attached to " + p1.benched[digit - 1].ToString());
                        }
                    }
                }
            }
        }

        public bool endPhase(Player p1, Player p2)
        {
            if (p2.front.damage < p2.front.HP) return (false);

            p2.frontToDiscard();

            bool[] prices = p1.listPrices();
            string numPrices = "";
            for (int i = 0; i < prices.Length; i++)
                numPrices += prices[i] ? (i+1) + " " : "";

            Console.WriteLine("Select a price card to draw: " + numPrices);

            int index = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;
            while (!prices[index])
            {
                Console.WriteLine("Invalid selection. Select a price card to draw: " + numPrices);
                index = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;
            }

            p1.drawPrice(index);

            prices = p1.listPrices();
            bool accu = false;
            for (int i = 0; i < prices.Length; i++)
                accu = accu || prices[i];

            if (!accu)
            {
                Console.WriteLine("No more price cards. Player " + p1.id + " wins.");
                return (true);
            }

            if (p2.benched.Count == 0)
            {
                Console.WriteLine("Player "+p2.id+" has no benched pokemon. Player " + p1.id + " wins.");
                return (true);
            }

            Console.WriteLine("Player " + p2.id + ": Select a benched pokemon");
            Console.WriteLine(p2.writeBenched());

            p2.toFront(Convert.ToInt16(Console.ReadKey().KeyChar) - 49);

            return (false);
        }

        public void betweenTurns(Player p1, Player p2)
        {
            p1.clearSickness();

            if (p1.front.status == 1) // Heal if paralyzed
            {
                p1.front.status = 0;
                Console.WriteLine(p1.front.ToString() + " is not longer paralyzed.");
            }

            p2.checkConditions();
        }

        public void initialPhase()
        {
            player1.draw(7);
            player2.draw(7);

            selectActive(player1);
            selectBenched(player1);

            selectActive(player2);
            selectBenched(player2);

            player1.putPrices();
            player2.putPrices();

        }

        public void selectActive(Player p1)
        {
            Console.WriteLine("Player " + p1.id + " select your active pokemon.");
            Console.WriteLine(p1.writeHand());
            bool correct = false;
            card selected = null;
            while (!correct)
            {
                selected = p1.hand[Convert.ToInt16(Console.ReadKey().KeyChar) - 49];
                if (selected.getSuperType() == 0)
                {
                    battler temp = (battler)selected;

                    if ( temp.type == 0 )
                        correct = true;
                    else
                        Console.WriteLine("The selected card is an evolution card, please select a basic pokemon card");
                }
                else
                {
                    Console.WriteLine("The selected card is not a pokemon card, please select a pokemon card");
                }
            }

            p1.hand.Remove(selected);
            p1.front = (battler)selected;
            Console.WriteLine(selected.ToString() + " selected as active pokemon");
        }

        public void selectBenched(Player p1)
        {
            Console.WriteLine("Player " + p1.id + " select any number of benched pokemon, press 0 to exit.");
            Console.WriteLine(p1.writeHand());
            bool correct = false;
            card selected;
            while (!correct)
            {
                int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;
                if (digit == -1)
                {
                    correct = true;
                    break;
                }
                selected = p1.hand[digit];
                if (selected.getSuperType() == 0)
                {
                    if (p1.benched.Count < 5)
                    {
                        battler temp = (battler)selected;

                        if (temp.type == 0)
                        {
                            p1.hand.Remove(selected);
                            p1.benched.Add((battler)selected);
                            Console.WriteLine(selected.ToString() + " selected as benched pokemon");
                            Console.WriteLine(p1.writeHand());
                        }
                        else
                            Console.WriteLine("The selected card is an evolution card, please select a basic pokemon card");
                    }
                    else
                        Console.WriteLine("You cannot bench more pokemon. Press 0 to end");
                }
                else
                {
                    Console.WriteLine("The selected card is not a pokemon card, please select a pokemon card or press 0 to exit");
                }
            }
        }
    }
}
