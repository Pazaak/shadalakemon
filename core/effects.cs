﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    class effects
    {
        public static void move_selector(Player source_controller, Player target_controller, battler source, battler target, movement mov, int type, int selector, int quantity1, int quantity2)
        {
            switch (selector)
            {
                case 0: // Simple damage
                    damage(type, quantity1, target); 
                    break;
                case 1: // Discard and heal
                    discardEnergy(source_controller, source, type, quantity1);
                    heal(source, quantity2);
                    break;
                case 2: // Damage and coin for status
                    if (CRandom.RandomInt() < 0)
                        inflictStatus(target, quantity2);
                    damage(type, quantity1, target);
                    break;
                case 3: // Add condition by coin
                    if (CRandom.RandomInt() < 0)
                        addCondition(source, quantity1, quantity2);
                    break;
                case 4: // Damage empowered by excess of energy
                    quantity1 += ExcessEnergy(mov, source) > quantity2? quantity2 : ExcessEnergy(mov, source);
                    damage(type, quantity1, target);
                    break;
            }
        }

        public static void power_selector(Player source_controller, battler source, int selector, int quantity1, int quantity2)
        {
            switch (selector)
            {
                case 0: // Rain dance
                    SwitchEnergySameType(source_controller, quantity1);
                    // Can be executed any number of times
                    break;
            }
        }

        public static void damage(int type, int quantity, battler target)
        {
            if (target.conditions.ContainsKey(Legacies.fog))
            {
                Console.WriteLine(target.ToString() + " is protected from damage.");
                return;
            }
            int output = quantity;
            if (type == target.weak_elem) output *= target.weak_mod;
            else if (type == target.res_elem) output -= target.res_mod;

            target.damage += output;

            Console.WriteLine(target.ToString() + " received " + output + " points of damage.");
        }

        public static void discardEnergy(Player source_controller, battler source, int type, int quantity)
        {
            bool end;
            int digit;
            for ( int i = 0; i < quantity; i++)
            {
                Console.WriteLine("Select to discard " + utilities.numToType(type) + " energy card/s. " + (i + 1) + "/" + quantity);
                end = false;
                while (!end)
                {
                    Console.WriteLine(source.showEnergy());
                    digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;
                    if (source.energies[digit].elem == type)
                    {
                        source_controller.discardEnergy(source, digit);
                        end = true;
                    }

                }
            }
            Console.WriteLine("Energy succcessfully discarded");
        }

        public static void heal(battler source, int quantity)
        {
            if (quantity == 0)
            {
                source.damage = 0;
                Console.WriteLine("Damage eliminated");
            }
            else
            {
                source.damage -= quantity;
                Console.WriteLine("Removed "+quantity+" damage");
            }
        }

        public static void inflictStatus(battler target, int type)
        {
            target.status = type;
            Console.WriteLine(target.ToString() + " is now " + utilities.numToStatus(type));
        }

        public static void addCondition(battler source, int condition, int duration)
        {
            source.conditions.Add(condition, duration);
            Console.WriteLine("Condition activated");
        }

        public static void SwitchEnergySameType(Player source, int elem)
        {
            Console.WriteLine("Select a Pokemon with an energy card of the selected type");
            source.DisplayTypedEnergies(elem);

            int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50;
            battler target = null;

            if (digit == -1)
                target = source.front;
            else
                target = source.benched[digit];

            if (target.energies.Count == 0)
            {
                Console.WriteLine("That pokemon has not enough energy cards");
                return;
            }

            int counter = 0;
            while (!target.energies[counter].name.Equals("Water Energy"))
                counter++;

            energy selected = target.energies[counter];
            target.energies.RemoveAt(counter);

            Console.WriteLine("Select a Pokemon to attach the energy");
            source.DisplayTypedEnergies(elem);

            digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50;

            if (digit == -1 && source.front.element == elem)
                target = source.front;
            else if (source.benched[digit].element == elem)
                target = source.benched[digit];
            else
            {
                Console.WriteLine("This pokemon is not " + utilities.numToType(elem) + " type.");
                return;
            }

            target.attachEnergy(selected);

            Console.WriteLine("Energy attached to " + selected.ToString());

        }

        public static int ExcessEnergy(movement mov, battler source)
        {
            int excess = source.energyTotal[mov.type] - mov.cost[mov.type];

            if (excess <= 0) return 0;

            int colorless = 0;

            for (int i = 0; i < mov.cost.Length; i++)
                if (i != mov.type)
                    colorless += source.energyTotal[i];

            if (colorless >= mov.cost[0]) return excess * 10;

            return (excess + colorless - mov.cost[0])*10;
        }
    }
}
