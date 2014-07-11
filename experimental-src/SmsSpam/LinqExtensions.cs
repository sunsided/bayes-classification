using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace SmsSpam
{
    /// <summary>
    /// Class LinqExtensions.
    /// </summary>
    static class LinqExtensions
    {
        /// <summary>
        /// Glues the specified <see cref="T:System.Char"/> values into a <see cref="T:System.String"/>.
        /// </summary>
        /// <param name="chars">The characters.</param>
        /// <returns>System.String.</returns>
        [NotNull]
        public static string Glue([NotNull] this IEnumerable<char> chars)
        {
            var builder = new StringBuilder();
            foreach (var c in chars)
            {
                builder.Append(c);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Determines if the specified <see cref="T:IEnumerable&lt;System.Char&gt;" /> does not contain a given <see cref="T:System.Char" />
        /// </summary>
        /// <param name="chars">The characters.</param>
        /// <param name="c">The character under test.</param>
        /// <returns><c>true</c> if the enumerable does not contain the character, <c>false</c> otherwise.</returns>
        public static bool DoesNotContain([NotNull] this IEnumerable<char> chars, char c)
        {
            return !chars.Contains(c);
        }
    }
}
