using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ScriptConsoleLibrary
{
    public static partial class Extensions
    {

        public static int CountOccurences(this string haystack, string needle)
        {
            return (haystack.Length - haystack.Replace(needle, "").Length) / needle.Length;
        }

        public static int CountCharsUpToPosition(this string source, int position)
        {
            string sub = source.Substring(0, Math.Min(position, source.Length));
            int numTabs = CountOccurences(sub, "\t");

            return sub.Length + numTabs * 3;
        }
    }
}
