using System.Linq;

namespace Controlpanel.Utilities
{
    public abstract class Obfuscator
    {
        public static string encode(string input)
        {
            return input.Aggregate("", (current, t) => current + (char)(t + 1));
        }
        
        public static string decode(string input)
        {
            return input.Aggregate("", (current, t) => current + (char)(t - 1));
        }
    }
}