using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.Utils
{
    public class Utility
    {
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "";

            // Determine the correct separator for the current OS
            char correctSeparator = Path.DirectorySeparatorChar;
            char incorrectSeparator = (correctSeparator == '/') ? '\\' : '/';

            // Replace incorrect separators with the correct ones
            return path.Replace(incorrectSeparator, correctSeparator);
        }
    }
}