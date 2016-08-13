using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.utils
{
    class ReadDeck
    {
        public static List<int> ReadIndexes(string path)
        {
            string[] buffer = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + @"/decks/" + path);
            List<int> output = new List<int>();

            foreach (string str in buffer)
            {
                string[] tokens = str.Split(' ');
                int times, index;
                if (Int32.TryParse(tokens[0], out times) && Int32.TryParse(tokens[1], out index))
                    while ( times-- > 0 ) output.Add(index);
            }

            return output;
        }
    }
}
