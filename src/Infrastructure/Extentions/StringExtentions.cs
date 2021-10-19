using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Infrastructure.Extentions
{
    public static class StringExtentions
    {
        public static byte[] ToByteArray(this string str)
            => System.Text.Encoding.ASCII.GetBytes(str);

        public static bool IsAllNullOrEmpty(this IEnumerable<string> strings)
            => strings.Select(s => string.IsNullOrEmpty(s))?.All(b => b == true) ?? true;
    }
}
