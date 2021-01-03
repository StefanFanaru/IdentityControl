using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace IdentityControl.API.Extensions
{
    public static class StringExtensions
    {
        private const int MaximumIdenticalConsecutiveChars = 2;
        private const string LowercaseCharacters = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NumericCharacters = "0123456789";
        private const string SpecialCharacters = "!@$?*£$%!#";
        private const string SpaceCharacter = " ";
        private const int PasswordLengthMin = 8;
        private const int PasswordLengthMax = 256;

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case "":
                    throw new ArgumentException("input cannot be empty", nameof(input));
                case null:
                    throw new ArgumentNullException(nameof(input));
                default:
                    return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static string GenerateEmailToken()
        {
            return GeneratePassword(true, true, true, false, false, 256);
        }

        public static string GeneratePassword(
            bool includeLowercase,
            bool includeUppercase,
            bool includeNumeric,
            bool includeSpecial,
            bool includeSpaces,
            int lengthOfPassword)
        {
            if (lengthOfPassword < 8 || lengthOfPassword > 256)
                return "Password length must be between 8 and 128.";
            var str = "";
            if (includeLowercase)
                str += "abcdefghijklmnopqrstuvwxyz";
            if (includeUppercase)
                str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (includeNumeric)
                str += "0123456789";
            if (includeSpecial)
                str += "!@$?*£$%!#";
            if (includeSpaces)
                str += " ";
            var chArray = new char[lengthOfPassword];
            var length = str.Length;
            var random = new Random();
            for (var index = 0; index < lengthOfPassword; ++index)
            {
                chArray[index] = str[random.Next(length - 1)];
                if ((index <= 2 || (int) chArray[index] != (int) chArray[index - 1]
                    ? 0
                    : (int) chArray[index - 1] == (int) chArray[index - 2]
                        ? 1
                        : 0) != 0)
                    --index;
            }

            return string.Join(null, chArray);
        }

        /// <summary>
        ///     Returns first 8 chars of a GUID in uppercase
        /// </summary>
        /// <param name="id">Any id you find hard to read</param>
        public static string GetHumanReadableId(this string id)
        {
            return id.Substring(0, 8).ToUpper();
        }

        public static string FormatType(this string type)
        {
            return Regex.Replace(type, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
        }
    }
}