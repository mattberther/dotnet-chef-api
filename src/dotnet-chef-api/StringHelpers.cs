namespace mattberther.chef
{
    using System;
    using System.Collections.Generic;

    internal static class StringHelpers
    {

        public static IEnumerable<string> Split(this string input, int length)
        {
            for (int i = 0; i < input.Length; i += length)
                yield return input.Substring(i, Math.Min(length, input.Length - i));
        }
    }
}