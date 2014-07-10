using System.Collections.Generic;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Interface IDataSetAccessor
    /// </summary>
    /// <typeparam name="TClass">The type of the class.</typeparam>
    /// <typeparam name="TToken">The type of the token.</typeparam>
    public interface IDataSetAccessor<out TClass, TToken> : IEnumerable<TokenCount<TToken>> 
        where TClass : IClass 
        where TToken : IToken
    {
        /// <summary>
        /// Gets the number of distinct tokens, 
        /// i.e. every token counted at exactly once.
        /// </summary>
        /// <value>The token count.</value>
        /// <seealso cref="SetSize"/>
        long TokenCount { get; }

        /// <summary>
        /// Gets the size of the set.
        /// </summary>
        /// <value>The size of the set.</value>
        /// <seealso cref="TokenCount"/>
        long SetSize { get; }

        /// <summary>
        /// Gets the class.
        /// </summary>
        /// <value>The class.</value>
        [NotNull]
        TClass Class { get; }

        /// <summary>
        /// Gets the <see cref="TokenInformation{TToken}" /> with the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>TokenInformation&lt;TToken&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        TokenInformation<TToken> this[TToken token] { get; }

        /// <summary>
        /// Gets the number of occurrences of the given token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Int64.</returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        /// <seealso cref="GetPercentage"/>
        long GetCount([NotNull] TToken token);

        /// <summary>
        /// Gets the approximated percentage of the given <see cref="TToken"/> in this data set
        /// by determining its occurrence count over the whole population.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        /// <seealso cref="GetCount"/>
        double GetPercentage([NotNull] TToken token);
    }
}