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
                    damage(source.element, parameters[0], target); 
                    break;
                case 1: // Discard and heal
                    if ( !costless )
                        discardEnergy(source_controller, source, source.element, parameters[0]);
                    heal(source, parameters[1]);
                    break;
                case 2: // Damage and coin for status
                    damage(source.element, parameters[0], target);
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        inflictStatus(target, parameters[1]);
                    }
                    else
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                    break;
                case 3: // Add condition by coin
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        addCondition(source, parameters[0], parameters[1]); 
                    }
                    else
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                    break;
                case 4: // Damage empowered by excess of energy
                    int exEner = ExcessEnergy(mov, source, parameters[2], costless);
                    damage(source.element, parameters[0] + (exEner > parameters[1] ? parameters[1] : exEner), target);
                    break;
                case 5: // Deactivate a movement
                    MoveDeactivator(target);
                    break;
                case 6: // Flip Q2 coins, do goodFlips(Q2)*Q1 damage
                    FlipDamage(target, source.element, parameters[0], parameters[1]);
                    break;
                case 7: // Damage and discard
                    damage(source.element, parameters[0], target);
                    discardEnergy(target_controller, target, -1, 1);
                    break;
                case 8: // Damage equal the number of damage counters
                    damage(source.element, source.damage, target);
                    break;
                case 9: // Damage and wheel
                    damage(source.element, parameters[0], target);
                    Wheel(target_controller);
                    break;
                case 10: // Damage = Half of the remaining HP
                    int result = (target.HP - target.damage) / 2;
                    result += result % 10 == 5 ? 5 : 0;
                    damage(source.element, result, target);
                    break;
                case 11: // Change target weakness
                    ChangeWeakness(target);
                    break;
                case 12: // Change own resistance
                    ChangeResistance(source);
                    break;
                case 13: // Leek Slap!
                    FlipDamage(target, source.element, 30, 1);
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
                    damage(source.element, parameters[0], target);
                    damage(source.element, parameters[1], source);
                    break;
                case 18: // Discard to do damage
                    if (!costless)
                        discardEnergy(source_controller, source, parameters[1], parameters[2]);
                    damage(source.element, parameters[0], target);
                    break;
                case 19: // Wheel
                    Wheel(target_controller);
                    break;
                case 20: // Damage and status
                    damage(source.element, parameters[0], target);
                    inflictStatus(target, parameters[1]);
                    break;
                case 21: // Damage and one of two status by coin
                    damage(source.element, parameters[0], target);
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

            }
        }

        // A big-old which that indicates the effects of the powers
        public static void power_selector(Player source_controller, battler source, int selector, int[] parameters)
        {
            switch (selector)
            {
                case 0: // Rain dance
                    PlayFreeEnergy(source_controller, parameters[0]);
                    // Can be executed any number of times
                    break;
            }
        }

        // Does plain damage taking into account conditions and type effectiveness
        public static void damage(int type, int quantity, battler target)
        {
            if (target.conditions.ContainsKey(Legacies.fog)) // Prevent damage condition
            {
                Console.WriteLine(target.ToString() + " is protected from damage.");
                utils.Logger.Report(target.ToString() + " is protected from damage.");
                return;
            }
            int output = quantity;
            if (type == target.weak_elem) output *= target.weak_mod; // Apply weaknesses
            else if (type == target.res_elem) output -= target.res_mod; // Apply resistances

            if (output > 0)
                target.damage += output;
            else
                output = 0;

            Console.WriteLine(target.ToString() + " received " + output + " points of damage.");
            utils.Logger.Report(target.ToString() + " received " + output + " points of damage.");
        }

        // Effect to discard a card, as a cost or as an effect
        public static void discardEnergy(Player source_controller, battler source, int type, int quantity)
        {
            bool end;
            int digit;
            if (source.energies.Count == 0)
                Console.WriteLine(source.ToString() + " has no energy to discard.");

            for ( int i = 0; i < quantity; i++)
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
                source.damage -= quantity;
                Console.WriteLine(source.ToString()+" has "+quantity+" damage removed.");
                utils.Logger.Report(source.ToString() + " has " + quantity + " damage removed.");
            }
        }

        // Assign an status alignment
        public static void inflictStatus(battler target, int type)
        {
            target.status = type;
            Console.WriteLine(target.ToString() + " is now " + utilities.numToStatus(type));
            utils.Logger.Report(target.ToString() + " is now " + utilities.numToStatus(type));

        }

        // Adds a condition
        public static void addCondition(battler source, int condition, int duration)
        {
            source.conditions.Add(condition, duration);
            Console.WriteLine(source.ToString()+Legacies.IndexToLegacy(condition));
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
            while (!source.energies[counter].name.Equals("Water Energy")) // TODO: Only can take Water Energy
                counter++;

            energy selected = source.energies[counter];
            source.energies.RemoveAt(counter);

            Console.WriteLine("Select a Pokemon to attach the energy");
            source_controller.DisplayTypedEnergies(elem);

            digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50;

            battler target = null;

            if (digit == -1 && source_controller.front.element == elem)
                target = source_controller.front;
            else if (source_controller.benched[digit].element == elem)
                target = source_controller.benched[digit];
            else
            {
                Console.WriteLine("This pokemon is not " + utilities.numToType(elem) + " type.");
                return;
            }

            target.attachEnergy(selected);

            Console.WriteLine(selected.name + " attached from " + source.ToString() + " to " + target.ToString());
            utils.Logger.Report(selected.name + " attached from " + source.ToString() + " to " + target.ToString());

        }

        // Checks for excesses of energy for a determined movement and adds it to the power
        public static int ExcessEnergy(movement mov, battler source, int type, bool costless)
        {
            if (costless) return source.energyTotal[type]*10;

            int excess = source.energyTotal[type] - mov.cost[type];

            if (excess <= 0) return 0;

            int colorless = 0;

            for (int i = 0; i < mov.cost.Length; i++)
                if (i != type)
                    colorless += source.energyTotal[i];

            if (colorless >= mov.cost[0]) return excess * 10;

            return (excess + colorless - mov.cost[0])*10;
        }

        // Deactivates a movement
        public static void MoveDeactivator(battler target)
        {
            Console.WriteLine("Select the movement to deactivate:");
            for (int i = 0; i < target.movements.Length; i++)
                Console.WriteLine((i + 1) +"- "+ target.movements[i].ToString());

            addCondition(target, Convert.ToInt16(Console.ReadKey().KeyChar) - 48, 2);
        }

        // Deals damage equals to the number of flips winned
        public static void FlipDamage(battler target, int type, int quantity, int flips)
        {
            int multiplier = 0;

            for (int i = 0; i < flips; i++)
                if (CRandom.RandomInt() < 0)
                    multiplier++;

            Console.WriteLine(multiplier + " successful flips.");
            utils.Logger.Report(multiplier + " successful flips.");
            damage(type, quantity * multiplier, target);
        }

        // Wheels the front pokemon
        public static void Wheel(Player target)
        {
            if ( target.benched.Count == 0)
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
            if ( target.weak_elem == Constants.TNone )
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
            if ( source_controller.lastFront != source_controller.front )
            {
                Console.WriteLine(source_controller.front.ToString() + " wasn't attacked last turn.");
                utils.Logger.Report(source_controller.front.ToString() + " wasn't attacked last turn.");
                return;
            }

            // Damage phase
            string[] log = utils.Logger.FindCombatString(target_controller.ToString(), source_controller.ToString());

            foreach (string s in log)
            {
                if ( s.Contains(source.ToString() + " received ") )
                {
                    string[] temp = s.Split(' ');
                    damage(Constants.TNone, Int32.Parse(temp[2]), target);
                }
            }

            if ( parameter == 1 ) // All the other effects (base set mirror move)
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
                    if (s.Contains(source.ToString()+" is now weak to "))
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

        public static void PlayFreeEnergy(Player source, int elem)
        {
            Console.WriteLine("Attach " + utilities.numToType(elem) + " a energy card from your hand to " + utilities.numToType(elem) + " pokémon. 0 to exit");
            Console.WriteLine(source.writeHand());

            int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;

            if (digit == -1) // Exit without selecting
                return;

            if ( source.hand[digit].getSuperType() != 2) // Not an energy card
            {
                Console.WriteLine("That isn't a energy card.");
                return;
            }

            energy selected = (energy)source.hand[digit];

            if ( selected.elem != elem)
            {
                Console.WriteLine("That isn't a "+ utilities.numToType(elem)+" energy card.");
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
                Console.WriteLine("That isn't a "+utilities.numToType(elem)+" pokémon.");
                return;
            }

            target.attachEnergy(selected);
            source.hand.Remove(selected);
        }
    }
}
