using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    class effects
    {
        // A big-old switch which contains the necessary calls to execute movements 
        public static void move_selector(Player source_controller, Player target_controller, battler source, battler target, movement mov, int selector, int[] parameters, bool costless)
        {
            switch (selector)
            {
                case 0: // Simple damage
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    break;
                case 1: // Discard and heal
                    if (!costless)
                        discardEnergy(source_controller, source, source.element, parameters[0]);
                    heal(source, parameters[1]);
                    break;
                case 2: // Damage and coin for status
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        inflictStatus(target, parameters[1]);
                    }
                    else
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                    break;
                case 3: // Add condition by coin
                    int[] temp;
                    if (CRandom.RandomInt() < 0)
                    {
                        temp = new int[parameters.Length - 1];
                        Array.Copy(parameters, 1, temp, 0, temp.Length);
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        addCondition(source, parameters[0], temp);
                    }
                    else
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                    break;
                case 4: // Damage empowered by excess of energy
                    int exEner = ExcessEnergy(mov, source, parameters[2], costless);
                    damage(source.element, parameters[0] + (exEner > parameters[1] ? parameters[1] : exEner), target, source, target_controller, source_controller);
                    break;
                case 5: // Deactivate a movement
                    MoveDeactivator(target);
                    break;
                case 6: // Flip Q2 coins, do goodFlips(Q2)*Q1 damage
                    damage(source.element, FlipDamage(parameters[0], parameters[1]), target, source, target_controller, source_controller);
                    break;
                case 7: // Damage and discard
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    discardEnergy(target_controller, target, -1, 1);
                    break;
                case 8: // Damage equal the number of damage counters
                    damage(source.element, source.damage, target, source, target_controller, source_controller);
                    break;
                case 9: // Damage and wheel
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    Wheel(target_controller);
                    break;
                case 10: // Damage = Half of the remaining HP
                    int result = (target.HP - target.damage) / 2;
                    result += result % 10 == 5 ? 5 : 0;
                    damage(source.element, result, target, source, target_controller, source_controller);
                    break;
                case 11: // Change target weakness
                    ChangeWeakness(target);
                    break;
                case 12: // Change own resistance
                    ChangeResistance(source);
                    break;
                case 13: // Leek Slap!
                    damage(source.element, FlipDamage(30, 1), target, source, target_controller, source_controller);
                    source.leekSlap = true;
                    break;
                case 14: // Base set mirror movement
                    MirrorMove(source_controller, source, target_controller, target, parameters[0]);
                    break;
                case 15: // Execute a movement of the defender pokemon without paying costs
                    ExDefMovement(source_controller, target_controller, source, target);
                    break;
                case 16: // Status by coin
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        inflictStatus(target, parameters[0]);
                    }
                    else
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                    break;
                case 17: // Damage at both sides
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    damage(Constants.TNone, parameters[1], source, source, target_controller, source_controller);
                    break;
                case 18: // Discard to do damage
                    if (!costless)
                        discardEnergy(source_controller, source, parameters[1], parameters[2]);
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    break;
                case 19: // Wheel
                    Wheel(target_controller);
                    break;
                case 20: // Damage and status
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    inflictStatus(target, parameters[1]);
                    break;
                case 21: // Damage and one of two status by coin
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        inflictStatus(target, parameters[1]);
                    }
                    else
                    {
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                        inflictStatus(target, parameters[2]);
                    }
                    break;
                case 22: // Damage and leech
                    if (damage(source.element, parameters[0], target, source, target_controller, source_controller) > 0)
                        heal(source, parameters[1]);
                    break;
                case 23: // Extra damage or recoil
                    bool extraDamage;
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        extraDamage = true;
                    }
                    else
                    {
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                        extraDamage = false;
                    }
                    damage(source.element, extraDamage ? parameters[0] + parameters[1] : parameters[0], target, source, target_controller, source_controller);
                    if (!extraDamage) damage(Constants.TNone, parameters[2], source, source, target_controller, source_controller);
                    break;
                case 24: // Discard type and legacy [element, quantity, legacy, duration]
                    if (!costless)
                        discardEnergy(source_controller, source, parameters[0], parameters[1]);
                    temp = new int[parameters.Length - 3];
                    Array.Copy(parameters, 3, temp, 0, temp.Length);
                    addCondition(source, parameters[2], temp);
                    break;
                case 25: // Damage + opposite damage counters
                    damage(source.element, parameters[0] + target.damage, target, source, target_controller, source_controller);
                    break;
                case 26: // Status
                    inflictStatus(target, parameters[0]);
                    break;
                case 27: // Damage only while statused
                    if (target.status == parameters[1])
                        damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    else
                        Console.WriteLine(target.ToString() + " is not under " + utilities.numToStatus(parameters[1]));
                    break;
                case 28: // Damage empowered by the energy attached to the target
                    damage(source.element, parameters[0] + target.energies.Count * 10, target, source, target_controller, source_controller);
                    break;
                case 29: // Damage and legacy to the opponent
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    temp = new int[parameters.Length - 2];
                    Array.Copy(parameters, 2, temp, 0, temp.Length);
                    addCondition(target, parameters[1], temp);
                    break;
                case 30: // Legacy
                    temp = new int[parameters.Length - 1];
                    Array.Copy(parameters, 1, temp, 0, temp.Length);
                    addCondition(source, parameters[0], temp);
                    break;
                case 31: // Damage downpowered by damage
                    damage(source.element, parameters[0] - source.damage <= 0 ? 0 : parameters[0] - source.damage, target, source, target_controller, source_controller);
                    break;
                case 32: // Damage and splash to own benched
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    foreach (battler btl in source_controller.benched)
                        damage(Constants.TNone, parameters[1], btl, null, target_controller, source_controller);
                    break;
                case 33: // Damage, splash to all benches and recoil
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    foreach (battler btl in source_controller.benched)
                        damage(Constants.TNone, parameters[1], btl, source, source_controller, target_controller);
                    foreach (battler btl in target_controller.benched)
                        damage(Constants.TNone, parameters[1], btl, source, target_controller, source_controller);
                    damage(source.element, parameters[2], source, source, source_controller, target_controller);
                    break;
                case 34: // Damage and legacy on self by coin
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        addCondition(source, parameters[1], new int[1] { parameters[2] });
                    }
                    else
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                    break;

            }
        }

        // A big-old which that indicates the effects of the powers
        public static void power_selector(Player source_controller, Player target_controller, battler source, int selector, int[] parameters)
        {
            switch (selector)
            {
                case 0: // Rain dance
                    PlayFreeEnergy(source_controller, parameters[0]);
                    // Can be executed any number of times
                    break;
                case 1: // Energy trans
                    SwitchEnergySameType(source_controller, parameters[0]);
                    break;
                case 2: // Damage Swap
                    ExchangeDamage(source_controller);
                    break;
                case 3: // Buzzap
                    source_controller.ToDiscard(source);
                    source_controller.discarded.Remove(source);
                    target_controller.PriceProcedure();
                    BecomeEnergy(source, source_controller, parameters[0]);
                    source_controller.KnockoutProcedure(target_controller);
                    break;
            }
        }

        // A big-old which to hold the trainer effects
        public static bool trainers(Player source_controller, Player target_controller, trainer source, int selector, int[] parameters)
        {
            switch (selector)
            {
                case 0: // Proxy battler
                    CreateProxyBattler(source_controller, source, parameters[0], parameters[1]);
                    source_controller.hand.Remove(source);
                    return false;
                case 1: // Discard and retrieve from deck
                    card selected;
                    if (DiscardCards(source_controller, parameters[0], source))
                    {
                        selected = source_controller.SearchDeck(parameters[1]);
                        source_controller.hand.Add(selected);
                        source_controller.deck.Remove(selected);
                        Console.WriteLine(selected.ToString() + " added to the hand.");
                        utils.Logger.Report(selected.ToString() + " added to the hand.");
                        source_controller.shuffle();
                        return true;
                    }
                    return false;
                case 2: // Return the battler to the selected evolutive stage and remove status and legacies
                    devolution(source_controller, parameters[0]);
                    return true;
                case 3: // Shuffle hand in the deck and draw X cards
                    if (parameters[0] == 0) // Source player
                    {
                        source_controller.DiscardHand();
                        source_controller.draw(parameters[1]);
                        return false;
                    }
                    else // Target player
                    {
                        target_controller.DiscardHand();
                        target_controller.draw(parameters[1]);
                        return true;
                    }
                case 4: // Discard and retrieve from discard pile
                    if (DiscardCards(source_controller, parameters[0], source))
                    {
                        for (int i = 0; i < parameters[2]; i++)
                        {
                            selected = source_controller.SearchPile(parameters[1]);
                            if (selected != null)
                            {
                                source_controller.hand.Add(selected);
                                source_controller.discarded.Remove(selected);
                                Console.WriteLine(selected.ToString() + " added to the hand.");
                                utils.Logger.Report(selected.ToString() + " added to the hand.");
                            }
                        }
                        return true;
                    }
                    return false;
                case 5: // Search in hand for the cards of an specified type and shuffle them in the deck
                    ShuffleCardsType(source_controller, parameters[0]);
                    ShuffleCardsType(target_controller, parameters[0]);
                    return parameters[0] != 1; // If it's not a trainer remove from hand and put in the discard pile
                case 6: // Evolve jumping one stage
                    return JumpEvolution(source_controller);
                case 7: // Put card from hand in library and a card in library into hand
                    card toDeck = source_controller.SearchHand(parameters[0]);
                    card toHand = source_controller.SearchDeck(parameters[0]);
                    // TODO: Show both cards
                    source_controller.hand.Remove(toDeck);
                    source_controller.deck.AddFirst(toDeck);
                    source_controller.deck.Remove(toHand);
                    source_controller.hand.Add(toHand);
                    source_controller.shuffle();
                    return true;
                case 8: // Select a battler in play, discard everything and return the basic to hand
                    ScoopUp(source_controller, target_controller);
                    return true;
                case 9: // Discard one energy from one of the source's battler and two from one of the target's battlers
                    discardEnergy(source_controller, source_controller.SelectBattler(), parameters[0], parameters[1]);
                    discardEnergy(target_controller, target_controller.SelectBattler(), parameters[2], parameters[3]);
                    return true;
                case 10: // Add a legacy to front
                    int[] arguments = new int[parameters.Length];
                    for (int i = 1; i < arguments.Length; i++)
                        arguments[i] = parameters[i];
                    arguments[0] = 2;
                    addCondition(source_controller.benched[0], parameters[0], arguments);
                    return true;
                case 11: // Eliminate status conditions
                    source_controller.benched[0].status = 0;
                    Console.WriteLine(source_controller.benched[0].ToString() + " is now healed of status effects.");
                    utils.Logger.Report(source_controller.benched[0].ToString() + " is now healed of status effects.");
                    return true;
                case 12: // Shuffle X cards to draw X cards
                    if (ShuffleCards(source_controller, parameters[0], source))
                        source_controller.draw(parameters[1]);
                    return true;
                case 13: // Pokemon center
                    HealAndDiscardEnergy(source_controller);
                    return true;
                case 14: // basic pokémon from discard pile to bench
                    Player target;
                    if (parameters[0] == 0) target = source_controller;
                    else target = target_controller;

                    if ( target.benched.Count() == 5 )
                    {
                        Console.WriteLine(target.ToString()+" bench if full, you cannot play " + source.ToString());
                        return false;
                    }

                    battler target_battler;
                    do
                    {
                        target_battler = (battler) target.SearchPile(0);
                    }
                    while (target_battler != null && target_battler.type != 0);

                    target.benched.Add(target_battler);
                    target.discarded.Remove(target_battler);

                    if ( parameters[0] == 0)
                    {
                        int result = (target_battler.HP - target_battler.damage) / 2;
                        result -= result % 10 == 5 ? 5 : 0;
                        target_battler.damage = result;
                    }

                    return true;
                case 15: // Look top X cards and rearrange them
                    if ( source_controller.deck.Count < parameters[0])
                    {
                        Console.WriteLine("The deck has not enough cards to perform that action.");
                        return false;
                    }
                    TopXRearrange(source_controller, parameters[0]);
                    return true;
                case 16: // Discard and heal
                    bool end = false;
                    do
                    {
                        target_battler = source_controller.SelectBattler();
                        if (target_battler.energies.Count < parameters[0])
                            Console.WriteLine(target_battler.ToString() + " has not enough energy to discard.");
                        else
                            end = true;
                    } while (!end);

                    discardEnergy(source_controller, target_battler, Constants.TNone, parameters[0]);

                    heal(target_battler, parameters[1]);

                    return true;
                case 17: // Draw X cards
                    if (parameters[0] == 0) target = source_controller;
                    else target = target_controller;
                    target.draw(parameters[1]);
                    return true;
                case 18: // Discard energy from opponent
                    target_battler = target_controller.SelectBattler();
                    discardEnergy(target_controller, target_battler, parameters[0], parameters[1]);
                    return true;
                case 19: // Switch a pokémon of the selected player
                    if (parameters[0] == 0) target = source_controller;
                    else target = target_controller;
                    Wheel(target);
                    return true;
                case 20: // Heal target pokémon
                    target_battler = source_controller.SelectBattler();
                    heal(target_battler, parameters[0]);
                    return true;
                default: // Error
                    Console.WriteLine(" PANIC -------- NOT IMPLEMENTED");
                    return false;
            }
        }

        // Does plain damage taking into account conditions and type effectiveness
        public static int damage(int type, int quantity, battler target, battler source, Player target_controller, Player source_controller)
        {
            if (target.conditions.ContainsKey(Legacies.fog)) // Prevent damage condition
            {
                Console.WriteLine(target.ToString() + " is protected from damage.");
                utils.Logger.Report(target.ToString() + " is protected from damage.");
                return 0;
            }
            int output = quantity;
            if (type == target.weak_elem) output *= target.weak_mod; // Apply weaknesses
            else if (type == target.res_elem) output -= target.res_mod; // Apply resistances

            if (target.conditions.ContainsKey(Legacies.lowThreshold) && target.conditions[Legacies.lowThreshold][1] >= output)
            {
                Console.WriteLine(target.ToString() + " prevents the damage.");
                utils.Logger.Report(target.ToString() + " prevents the damage.");
                return 0;
            }

            if (source != null && source.conditions.ContainsKey(Legacies.damageAmplification))
            {
                output += source.conditions[Legacies.damageAmplification][1];
            }

            if (target.conditions.ContainsKey(Legacies.damageReduction))
            {
                output = output - target.conditions[Legacies.damageReduction][1];
                output = output < 0 ? 0 : output;
            }


            if (output > 0)
                target.damage += output;
            else
                output = 0;

            Console.WriteLine(target.ToString() + " received " + output + " points of damage.");
            utils.Logger.Report(target.ToString() + " received " + output + " points of damage.");

            // Check for knockout
            if (target.damage >= target.HP)
            {
                bool doomIndicator = false;
                bool priceless = false;
                if (source != null && target.conditions.ContainsKey(Legacies.destinyBound))
                    doomIndicator = true;

                if (target.conditions.ContainsKey(Legacies.clefairyDoll))
                    priceless = true;

                target_controller.ToDiscard(target);

                if (!priceless) source_controller.PriceProcedure();

                if (source_controller.winCondition) return output; // break

                if (target_controller.benched.Count > 0)
                    target_controller.KnockoutProcedure(source_controller);
                else
                {
                    Console.WriteLine(target_controller.ToString() + " has no benched pokemon. " + source_controller.ToString() + " wins."); // The player losses
                    utils.Logger.Report(target_controller.ToString() + " has no benched pokemon. " + source_controller.ToString() + " wins.");
                    source_controller.winCondition = true;
                }

                if (doomIndicator)
                    effects.damage(Constants.TNone, source.HP - source.damage, source, null, source_controller, target_controller);
            }

            if (output > 0 && target.conditions.ContainsKey(Legacies.counter) && target.CanUsePowers())
                effects.damage(Constants.TNone, target.conditions[Legacies.counter][1], source, null, source_controller, target_controller);

            return output;
        }

        // Effect to discard a card, as a cost or as an effect
        public static void discardEnergy(Player source_controller, battler source, int type, int quantity)
        {
            bool end;
            int digit;
            if (source.energies.Count == 0)
                Console.WriteLine(source.ToString() + " has no energy to discard.");

            if (quantity == -1)
            {
                while (source.energies.Count != 0)
                    source_controller.discardEnergy(source, 0);
                return;
            }

            for (int i = 0; i < quantity; i++)
            {
                if (source.energies.Count == 0)
                {
                    Console.WriteLine(source.ToString() + " has no more energy to discard.");
                    Console.WriteLine("Energy succcessfully discarded");
                    return;
                }

                Console.WriteLine("Select to discard " + utilities.numToType(type) + " energy card/s. " + (i + 1) + "/" + quantity);
                end = false;
                while (!end)
                {
                    Console.WriteLine(source.ShowEnergy());
                    digit = utils.ConsoleParser.ReadNumber(source.energies.Count-1);
                    if (source.energies[digit].elem == type || type == -1 || (type == Constants.TFire && source.conditions.ContainsKey(Legacies.energyBurn)))
                    {
                        source_controller.discardEnergy(source, digit);
                        end = true;
                    }

                }
            }
            Console.WriteLine("Energy succcessfully discarded");
        }

        // Effect to heal the source battler, 0 for full heal
        public static void heal(battler source, int quantity)
        {
            if (quantity == 0)
            {
                source.damage = 0;
                Console.WriteLine("Damage eliminated");
                utils.Logger.Report(source.ToString() + " has its damage removed.");
            }
            else
            {
                source.damage -= source.damage >= quantity ? quantity : source.damage;
                Console.WriteLine(source.ToString() + " has " + quantity + " damage removed.");
                utils.Logger.Report(source.ToString() + " has " + quantity + " damage removed.");
            }
        }

        // Assign an status alignment
        public static void inflictStatus(battler target, int type)
        {
            if (target.conditions.ContainsKey(Legacies.clefairyDoll))
            {
                Console.WriteLine(target.ToString() + " is inmune to status effects");
                return;
            }

            target.status = type;
            Console.WriteLine(target.ToString() + " is now " + utilities.numToStatus(type));
            utils.Logger.Report(target.ToString() + " is now " + utilities.numToStatus(type));

        }

        // Adds a condition
        public static void addCondition(battler source, int condition, int[] parameters)
        {
            if (source.conditions.ContainsKey(condition))
            {
                Console.WriteLine(source.ToString() + " has that condition.");
                return;
            }
            source.conditions.Add(condition, parameters);
            Console.WriteLine(source.ToString() + Legacies.IndexToLegacy(condition));
            utils.Logger.Report(source.ToString() + Legacies.IndexToLegacy(condition));
        }

        // Permits to exchange energy attached to one battler to other
        public static void SwitchEnergySameType(Player source_controller, int elem)
        {
            bool end = false;
            battler source;
            do
            {
                Console.WriteLine("Select a Pokemon with an energy card of the selected type");
                Console.WriteLine(source_controller.DisplayTypedEnergies(elem));

                int digit = utils.ConsoleParser.ReadOrExit(source_controller.benched.Count - 1);
                if (digit == -1) return;

                source = source_controller.SelectBattler(digit);

                if (source.energies.Count == 0)
                    Console.WriteLine("That pokemon has not enough energy cards");
                else end = true;
            }
            while (!end);
            

            int counter = 0;
            while (source.energies[counter].elem != elem)
                counter++;

            energy selected = source.energies[counter];

            battler target = source_controller.SelectBattler();

            source.energies.Remove(selected);
            target.attachEnergy(selected);

            Console.WriteLine(selected.name + " attached from " + source.ToString() + " to " + target.ToString());
            utils.Logger.Report(selected.name + " attached from " + source.ToString() + " to " + target.ToString());

        }

        // Checks for excesses of energy for a determined movement and adds it to the power
        public static int ExcessEnergy(movement mov, battler source, int type, bool costless)
        {
            if (costless) return source.energyTotal[type] * 10;

            int excess = source.energyTotal[type] - mov.cost[type];

            if (excess <= 0) return 0;

            int colorless = 0;

            for (int i = 0; i < mov.cost.Length; i++)
                if (i != type)
                    colorless += source.energyTotal[i];

            if (colorless >= mov.cost[0]) return excess * 10;

            return (excess + colorless - mov.cost[0]) * 10;
        }

        // Deactivates a movement
        public static void MoveDeactivator(battler target)
        {
            Console.WriteLine("Select the movement to deactivate:");
            for (int i = 0; i < target.movements.Length; i++)
                Console.WriteLine((i + 1) + "- " + target.movements[i].ToString());

            addCondition(target, Legacies.deacMov, new int[2] { 2, utils.ConsoleParser.ReadNumber(target.movements.Length -1) });
        }

        // Deals damage equals to the number of flips winned
        public static int FlipDamage(int quantity, int flips)
        {
            int multiplier = 0;

            for (int i = 0; i < flips; i++)
                if (CRandom.RandomInt() < 0)
                    multiplier++;

            Console.WriteLine(multiplier + " successful flips.");
            utils.Logger.Report(multiplier + " successful flips.");
            return quantity * multiplier;
        }

        // Wheels the front pokemon
        public static void Wheel(Player target)
        {
            if (target.benched.Count == 0)
            {
                Console.WriteLine("There are no pokemon in the bench.");
                return;
            }

            Console.WriteLine(target.ToString() + "- Select the pokemon to exchange:");
            Console.WriteLine(target.ShowBenched());
            int digit;
            do
            {
                digit = utils.ConsoleParser.ReadNumber(target.benched.Count - 1); // Choose the battler to put in front
                if (digit == 0)
                    Console.WriteLine("Not valid input. Try again.");
            }
            while (digit == 0);
            target.ExchangePosition(digit);
            Console.WriteLine("Pokemon exchanged.");
        }

        // Change weakness of target battler
        public static void ChangeWeakness(battler target)
        {
            if (target.weak_elem == Constants.TNone)
            {
                Console.WriteLine("Defending Pokemon has no weakness");
                utils.Logger.Report("Defending Pokemon has no weakness");
                return;
            }
            Console.WriteLine("Select the new type for the defending weakness: ");
            Console.WriteLine("1 - Water" + Environment.NewLine + "2 - Fire" + Environment.NewLine + "3 - Grass"
                + Environment.NewLine + "4 - Psychic" + Environment.NewLine + "5 - Fighting" + Environment.NewLine + "6 - Lightning");
            target.weak_elem = utils.ConsoleParser.ReadNumber(6);

            Console.WriteLine(target.ToString() + " is now weak to " + utilities.numToType(target.weak_elem) + ".");
            utils.Logger.Report(target.ToString() + " is now weak to " + utilities.numToType(target.weak_elem) + ".");
        }

        // Change resistance of target battler
        public static void ChangeResistance(battler target)
        {
            Console.WriteLine("Select the new type for the defending resistance: ");
            Console.WriteLine("1 - Water" + Environment.NewLine + "2 - Fire" + Environment.NewLine + "3 - Grass"
                + Environment.NewLine + "4 - Psychic" + Environment.NewLine + "5 - Fighting" + Environment.NewLine + "6 - Lightning");
            target.weak_elem = utils.ConsoleParser.ReadNumber(6);

            Console.WriteLine(target.ToString() + " is now resistant to " + utilities.numToType(target.res_elem) + ".");
            utils.Logger.Report(target.ToString() + " is now resistant to " + utilities.numToType(target.res_elem) + ".");
        }

        // Mirror move
        public static void MirrorMove(Player source_controller, battler source, Player target_controller, battler target, int parameter)
        {
            if (source_controller.lastFront != source_controller.benched[0])
            {
                Console.WriteLine(source_controller.benched[0].ToString() + " wasn't attacked last turn.");
                utils.Logger.Report(source_controller.benched[0].ToString() + " wasn't attacked last turn.");
                return;
            }

            // Damage phase
            string[] log = utils.Logger.FindCombatString(target_controller.ToString(), source_controller.ToString());

            foreach (string s in log)
            {
                if (s.Contains(source.ToString() + " received "))
                {
                    string[] temp = s.Split(' ');
                    damage(Constants.TNone, Int32.Parse(temp[2]), target, source, target_controller, source_controller);
                }
            }

            if (parameter == 1) // All the other effects (base set mirror move)
            {
                foreach (string s in log)
                {
                    if (s.Contains(source.ToString() + " had its energy "))
                        discardEnergy(target_controller, target, -1, 1);
                    if (s.Contains(source.ToString() + " is now "))
                    {
                        string[] temp = s.Split(' ');
                        inflictStatus(target, utilities.nameToStatus(temp[3]));
                    }
                    if (s.Contains(" attack deactivated.") && s.Contains(source.ToString()))
                        MoveDeactivator(target);
                    if (s.Contains(source.ToString() + " is now weak to "))
                        ChangeWeakness(target);
                }
            }
        }

        // Execute a movement of the defending pokemon
        public static void ExDefMovement(Player source_controller, Player target_controller, battler source, battler target)
        {
            Console.WriteLine(target.BattleDescription());
            Console.WriteLine("Select a movement: ");
            movement mov = target.movements[utils.ConsoleParser.ReadNumber(target.movements.Length)];
            mov.execute(source_controller, target_controller, source, target, true);
        }

        // Permits to play energy above the limit
        public static void PlayFreeEnergy(Player source, int elem)
        {
            bool end = false;
            energy selected = null;
            do
            {
                Console.WriteLine("Attach " + utilities.numToType(elem) + " a energy card from your hand to " + utilities.numToType(elem) + " pokémon. 'e' to exit");
                Console.WriteLine(source.ShowHand());
                int digit = utils.ConsoleParser.ReadOrExit(source.hand.Count-1);
                if (digit == -1) // Exit without selecting
                    return;
                if (source.hand[digit].getSuperType() != 2) // Not an energy card
                    Console.WriteLine("That isn't a energy card.");
                else
                {
                    selected = (energy)source.hand[digit];

                    if (selected.elem != elem)
                        Console.WriteLine("That isn't a " + utilities.numToType(elem) + " energy card.");
                    else
                        end = true;
                }
            }
            while (!end);

            do
            {
                Console.WriteLine("Select a " + utilities.numToType(elem) + " pokémon. 'e' to exit");
                Console.WriteLine(source.ShowBattlers());

                int digit = utils.ConsoleParser.ReadOrExit(source.benched.Count);

                if (digit == -1) // Exit without selecting
                    return;

                battler target = source.SelectBattler(digit);

                if (target.element != elem) // Not a pokémon of the selected type
                    Console.WriteLine("That isn't a " + utilities.numToType(elem) + " pokémon.");
                else
                {
                    target.attachEnergy(selected);
                    source.hand.Remove(selected);
                    return;
                }
            }
            while (true);            
        }

        // Exchange damage between battlers
        public static void ExchangeDamage(Player source_controller)
        {
            bool end = false;
            battler source;
            do
            {
                Console.WriteLine("Select a pokemon with damage counters on it: (Press 'e' to exit)");
                Console.WriteLine(source_controller.ShowDamage());

                int digit = utils.ConsoleParser.ReadOrExit(source_controller.benched.Count - 1);
                if (digit == -1) return; // exit

                source = source_controller.SelectBattler(digit); // select the source of the damage

                if (source.damage == 0) // Discard a selection with zero damage
                    Console.WriteLine("That pokemon hasn't damage counters on it");
                else
                    end = true;
            }
            while (!end);
            
            source.damage -= 10; // Substract the damage from source

            end = false;
            battler target;
            do
            {
                Console.WriteLine("Select a pokemon to put damage counters on it: (Press 'e' to exit)");
                Console.WriteLine(source_controller.ShowDamage());

                int digit = utils.ConsoleParser.ReadNumber(source_controller.benched.Count - 1);
                if (digit == -1) //  exit
                {
                    source.damage += 10;
                    return;
                }

                target = source_controller.SelectBattler(digit); // select the source of the damage

                if (target.damage + 10 == target.HP) // Discard a selection that will be knocked out
                    Console.WriteLine("That pokemon will be knocked out with further damage");
                else
                    end = true;
            }
            while (!end);

            // All clear, proceed to exchange damage
            target.damage += 10;
            Console.WriteLine(source.ToString() + " losses a damage counter. " + target.ToString() + " obtains a damage counter");
            utils.Logger.Report(source.ToString() + " losses a damage counter. " + target.ToString() + " obtains a damage counter");
        }

        // Transform into energy and attach it to a battler
        public static void BecomeEnergy(battler source, Player source_controller, int quantity)
        {
            Console.WriteLine("Select the type of energy that you want to produce: ");
            Console.WriteLine("0 - Normal" + Environment.NewLine + "1 - Water" + Environment.NewLine + "2 - Fire" + Environment.NewLine + "3 - Grass"
                + Environment.NewLine + "4 - Psychic" + Environment.NewLine + "5 - Fighting" + Environment.NewLine + "6 - Lightning");
            int elem = utils.ConsoleParser.ReadNumber(6); ;

            energy attEnergy = new energy(1, elem, quantity, source.name, source);

            Console.WriteLine("Please select the pokemon or press 0 to exit:");
            Console.WriteLine(source_controller.ShowBattlers());

            int digit = utils.ConsoleParser.ReadNumber(source_controller.benched.Count-1); // Select the receiver

            source_controller.benched[digit].attachEnergy(attEnergy);
            Console.WriteLine(attEnergy.name + " attached to " + source_controller.benched[digit].ToString());
            utils.Logger.Report(attEnergy.name + " attached to " + source_controller.benched[digit].ToString());
        }

        // Creates a proxy battler
        public static void CreateProxyBattler(Player source_player, trainer source, int HP, int Legacy)
        {
            if (source_player.benched.Count >= 5)
            {
                Console.WriteLine(source.ToString() + " cannot be played as the bench is already full");
                utils.Logger.Report(source.ToString() + " cannot be played as the bench is already full");
                return;
            }

            battler proxy = new battler(0, Constants.TNone, HP, Constants.TNone, 1, Constants.TNone, 0, 0, source.name, -2, -1, new movement[0], null, new int[1] { Legacies.clefairyDoll }, source);
            source_player.benched.Add(proxy);
        }

        // Discard cards from hand
        public static bool DiscardCards(Player target_player, int quantity, trainer source = null)
        {
            if (target_player.hand.Count() < quantity)
            {
                Console.WriteLine("Not enough cards in hand.");
                return false;
            }

            bool end;
            int digit;
            card selected;
            List<card> toDiscard = new List<card>();
            for (int i = 0; i < quantity; i++)
            {
                end = false;
                do
                {
                    Console.WriteLine("Select a card to discard (" + (i + 1) + "/" + quantity + "). Press 'e' to exit:");
                    Console.WriteLine(target_player.ShowHand());
                    digit = utils.ConsoleParser.ReadNumber(target_player.hand.Count - 1);
                    if (digit == -1) return false;
                    selected = target_player.hand[digit];
                    if ( (source == null || selected != source) && !toDiscard.Contains(selected))
                        end = true;
                    else
                        Console.WriteLine("You cannot select that card");
                }
                while (!end);

                toDiscard.Add(selected);
            }

            foreach (card ca in toDiscard)
                target_player.CardToDiscard(ca);

            return true;

        }

        // Handles the devolution procedure
        public static void devolution(Player source_controller, int steps)
        {
            bool end = false;
            battler target;
            int digit;
            do
            {
                Console.WriteLine("Select an active Pokémon. Press 'e' to exit:");
                Console.WriteLine(source_controller.ShowBattlers());
                digit = utils.ConsoleParser.ReadOrExit(source_controller.benched.Count - 1);
                if (digit == -1) return;
                target = source_controller.SelectBattler(digit);

                if (target.prevolution == null)
                    Console.WriteLine(target.ToString() + " is a basic pokemon.");
                else
                    end = true;

            }
            while (!end);
            

            int chain;
            if (target.prevolution.prevolution != null)
                chain = 2;
            else
                chain = 1;

            int stage_selected;
            if (steps == 0) // Return to the selected form
            {
                Console.WriteLine("Select the form to which the pokemon must return. Press 'e' to exit: ");
                Console.WriteLine(target.ShowEvolutions());

                stage_selected = utils.ConsoleParser.ReadOrExit(chain - 1);
                if (stage_selected == -1) return;
            }
            else
                stage_selected = chain - steps;

            battler newForm;
            if (stage_selected == 1 && chain == 2)
                newForm = target.prevolution.prevolution;
            else
                newForm = target.prevolution;

            battler middle = target.devolve(newForm);

            if (middle != null)
                source_controller.ToDiscard(middle);

            source_controller.ToDiscard(target);

            if (digit == 0)
                source_controller.benched[0] = newForm;
            else
                source_controller.benched.Add(newForm);
        }

        // Shuffles the selected cards from the hand in to the deck
        public static void ShuffleCardsType(Player target, int superType)
        {
            Console.WriteLine(target.ToString() + "'s hand.");
            Console.WriteLine(target.ShowHand());

            string buffer = "";
            List<card> ToShuffle = new List<card>();
            foreach (card crd in target.hand)
                if (crd.getSuperType() == superType)
                {
                    buffer += crd.ToString() + " ";
                    ToShuffle.Add(crd);
                }

            if (buffer.Length == 0)
                Console.WriteLine(target.ToString() + " has no cards of the selected type in hand.");
            else
                Console.WriteLine(buffer + "will be shuffled.");

            foreach (card crd in ToShuffle)
            {
                target.hand.Remove(crd);
                target.deck.AddFirst(crd);
            }

            target.shuffle();
        }

        // Evolves a basic pokemon jumping one stage
        public static bool JumpEvolution(Player target_source)
        {
            int digit;
            battler source = null;
            bool found = false;
            do
            {
                Console.WriteLine(target_source.ToString() + " choose an evolution card:");
                Console.WriteLine(target_source.ShowHand());
                digit = utils.ConsoleParser.ReadOrExit(target_source.hand.Count - 1);

                if (digit == -1) return false;

                if (target_source.hand[digit].getSuperType() == 0)
                {
                    source = (battler)target_source.hand[digit];
                    if (source.type == 1) found = true;
                }
            } while (!found); // Select an evolution card

            battler target;
            do
            {
                Console.WriteLine(target_source.ToString() + " a basic pokemon:");
                Console.WriteLine(target_source.ShowBattlers());
                digit = utils.ConsoleParser.ReadOrExit(target_source.benched.Count);

                if (digit == -1) return false;

                target = (battler)target_source.SelectBattler(digit);
            } while (source.id - 2 != target.id); // Select a matching basic card

            source.evolve(target);
            target_source.benched[digit] = source;

            target_source.hand.Remove(source);

            return true;
        }

        // Select a battler in play, discard everything and return the basic to hand
        public static void ScoopUp(Player target_controller, Player opponent)
        {
            int digit;

            Console.WriteLine("Select an active pokemon:");
            Console.WriteLine(target_controller.ShowBattlers());
            digit = utils.ConsoleParser.ReadOrExit(target_controller.benched.Count - 1);

            if (digit == -1) return;

            battler target = target_controller.SelectBattler(digit);
            if (digit == 0)
                target_controller.benched[0]= null;
            else
                target_controller.benched.Remove(target);

            battler temp;
            while (target.prevolution != null)
            {
                temp = target.prevolution;
                target.prevolution = null;
                target_controller.ToDiscard(target);
                target = temp;
            }

            target_controller.hand.Add(target);

            if (digit == 0) target_controller.KnockoutProcedure(opponent);
        }

        // Selects cards from the hand and shuffles them in the deck
        public static bool ShuffleCards(Player target_controller, int times, card source = null)
        {
            if (target_controller.hand.Count() < times)
            {
                Console.WriteLine("Not enough card in hand.");
                return false;
            }
            int digit;
            List<card> toShuffle = new List<card>();
            while (times != 0)
            {
                Console.WriteLine("Select a card from your hand to shuffle in the deck. Press 'e' to exit: ");
                Console.WriteLine(target_controller.ShowHand());
                digit = utils.ConsoleParser.ReadOrExit(target_controller.hand.Count-1);

                if (digit == -1) return false;

                card toDeck = target_controller.hand[digit];
                if ( (source == null || toDeck != source) && !toShuffle.Contains(toDeck))
                {
                    toShuffle.Add(toDeck);
                    times--;
                }
                else
                    Console.WriteLine("You cannot select the card.");
            }

            foreach (card ca in toShuffle)
            {
                target_controller.hand.Remove(ca);
                target_controller.deck.AddFirst(ca);
            }
                
            target_controller.shuffle();
            return true;
        }

        // Heal and discard all energy of the damaged battlers
        public static void HealAndDiscardEnergy(Player target)
        {
            List<battler> battlers = new List<battler>();
            battlers = battlers.Concat<battler>(target.benched).ToList<battler>();

            foreach ( battler btl in battlers)
            {
                if ( btl != null && btl.damage > 0 )
                {
                    btl.damage = 0;
                    foreach ( energy en in btl.energies)
                        target.discarded.Add(en);
                    for (int i = 0; i < btl.energyTotal.Length; i++)
                        btl.energyTotal[i] = 0;
                    btl.energies.Clear();
                    Console.WriteLine(btl.ToString() + " has its damage healed and energy discarded.");
                    utils.Logger.Report(btl.ToString() + " has its damage healed and energy discarded.");
                }
            }
        }

        // Look at the top X cards of the deck and rearrange them
        public static void TopXRearrange(Player target, int quantity)
        {
            card[] topX = new card[quantity];

            Console.WriteLine("Select cards from bottom to top: ");
            for (int i = 0; i < quantity; i++)
            {
                Console.WriteLine(i + "- " + target.deck.First().ToString());
                topX[i] = target.deck.First();
                target.deck.RemoveFirst();
            }

            int digit;
            for (int i = 0; i < quantity; i++)
            {
                do
                {
                    Int32.TryParse(Console.ReadLine(), out digit);
                    if (topX[digit] == null)
                        Console.WriteLine("Card already selected");
                }
                while (topX[digit] == null);
                target.deck.AddFirst(topX[digit]);
                topX[digit] = null; 
            }
        }

    }
}
