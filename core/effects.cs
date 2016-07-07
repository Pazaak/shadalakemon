using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    class effects
    {
        public static int move_selector(Player source_controller, Player target_controller, battler source, battler target, int type, int selector, int quantity1, int quantity2)
        {
            switch (selector)
            {
                case 0: // Simple damage
                    return(damage(type, quantity1, target));
                case 1: // Discard and heal
                    discardEnergy(source_controller, source, type, quantity1);
                    heal(source, quantity2);
                    return (0);
                default:
                    return (-1);
            }
        }

        public static int damage(int type, int quantity, battler target)
        {
            int output = quantity;
            if (type == target.weak_elem) output *= target.weak_mod;
            else if (type == target.res_elem) output -= target.res_mod;

            target.damage += output;

            return output;
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
    }
}
