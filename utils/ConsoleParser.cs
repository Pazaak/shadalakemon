using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.utils
{
    class ConsoleParser
    {
        public static int ReadFull(int threshold)
        {
            int digit;
            string line = Console.ReadLine();
            if (Int32.TryParse(line, out digit)) // A number
                if (digit <= threshold)
                    return digit;
                else if (digit < 0)
                    return -1;
                else
                {
                    Console.WriteLine("There are not so many cards in hand.");
                    return ReadFull(threshold);
                }

            char parsed;
            if (!char.TryParse(line, out parsed)) // Not a char
            {
                Console.WriteLine("Not valid input. Try again.");
                return ReadFull(threshold);
            }

            switch (parsed) // A char
            {
                case 'e': return -1;
                case 'a': return -2;
                case 'r': return -3;
                case 'p': return -4;
                default:
                    Console.WriteLine("Not valid input. Try again.");
                    return ReadFull(threshold);
            }
        }

        public static int ReadNumber(int threshold)
        {
            int digit;
            string line = Console.ReadLine();
            if (Int32.TryParse(line, out digit)) // A number
                if (digit <= threshold)
                    return digit;
                else if (digit < 0)
                    return -1;
                else
                {
                    Console.WriteLine("There are not so many cards in hand.");
                    return ReadNumber(threshold);
                }
            else
            {
                Console.WriteLine("Not valid input. Try again.");
                return ReadNumber(threshold);
            }
        }

        public static int ReadOrExit(int threshold)
        {
            int digit;
            string line = Console.ReadLine();
            if (Int32.TryParse(line, out digit)) // A number
                if (digit <= threshold)
                    return digit;
                else if (digit < 0)
                    return -1;
                else
                {
                    Console.WriteLine("There are not so many cards in hand.");
                    return ReadOrExit(threshold);
                }

            char parsed;
            if (!char.TryParse(line, out parsed)) // Not a char
            {
                Console.WriteLine("Not valid input. Try again.");
                return ReadOrExit(threshold);
            }

            if (parsed == 'e')
                return -1;

            Console.WriteLine("Not valid input. Try again.");
            return ReadOrExit(threshold);
        }
    }
}
