using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    internal static class LinqExtensions
    {
        /// <summary>
        /// Converts an enumerable to a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>ICollection&lt;T&gt;.</returns>
        [NotNull]
        public static ICollection<T> ToCollection<T>([NotNull] this IEnumerable<T> enumerable)
        {
            var collection = new Collection<T>();
            foreach (var t in enumerable)
            {
                collection.Add(t);
            }
            return collection;
        }
    }
}
