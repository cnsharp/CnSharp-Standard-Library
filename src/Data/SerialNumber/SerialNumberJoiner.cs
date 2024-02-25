using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CnSharp.Data.SerialNumber
{
    public class SerialNumberJoiner
    {
        private const string SequencePattern = @"0\d+d";
        private const string DatePattern = "^[y|M|d|H|m|s|fff]+$";
        private const string Separator = "%";

        public static string GetNumber(string pattern, long sequence, Dictionary<string,object> context = null)
        {
            var number = pattern;
            var placeholders = SplitPlaceholder(pattern);
            foreach(var placeholder in placeholders)
            {
                var oldString = $"{Separator}{placeholder}{Separator}";
                var sequencePattern = GetSequencePattern(placeholder);
                if(sequencePattern != null)
                {
                    number = number.Replace(oldString, FormatSequence(sequence, sequencePattern));
                }
                else if(IsDateChars(placeholder))
                {
                    number = number.Replace(oldString, DateTimeOffset.Now.ToString(placeholder));
                }
                else if(context != null && context.ContainsKey(placeholder))
                {
                    number = number.Replace(oldString, context[placeholder].ToString());
                }
            }

            return number;
        }

        private static List<string> SplitPlaceholder(string pattern)
        {
            var placeholders = new List<string>();
            var str = pattern;
            var i = str.IndexOf(Separator, StringComparison.Ordinal);
            while (i >= 0)
            {
                str = str.Substring(i + 1);
                var j = str.IndexOf(Separator, StringComparison.Ordinal);
                if (j <= 0)
                {
                    throw new IndexOutOfRangeException("placeholder not closed.");
                }
                placeholders.Add(str.Substring(0, j));
                str = str.Substring(j + 1);
                i = str.IndexOf(Separator, StringComparison.Ordinal);
            }

            return placeholders;
        }

        private static string GetSequencePattern(string placeholder)
        {
            var match = Regex.Match(placeholder, SequencePattern);
            return match.Success ? match.Value : null;
        }

        private static string FormatSequence(long sequence, string pattern)
        {
            string num = pattern.TrimStart('0').TrimEnd('d');
            var len = int.Parse(num);
            return sequence.ToString().PadLeft(len, '0');
        }

        private static bool IsDateChars(string placeholder)
        {
           return Regex.IsMatch(placeholder, DatePattern);
        }
    }
}
