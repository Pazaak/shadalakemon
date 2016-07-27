using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.utils
{
    class Logger
    {
        public static void Clear()
        {
            System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + @"/match.log", string.Empty);
        }

        public static void Report(string message)
        {
            System.IO.File.AppendAllText(System.IO.Directory.GetCurrentDirectory() + @"/match.log", message+Environment.NewLine);
        }

        public static string[] FindCombatString(string player1, string player2)
        {
            string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + @"/match.log");

            string mainPhase = "-> " + player1 + "'s main phase";

            int i, j;
            for (i = lines.Length-1; i >= 0; i--)
                if (string.Compare(lines[i], mainPhase) == 0)
                    break; // Finds the last turn of the opponent 

            string attackPhase = "-> " + player1 + "'s attack phase";

            for (j = i; j < lines.Length; j++)
                if (string.Compare(lines[j], attackPhase) == 0)
                    break; // Finds the attack phase of the last turn, if any

            if (j+1 == lines.Length)
                return null; // There was no attack last turn

            mainPhase = "-> " + player2 + "'s main phase";

            for (i = j; i < lines.Length; i++)
                if (string.Compare(lines[i], mainPhase) == 0)
                    break; // Finds the end of the attack phase

            string[] output = new string[i - j - 1];
            for (int k = 0; k < output.Length; k++)
                output[k] = lines[j + k + 1];

            return output;
        }

    }
}
