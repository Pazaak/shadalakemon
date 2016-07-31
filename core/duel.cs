using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    /* Holds information about the duel and implements the methods that control the battle
     * player1, player2- Holds the instance of the first and second player
     * energyLimit- States if an energy was played this turn or not
     */
    class duel
    {
        Player player1;
        Player player2;
        bool energyLimit;

        public duel(Player p1, Player p2)
        {
            player1 = p1;
            player2 = p2;
            utils.Logger.Clear();
        }

        // Controls the bucle of the duel
        public bool battleFlow()
        {
            this.initialPhase(); // Initial hands, placement of battlers and prices
            int result;

            while (true)
            {
                // Player 1
                energyLimit = false;
                result = mainPhase(player1, player2); // Main phase of the duel

                if (player1.winCondition) // Check for a winning player
                    return true;
                if (player2.winCondition)
                    return false;

                switch (result) 
                {
                    case -1: return false;
                    case 0: attackPhase(player1, player2); break; // Attack phase of the duel
                    case 1: break;
                }

                if (player1.winCondition) // Check for a winning player
                    return true;
                if (player2.winCondition)
                    return false;

                betweenTurns(player1, player2); // Between turns phase

                if (player1.winCondition) // Check for a winning player
                    return true;
                if (player2.winCondition)
                    return false;

                // Player 2
                energyLimit = false;
                result = mainPhase(player2, player1);

                if (player2.winCondition) // Check for a winning player
                    return false;
                if (player1.winCondition)
                    return true;

                switch (result)
                {
                    case -1: return false;
                    case 0: attackPhase(player2, player1); break;
                    case 1: break;
                }

                if (player2.winCondition) // Check for a winning player
                    return false;
                if (player1.winCondition)
                    return true;

                betweenTurns(player2, player1);

                if (player2.winCondition) // Check for a winning player
                    return false;
                if (player1.winCondition)
                    return true;
            }
        }

        // Enters the attack phase
        public void attackPhase(Player att, Player def)
        {
            def.lastFront = def.front;
            utils.Logger.Report("-> " + att.ToString() + "'s attack phase");
            Console.WriteLine("Player " + att.id);
            Console.WriteLine("Active pokemon: ");
            Console.WriteLine(att.front.BattleDescription());
            Console.WriteLine("Select a movement: ");
            att.front.execute(Convert.ToInt16(Console.ReadKey().KeyChar)-49, att, def, def.front); // Reads the movement selected by the user and executes it
        }

        // Controls the main phase of the selected player
        // Return codes:
        // -1: p1 lost
        // 0: Normal exit
        // 1: No attack exit
        public int mainPhase(Player p1, Player p2)
        {
            utils.Logger.Report("-> " + p1.ToString() + "'s main phase");

            if (!p1.draw(1))
            {
                Console.WriteLine(p1.ToString()+" cannot draw any more cards. "+ p1.ToString()+" losses.");
                utils.Logger.Report(p1.ToString() + " cannot draw any more cards. " + p1.ToString() + " losses.");
                return -1;
            }

            while (true)
            {
                Console.WriteLine("Player " + p1.id + " main phase.");
                Console.WriteLine("- 'a' to advance to attack phase");
                Console.WriteLine("- 'e' end the turn");
                Console.WriteLine("- 'r' retreat a pokemon");
                Console.WriteLine("- 'p' use pokemon power");
                Console.WriteLine(p1.writeHand());
                int digit = Convert.ToInt16(Console.ReadKey().KeyChar);

                switch (digit)
                {
                    case 97:
                        if (p1.front.status == 1 || p1.front.status == 2) // Statused
                            Console.WriteLine("Front pokemon is "+utilities.numToStatus(p1.front.status)+" and can't attack");
                        else if ( p1.front.status == 3 ) // Confusion check
                        {
                            Console.WriteLine("Confusion check:");
                            utils.Logger.Report("Confusion check:");
                            if (CRandom.RandomInt() < 0)
                            {
                                Console.WriteLine(p1.ToString() + " won the coin flip.");
                                utils.Logger.Report(p1.ToString() + " won the coin flip.");
                                utils.Logger.Report(p1.ToString() + " advances to attack phase.");
                                return 0;
                            }
                            else
                            {
                                Console.WriteLine(p1.ToString() + " lost the coin flip.");
                                utils.Logger.Report(p1.ToString() + " lost the coin flip.");
                                effects.damage(Constants.TNone, 30, p1.front, null, p1, p2);
                                utils.Logger.Report(p1.ToString() + " ends the turn without attacking.");
                                return 1;
                            }
                        }
                        else
                        {
                            utils.Logger.Report(p1.ToString() + " advances to attack phase.");
                            return 0; // Advance to attack phase
                        }
                        break;
                    case 101:
                        utils.Logger.Report(p1.ToString() + " ends the turn without attacking.");
                        return 1; // No attacks
                    case 114:
                        retreat(p1); // Retreat menu
                        break;
                    case 112:
                        PokemonPowerMenu(p1); //Power menu
                        break;
                    default:
                        playCard(p1.hand[digit - 49], p1); // Play selected card
                        break;
                }
            }
        }

        // Status check phase
        public void betweenTurns(Player p1, Player p2)
        {
            p1.clearSickness(); // Removes the summoning sicknes

            p1.ParalysisCheck();

            p1.SleepCheck();
            p2.SleepCheck();

            p1.PoisonCheck(p2);
            p2.PoisonCheck(p1);

            p1.checkConditions();
            p2.checkConditions(); // Decrease the counters of the conditions

            p1.ResetPowers(); // Resets the powers
        }

        // Initial phase of mulligans and initial hands
        public void initialPhase()
        {
            utils.Logger.Report("-> Initial Phase");

            // Both players have invalid hands
            while (!player1.checkInitialHand() && !player2.checkInitialHand())
            {
                if (player1.hand.Count > 0)
                    utils.Logger.Report("Both players have invalid hands");

                player1.ShuffleHand();
                player2.ShuffleHand();
                player1.draw(7);
                player2.draw(7);
                
            }

            // Both players have valid hands
            if (player1.checkInitialHand() && player2.checkInitialHand())
            {
                selectActive(player1);
                selectBenched(player1);
                player1.putPrices();


                selectActive(player2);
                selectBenched(player2);
                player2.putPrices();

                return;
            }

            // Player 1 has an invalid hand, player2 plays
            if (!player1.checkInitialHand())
            {
                utils.Logger.Report("Player 1 has an invalid hand.");
                selectActive(player2);
                selectBenched(player2);
                player2.putPrices();

                int mulligans = 0;
                while (!player1.checkInitialHand())
                {
                    mulligans += 1;
                    player1.ShuffleHand();
                    player1.draw(7);
                    if (!player1.checkInitialHand())
                        utils.Logger.Report("Player 1 has an invalid hand.");
                }

                // TODO: This must be optional
                Console.WriteLine("Player 2 draws " + mulligans + " additional card/s.");
                utils.Logger.Report("Player 1 took " + mulligans + (mulligans == 1? " mulligan." : " mulligans."));

                player2.draw(mulligans);

                selectActive(player1);
                selectBenched(player1);
                player1.putPrices();
            }
            // Player 2 has an invalid hand, player1 plays
            else
            {
                utils.Logger.Report("Player 2 has an invalid hand.");
                selectActive(player1);
                selectBenched(player1);
                player1.putPrices();

                int mulligans = 0;
                while (!player2.checkInitialHand())
                {
                    mulligans += 1;
                    player2.ShuffleHand();
                    player2.draw(7);
                    if (!player2.checkInitialHand())
                        utils.Logger.Report("Player 2 has an invalid hand.");
                }

                // TODO: This must be optional
                Console.WriteLine("Player 1 draws " + mulligans + " additional card/s.");
                utils.Logger.Report("Player 2 took " + mulligans + (mulligans == 1 ? " mulligan." : " mulligans."));


                player1.draw(mulligans);

                selectActive(player2);
                selectBenched(player2);
                player2.putPrices();
            }

        }

        // Selects the active battler
        public void selectActive(Player p1)
        {
            Console.WriteLine("Player " + p1.id + " select your active pokemon.");
            Console.WriteLine(p1.writeHand());
            bool correct = false;
            card selected = null;
            while (!correct) // If the selection is not correct, repeat the question
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
            p1.front = (battler)selected; // Play the card
            Console.WriteLine(selected.ToString() + " selected as active pokemon");
            utils.Logger.Report(p1.ToString() + " selects " +selected.ToString() + " as active pokemon");
        }

        // Selects benched battlers
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
                    if (p1.benched.Count < 5) // Checks if the bench is not full yet
                    {
                        battler temp = (battler)selected;

                        if (temp.type == 0)
                        {
                            p1.hand.Remove(selected);
                            p1.benched.Add((battler)selected);
                            Console.WriteLine(selected.ToString() + " selected as benched pokemon");
                            utils.Logger.Report(p1.ToString() + " selects " + selected.ToString() + " as benched pokemon");
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

        // Play cards front the hand
        public void playCard(card selected, Player p1)
        {
            if (selected.getSuperType() == 0) // Pokemon card
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
                            utils.Logger.Report(selected.ToString() + " selected as benched pokemon");
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
                            int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50; // Select the battler to evolve
                            if (digit == -2)
                            {
                                done = true;
                                break;
                            }

                            battler target;

                            if (digit == -1) // Select the receiver
                                target = p1.front;
                            else
                                target = p1.benched[digit];

                            if (target.id != newBattler.evolvesFrom) // Checks if the pokemon can evolve
                                Console.WriteLine("That pokemon cannot evolve in the selected one");
                            else if (target.sumSick)
                                Console.WriteLine("You cannot evolve a pokemon that was benched this turn");
                            else
                            {
                                newBattler.evolve(target); // Apply the evolution procedures
                                if (digit == -1)
                                    p1.front = newBattler;
                                else
                                    p1.benched[digit] = newBattler;
                                p1.hand.Remove(selected);
                                done = true;
                                Console.WriteLine(target.ToString() + " evolved into " + newBattler.ToString());
                                utils.Logger.Report(target.ToString() + " evolved into " + newBattler.ToString());
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
                if (energyLimit) // Check for energy limit
                    Console.WriteLine("You can only attach one energy per turn.");
                else
                {
                    energy attEnergy = (energy)selected;
                    Console.WriteLine("Please select the pokemon or press 0 to exit:");
                    Console.WriteLine(p1.writeBattlers());

                    int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49; // Select the receiver
                    if (digit == 0) // Attach the energy to the selected battler
                    {
                        p1.front.attachEnergy(attEnergy);
                        p1.hand.Remove(selected);
                        energyLimit = true;
                        Console.WriteLine(attEnergy.name+" attached to " + p1.front.ToString());
                        utils.Logger.Report(attEnergy.name + " attached to " + p1.front.ToString());
                    }
                    else if (digit > 0)
                    {
                        p1.benched[digit - 1].attachEnergy(attEnergy);
                        p1.hand.Remove(selected);
                        energyLimit = true;
                        Console.WriteLine(attEnergy.name + " attached to " + p1.benched[digit - 1].ToString());
                        utils.Logger.Report(attEnergy.name + " attached to " + p1.benched[digit - 1].ToString());
                    }
                }
            }
        }

        // Retreat menu
        public void retreat(Player p1)
        {
            if (!p1.front.canRetreat()) // Check if it can retreat
            {
                Console.WriteLine("Active pokemon has not enough energy to retreat");
                return;
            }

            if (p1.benched.Count == 0) // Check for bench limit
            {
                Console.WriteLine("There are not any pokemon in the bench");
                return;
            }

            if (p1.front.status == 1 || p1.front.status == 2) // Battler is statused
            {
                Console.WriteLine("The pokemon cannot retreat due "+utilities.numToStatus(p1.front.status));
                return;
            }

            int counter = p1.front.retreat;
            int digit;
            Console.WriteLine("Discard energy until you pay the cost");
            while (counter > 0) // Discard cards until the cost
            {
                Console.WriteLine(counter + " energy to go.");

                Console.WriteLine(p1.front.showEnergy());

                digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;

                counter -= p1.front.energies[digit].quan;

                p1.discardEnergy(p1.front, digit);
            }

            Console.WriteLine("Choose the benched pokemon to put in front.");
            
            Console.WriteLine(p1.writeBenched());
            digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49; // Choose the battler to put in front

            p1.ExchangePosition(digit);
        }

        // Power menu to select power
        public void PokemonPowerMenu(Player p1)
        {
            if (p1.ListPowers()) // Exit if there are no powers
            {
                int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50;

                if (digit == -1)
                    p1.front.ExecutePower(p1);
                else
                    p1.benched[digit].ExecutePower(p1);
            }
        }

    }
}
