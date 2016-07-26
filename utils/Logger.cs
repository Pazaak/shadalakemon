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
    }
}
