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
            def.lastFront = def.benched[0];
            utils.Logger.Report("-> " + att.ToString() + "'s attack phase");
            Console.WriteLine("Player " + att.id);
            Console.WriteLine("Active pokemon: ");
            Console.WriteLine(att.benched[0].BattleDescription());
            Console.WriteLine("Select a movement: ");
            att.benched[0].execute(utils.ConsoleParser.ReadNumber(att.benched[0].movements.Length-1), att, def, def.benched[0]); // Reads the movement selected by the user and executes it
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
                if (p1.winCondition || p2.winCondition) return 1;

                Console.WriteLine("Player " + p1.id + " main phase.");
                Console.WriteLine("- 'a' to advance to attack phase");
                Console.WriteLine("- 'e' end the turn");
                Console.WriteLine("- 'r' retreat a pokemon");
                Console.WriteLine("- 'p' use pokemon power");
                Console.WriteLine(p1.ShowHand());

                int digit = utils.ConsoleParser.ReadFull(p1.hand.Count-1);
                switch (digit)
                {
                    case -2:
                        if (p1.benched[0].status == 1 || p1.benched[0].status == 2) // Statused
                            Console.WriteLine("Front pokemon is "+utilities.numToStatus(p1.benched[0].status)+" and can't attack");
                        else if ( p1.benched[0].status == 3 ) // Confusion check
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
                                effects.damage(Constants.TNone, 30, p1.benched[0], null, p1, p2);
                                utils.Logger.Report(p1.ToString() + " ends the turn without attacking.");
                                return 1;
                            }
                        }
                        else if (p1.benched[0].movements.Count() == 0)
                        {
                            Console.WriteLine(p1.benched[0].ToString() + " has no attacks.");
                            return 1;
                        }
                        else
                        {
                            utils.Logger.Report(p1.ToString() + " advances to attack phase.");
                            return 0; // Advance to attack phase
                        }
                        break;
                    case -1:
                        utils.Logger.Report(p1.ToString() + " ends the turn without attacking.");
                        return 1; // No attacks
                    case -3:
                        retreat(p1, p2); // Retreat menu
                        break;
                    case -4:
                        PokemonPowerMenu(p1, p2); //Power menu
                        break;
                    default:
                        playCard(p1.hand[digit], p1, p2); // Play selected card
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
            Console.WriteLine(p1.ShowHand());
            bool correct = false;
            card selected = null;
            while (!correct) // If the selection is not correct, repeat the question
            {
                selected = p1.hand[utils.ConsoleParser.ReadNumber(p1.hand.Count-1)];
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
            p1.benched.Add((battler)selected); // Play the card
            Console.WriteLine(selected.ToString() + " selected as active pokemon");
            utils.Logger.Report(p1.ToString() + " selects " +selected.ToString() + " as active pokemon");
        }

        // Selects benched battlers
        public void selectBenched(Player p1)
        {
            Console.WriteLine("Player " + p1.id + " select any number of benched pokemon, press 'e' to exit.");
            Console.WriteLine(p1.ShowHand());
            bool correct = false;
            card selected;
            while (!correct)
            {
                int digit = utils.ConsoleParser.ReadOrExit(p1.hand.Count - 1);
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
                            Console.WriteLine(p1.ShowHand());
                        }
                        else
                            Console.WriteLine("The selected card is an evolution card, please select a basic pokemon card");
                    }
                    else
                        Console.WriteLine("You cannot bench more pokemon. Press 'e' to end");
                }
                else
                {
                    Console.WriteLine("The selected card is not a pokemon card, please select a pokemon card or press 'e' to exit");
                }
            }
        }

        // Play cards front the hand
        public void playCard(card selected, Player p1, Player p2)
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
                            Console.WriteLine(p1.ShowBattlers());
                            int digit = utils.ConsoleParser.ReadOrExit(p1.benched.Count - 1);
                            if (digit == -1)
                            {
                                done = true;
                                break;
                            }

                            battler target = p1.SelectBattler(digit);

                            if (target.id != newBattler.evolvesFrom) // Checks if the pokemon can evolve
                                Console.WriteLine("That pokemon cannot evolve in the selected one");
                            else if (target.sumSick)
                                Console.WriteLine("You cannot evolve a pokemon that was benched this turn");
                            else
                            {
                                newBattler.evolve(target); // Apply the evolution procedures
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
                trainer trn = (trainer)selected;
                trn.execute(p1, p2);
            }
            else // Energy
            {
                if (energyLimit) // Check for energy limit
                    Console.WriteLine("You can only attach one energy per turn.");
                else
                {
                    energy attEnergy = (energy)selected;
                    battler target = p1.SelectBattler();
                    if (target == null) return;


                    target.attachEnergy(attEnergy);
                    p1.hand.Remove(selected);
                    energyLimit = true;
                    Console.WriteLine(attEnergy.name + " attached to " + target.ToString());
                    utils.Logger.Report(attEnergy.name + " attached to " + target.ToString());
                }
            }
        }

        // Retreat menu
        public void retreat(Player p1, Player p2)
        {
            if (!p1.benched[0].canRetreat()) // Check if it can retreat
            {
                Console.WriteLine("Active pokemon has not enough energy to retreat");
                return;
            }

            if (p1.benched.Count == 0) // Check for bench limit
            {
                Console.WriteLine("There are not any pokemon in the bench");
                return;
            }

            if (p1.benched[0].status == 1 || p1.benched[0].status == 2) // Battler is statused
            {
                Console.WriteLine("The pokemon cannot retreat due "+utilities.numToStatus(p1.benched[0].status));
                return;
            }

            if (p1.benched[0].conditions.ContainsKey(Legacies.clefairyDoll))
            {
                Console.WriteLine(p1.benched[0].ToString() + " will be discarded.");
                p1.ToDiscard(p1.benched[0]);
                p1.benched[0] = null;
                p1.KnockoutProcedure(p2);
                return;
            }

            int counter = p1.benched[0].retreat;
            int digit;
            if ( counter > 0)
                Console.WriteLine("Discard energy until you pay the cost");
            while (counter > 0) // Discard cards until the cost
            {
                Console.WriteLine(counter + " energy to go.");

                Console.WriteLine(p1.benched[0].showEnergy());

                digit = utils.ConsoleParser.ReadNumber(p1.benched[0].energies.Count-1);

                counter -= p1.benched[0].energies[digit].quan;

                p1.discardEnergy(p1.benched[0], digit);
            }

            Console.WriteLine("Choose the benched pokemon to put in front.");
            
            Console.WriteLine(p1.ShowBenched());

            do
            {
                digit = utils.ConsoleParser.ReadNumber(p1.benched.Count - 1); // Choose the battler to put in front
                if ( digit == 0)
                    Console.WriteLine("Not valid input. Try again.");
            }
            while (digit == 0);
            

            p1.ExchangePosition(digit);
        }

        // Power menu to select power
        public void PokemonPowerMenu(Player p1, Player p2)
        {
            if (p1.ListPowers()) // Exit if there are no powers
            {
                int digit = utils.ConsoleParser.ReadOrExit(p1.benched.Count-1);

                if (digit == -1) return;

                p1.benched[digit].ExecutePower(p1, p2);
            }
        }

    }
}
