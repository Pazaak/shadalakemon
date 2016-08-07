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
        public static void trainers(Player source_controller, Player target_controller, trainer source, int selector, int[] parameters)
        {
            switch (selector)
            {
                case 0: // Proxy battler
                    CreateProxyBattler(source_controller, source, parameters[0], parameters[1]);
                    break;
                case 1: // Discard and retrieve from deck
                    card selected;
                    if (DiscardCards(source_controller, parameters[0]))
                    {
                        selected = SearchDeck(source_controller, parameters[1]);
                        source_controller.hand.Add(selected);
                        source_controller.deck.Remove(selected);
                        Console.WriteLine(selected.ToString() + " added to the hand.");
                        utils.Logger.Report(selected.ToString() + " added to the hand.");
                        source_controller.shuffle();
                    }
                    source_controller.TrainerToDiscard(source);
                    break;
                case 2: // Return the battler to the selected evolutive stage and remove status and legacies
                    devolution(source_controller, parameters[0]);
                    source_controller.TrainerToDiscard(source);
                    break;
                case 3: // Shuffle hand in the deck and draw X cards
                    if (parameters[0] == 0) // Source player
                    {
                        source_controller.ShuffleHand();
                        source_controller.draw(parameters[1]);
                    }
                    else // Target player
                    {
                        target_controller.ShuffleHand();
                        target_controller.draw(parameters[1]);
                    }
                    source_controller.TrainerToDiscard(source);
                    break;
                case 4: // Discard and retrieve from discard pile
                    if (DiscardCards(source_controller, parameters[0]))
                    {
                        for (int i = 0; i < parameters[2]; i++)
                        {
                            selected = SearchPile(source_controller, parameters[1]);
                            if (selected != null)
                            {
                                source_controller.hand.Add(selected);
                                source_controller.discarded.Remove(selected);
                                Console.WriteLine(selected.ToString() + " added to the hand.");
                                utils.Logger.Report(selected.ToString() + " added to the hand.");
                            }
                        }
                    }
                    source_controller.TrainerToDiscard(source);
                    break;
                case 5: // Search in hand for the cards of an specified type and shuffle them in the deck
                    ShuffleCardsType(source_controller, parameters[0]);
                    ShuffleCardsType(target_controller, parameters[0]);
                    source_controller.TrainerToDiscard(source);
                    break;
                case 6: // Evolve jumping one stage
                    JumpEvolution(source_controller);
                    source_controller.TrainerToDiscard(source);
                    break;
                case 7: // Put card from hand in library and a card in library into hand
                    card toDeck = SearchHand(source_controller, parameters[0]);
                    card toHand = SearchDeck(source_controller, parameters[0]);
                    // TODO: Show both cards
                    source_controller.hand.Remove(toDeck);
                    source_controller.deck.Add(toDeck);
                    source_controller.deck.Remove(toHand);
                    source_controller.hand.Add(toHand);
                    source_controller.shuffle();
                    source_controller.TrainerToDiscard(source);
                    break;
                case 8: // Select a battler in play, discard everything and return the basic to hand
                    ScoopUp(source_controller, target_controller);
                    source_controller.TrainerToDiscard(source);
                    break;
                case 9: // Discard one energy from one of the source's battler and two from one of the target's battlers
                    discardEnergy(source_controller, source_controller.SelectBattler(), parameters[0], parameters[1]);
                    discardEnergy(target_controller, target_controller.SelectBattler(), parameters[2], parameters[3]);
                    source_controller.TrainerToDiscard(source);
                    break;
                case 10: // Add a legacy to front
                    int[] arguments = new int[parameters.Length];
                    for (int i = 1; i < arguments.Length; i++)
                        arguments[i] = parameters[i];
                    arguments[0] = 2;
                    addCondition(source_controller.front, parameters[0], arguments);
                    source_controller.TrainerToDiscard(source);
                    break;
                case 11: // Eliminate status conditions
                    source_controller.front.status = 0;
                    Console.WriteLine(source_controller.front.ToString() + " is now healed of status effects.");
                    utils.Logger.Report(source_controller.front.ToString() + " is now healed of status effects.");
                    source_controller.TrainerToDiscard(source);
                    break;
                case 12: // Shuffle X cards to draw X cards
                    if (ShuffleCards(source_controller, parameters[0]))
                        source_controller.draw(parameters[1]);
                    source_controller.TrainerToDiscard(source);
                    break;
                case 13: // Pokemon center
                    HealAndDiscardEnergy(source_controller);
                    source_controller.TrainerToDiscard(source);
                    break;
                case 14: // Pokemon flute
                    if ( target_controller.benched.Count() == 5 )
                    {
                        Console.WriteLine("Opponent's bench if full, you cannot play " + source.ToString());
                        source_controller.hand.Add(source);
                        return;
                    }

                    battler target_battler;
                    do
                    {
                        target_battler = (battler) SearchPile(target_controller, 0);
                    }
                    while (target_battler != null && target_battler.type != 0);

                    target_controller.benched.Add(target_battler);
                    target_controller.discarded.Remove(target_battler);

                    source_controller.TrainerToDiscard(source);
                    break;
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
                    Console.WriteLine(source.showEnergy());
                    digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;
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
                source.damage -= source.damage >= quantity ? quantity : 0;
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
            Console.WriteLine("Select a Pokemon with an energy card of the selected type");
            source_controller.DisplayTypedEnergies(elem);

            int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50;
            battler source = null;

            if (digit == -1)
                source = source_controller.front;
            else
                source = source_controller.benched[digit];

            if (source.energies.Count == 0)
            {
                Console.WriteLine("That pokemon has not enough energy cards");
                return;
            }

            int counter = 0;
            while (source.energies[counter].elem != elem)
                counter++;

            energy selected = source.energies[counter];
            source.energies.RemoveAt(counter);

            Console.WriteLine("Select a Pokemon to attach the energy");
            source_controller.DisplayTypedEnergies(elem);

            digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50;

            battler target = null;

            if (digit == -1)
                target = source_controller.front;
            else
                target = source_controller.benched[digit];

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

            addCondition(target, Legacies.deacMov, new int[2] { 2, Convert.ToInt16(Console.ReadKey().KeyChar) - 49 });
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
            Console.WriteLine(target.writeBenched());
            target.ExchangePosition(Convert.ToInt16(Console.ReadKey().KeyChar) - 49);
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
            target.weak_elem = Convert.ToInt16(Console.ReadKey().KeyChar) - 48;

            Console.WriteLine(target.ToString() + " is now weak to " + utilities.numToType(target.weak_elem) + ".");
            utils.Logger.Report(target.ToString() + " is now weak to " + utilities.numToType(target.weak_elem) + ".");
        }

        // Change resistance of target battler
        public static void ChangeResistance(battler target)
        {
            Console.WriteLine("Select the new type for the defending resistance: ");
            Console.WriteLine("1 - Water" + Environment.NewLine + "2 - Fire" + Environment.NewLine + "3 - Grass"
                + Environment.NewLine + "4 - Psychic" + Environment.NewLine + "5 - Fighting" + Environment.NewLine + "6 - Lightning");
            target.res_elem = Convert.ToInt16(Console.ReadKey().KeyChar) - 48;

            Console.WriteLine(target.ToString() + " is now resistant to " + utilities.numToType(target.res_elem) + ".");
            utils.Logger.Report(target.ToString() + " is now resistant to " + utilities.numToType(target.res_elem) + ".");
        }

        // Mirror move
        public static void MirrorMove(Player source_controller, battler source, Player target_controller, battler target, int parameter)
        {
            if (source_controller.lastFront != source_controller.front)
            {
                Console.WriteLine(source_controller.front.ToString() + " wasn't attacked last turn.");
                utils.Logger.Report(source_controller.front.ToString() + " wasn't attacked last turn.");
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
            movement mov = target.movements[Convert.ToInt16(Console.ReadKey().KeyChar) - 49];
            mov.execute(source_controller, target_controller, source, target, true);
        }

        // Permits to play energy above the limit
        public static void PlayFreeEnergy(Player source, int elem)
        {
            Console.WriteLine("Attach " + utilities.numToType(elem) + " a energy card from your hand to " + utilities.numToType(elem) + " pokémon. 0 to exit");
            Console.WriteLine(source.writeHand());

            int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;

            if (digit == -1) // Exit without selecting
                return;

            if (source.hand[digit].getSuperType() != 2) // Not an energy card
            {
                Console.WriteLine("That isn't a energy card.");
                return;
            }

            energy selected = (energy)source.hand[digit];

            if (selected.elem != elem)
            {
                Console.WriteLine("That isn't a " + utilities.numToType(elem) + " energy card.");
                return;
            }

            Console.WriteLine("Select a " + utilities.numToType(elem) + " pokémon. 0 to exit");
            Console.WriteLine(source.writeBattlers());

            digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50;

            if (digit == -2) // Exit without selecting
                return;

            battler target = null;

            if (digit == -1) target = source.front;
            else target = source.benched[digit];

            if (target.element != elem) // Not a pokémon of the selected type
            {
                Console.WriteLine("That isn't a " + utilities.numToType(elem) + " pokémon.");
                return;
            }

            target.attachEnergy(selected);
            source.hand.Remove(selected);
        }

        // Exchange damage between battlers
        public static void ExchangeDamage(Player source_controller)
        {
            Console.WriteLine("Select a pokemon with damage counters on it: (Press 0 to exit)");
            Console.WriteLine(source_controller.ShowDamage());

            int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50; // selection
            if (digit == -2) return; // 0 equals exit

            battler source; // select the source of the damage
            if (digit == -1) source = source_controller.front;
            else source = source_controller.benched[digit];

            if (source.damage == 0) // Discard a selection with zero damage
            {
                Console.WriteLine("That pokemon hasn't damage counters on it");
                return;
            }

            source.damage -= 10; // Substract the damage from source

            Console.WriteLine("Select a pokemon to put damage counters on it: (Press 0 to exit)");
            Console.WriteLine(source_controller.ShowDamage());

            digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50; // selection
            if (digit == -2) // 0 equals exit
            {
                source.damage += 10;
                return;
            }

            battler target; // select the source of the damage
            if (digit == -1) target = source_controller.front;
            else target = source_controller.benched[digit];

            if (target.damage + 10 == target.HP) // Discard a selection that will be knocked out
            {
                source.damage += 10;
                Console.WriteLine("That pokemon will be knocked out with further damage");
                return;
            }

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
            int elem = Convert.ToInt16(Console.ReadKey().KeyChar) - 48;

            energy attEnergy = new energy(1, elem, quantity, source.name, source);

            Console.WriteLine("Please select the pokemon or press 0 to exit:");
            Console.WriteLine(source_controller.writeBattlers());

            int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49; // Select the receiver
            if (digit == 0) // Attach the energy to the selected battler
            {
                source_controller.front.attachEnergy(attEnergy);
                Console.WriteLine(attEnergy.name + " attached to " + source_controller.front.ToString());
                utils.Logger.Report(attEnergy.name + " attached to " + source_controller.front.ToString());
            }
            else if (digit > 0)
            {
                source_controller.benched[digit - 1].attachEnergy(attEnergy);
                Console.WriteLine(attEnergy.name + " attached to " + source_controller.benched[digit - 1].ToString());
                utils.Logger.Report(attEnergy.name + " attached to " + source_controller.benched[digit - 1].ToString());
            }
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
        public static bool DiscardCards(Player target_player, int quantity)
        {
            if (target_player.hand.Count() < quantity)
            {
                Console.WriteLine("Not enough cards in hand.");
                return false;
            }

            int digit;
            for (int i = 0; i < quantity; i++)
            {
                Console.WriteLine("Select a card to discard (" + (i + 1) + "/" + quantity + "):");
                Console.WriteLine(target_player.writeHand());
                Int32.TryParse(Console.ReadLine(), out digit);
                target_player.discarded.Add(target_player.hand[digit - 1]);
                utils.Logger.Report(target_player.hand[digit - 1].ToString() + " discarded from hand.");
                target_player.hand.RemoveAt(digit - 1);
            }

            return true;

        }

        // Search deck for a card
        public static card SearchDeck(Player target, int superType)
        {
            int digit;
            do
            {
                Console.WriteLine("Select a card from the deck:");
                target.ShowDeck();
                Int32.TryParse(Console.ReadLine(), out digit);
            } while (superType != -1 && target.deck[digit].getSuperType() != superType);
            return target.deck[digit];
        }

        // Handles the devolution procedure
        public static void devolution(Player source_controller, int steps)
        {
            Console.WriteLine("Select a pokemon:");
            Console.WriteLine(source_controller.writeBattlers());
            int digit;
            Int32.TryParse(Console.ReadLine(), out digit);

            battler target;
            if (digit == 1)
                target = source_controller.front;
            else
                target = source_controller.benched[digit - 2];

            if (target.prevolution == null)
            {
                Console.WriteLine(target.ToString() + " is a basic pokemon.");
                return;
            }

            int chain;
            if (target.prevolution.prevolution != null)
                chain = 2;
            else
                chain = 1;

            int stage_selected;
            if (steps == 0) // Return to the selected form
            {
                Console.WriteLine("Select the form to which the pokemon must return: ");
                Console.WriteLine(target.ShowEvolutions());

                Int32.TryParse(Console.ReadLine(), out stage_selected);
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

            if (digit == 1)
                source_controller.front = newForm;
            else
                source_controller.benched.Add(newForm);
        }

        // Search the discard file for a card
        public static card SearchPile(Player target, int superType = -1)
        {
            if (target.discarded.Count == 0)
            {
                Console.WriteLine("Discard pile is empty.");
                return null;
            }

            if (superType != -1) // Check if there are card of that type
            {
                bool flag = false;
                foreach (card ca in target.discarded)
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
                Console.WriteLine(target.ShowDiscardPile());
                Int32.TryParse(Console.ReadLine(), out digit);
            } while (superType != -1 && target.discarded[digit].getSuperType() != superType);
            return target.discarded[digit];
        }

        // Shuffles the selected cards from the hand in to the deck
        public static void ShuffleCardsType(Player target, int superType)
        {
            Console.WriteLine(target.ToString() + "'s hand.");
            Console.WriteLine(target.writeHand());

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
                target.deck.Add(crd);
            }

            target.shuffle();
        }

        // Evolves a basic pokemon jumping one stage
        public static void JumpEvolution(Player target_source)
        {
            int digit;
            battler source = null;
            bool found = false;
            do
            {
                Console.WriteLine(target_source.ToString() + " choose an evolution card:");
                Console.WriteLine(target_source.writeHand());
                Int32.TryParse(Console.ReadLine(), out digit);
                if (target_source.hand[digit - 1].getSuperType() == 0)
                {
                    source = (battler)target_source.hand[digit - 1];
                    if (source.type == 1) found = true;
                }
            } while (!found); // Select an evolution card

            battler target;
            do
            {
                Console.WriteLine(target_source.ToString() + " a basic pokemon:");
                Console.WriteLine(target_source.writeBattlers());
                Int32.TryParse(Console.ReadLine(), out digit);
                if (digit == 1)
                    target = (battler)target_source.front;
                else
                    target = (battler)target_source.benched[digit - 2];
            } while (source.id - 2 != target.id); // Select a matching basic card

            source.evolve(target);
            if (digit == 1)
                target_source.front = source;
            else
                target_source.benched[digit - 2] = source;

            target_source.hand.Remove(source);
        }

        // Searches a determined type of card in the hand
        public static card SearchHand(Player target, int superType)
        {
            int digit;
            do
            {
                Console.WriteLine("Select a card from the hand:");
                Console.WriteLine(target.writeHand());
                Int32.TryParse(Console.ReadLine(), out digit);
            } while (target.hand[digit-1].getSuperType() != superType);
            return target.hand[digit-1];
        }

        // Select a battler in play, discard everything and return the basic to hand
        public static void ScoopUp(Player target_controller, Player opponent)
        {
            int digit;
            Console.WriteLine("Select an active pokemon:");
            Console.WriteLine(target_controller.writeBattlers());
            Int32.TryParse(Console.ReadLine(), out digit);

            battler target;
            if (digit == 1)
            {
                target = target_controller.front;
                target_controller.front = null;
            }
            else
            {
                target = target_controller.benched[digit - 2];
                target_controller.benched.Remove(target);
            }

            battler temp;
            while (target.prevolution != null)
            {
                temp = target.prevolution;
                target.prevolution = null;
                target_controller.ToDiscard(target);
                target = temp;
            }

            target_controller.hand.Add(target);

            if (digit == 1) target_controller.KnockoutProcedure(opponent);
        }

        // Selects cards from the hand and shuffles them in the deck
        public static bool ShuffleCards(Player target_controller, int times)
        {
            if (target_controller.hand.Count() < times)
            {
                Console.WriteLine("Not enough card in hand.");
                return false;
            }
            int digit;
            while ( times-- != 0)
            {
                Console.WriteLine("Select a card from your hand to shuffle in the deck: ");
                Console.WriteLine(target_controller.writeHand());
                Int32.TryParse(Console.ReadLine(), out digit);
                card toDeck = target_controller.hand[digit - 1];
                target_controller.hand.Remove(toDeck);
                target_controller.deck.Add(toDeck);
            }
            target_controller.shuffle();
            return true;
        }

        // Heal and discard all energy of the damaged battlers
        public static void HealAndDiscardEnergy(Player target)
        {
            List<battler> battlers = new List<battler>();
            battlers.Add(target.front);
            if (target.benched.Count > 0)
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
    }
}
