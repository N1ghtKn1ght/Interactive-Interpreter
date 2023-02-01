using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InteractiveInterpreter
{
    public static partial class StringExtense
    {
        internal static bool CheckValid(this string str, Regex pattern = null) => !string.IsNullOrEmpty(str) && (pattern?.IsMatch(str) ?? true);
    }
}
