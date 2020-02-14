using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleSearch.Analyzer.Functions.Application.Extensions
{
    public static class Filters
    {
        public static bool IsSeparator(this char ch)
        {
            return char.IsSeparator(ch) || char.IsWhiteSpace(ch);
        }

        public static IEnumerable<char> ToCharStream(this StreamReader sr)
        {
            while (!sr.EndOfStream)
            {
                yield return (char)sr.Read();
            }
        }

        public static IEnumerable<char> ToLowerCharacters(this IEnumerable<char> charStream)
        {
            return charStream.Select(char.ToLowerInvariant);
        }

        public static IEnumerable<char> FilterSpecialCharacters(this IEnumerable<char> charStream)
        {
            return charStream.Where(ch => char.IsLetterOrDigit(ch) || ch.IsSeparator());
        }

        public static IEnumerable<string> SplitWords(this IEnumerable<char> charStream)
        {
            var buffer = new StringBuilder();
            foreach (var ch in charStream)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    buffer.Append(ch);
                }
                else
                {
                    if (buffer.Length <= 0) continue;
                    yield return buffer.ToString();
                    buffer.Clear();
                }
            }

            if (buffer.Length <= 0) yield break;
            yield return buffer.ToString();
        }
    }
}