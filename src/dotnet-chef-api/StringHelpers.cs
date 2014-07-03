using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace mattberther.chef
{
    static class StringHelpers
    {
        public static string ToBase64EncodedSha1String(this string input)
        {
            return Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

        public static IEnumerable<string> Split(this string input, int length)
        {
            for (int i = 0; i < input.Length; i += length)
                yield return input.Substring(i, Math.Min(length, input.Length - i));
        }
    }
}