using System;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Struct TokenCount
    /// </summary>
    /// <typeparam name="TToken">The type of the t token.</typeparam>
    public struct TokenCount<TToken>
        where TToken: IToken
    {
        /// <summary>
        /// The token
        /// </summary>
        [NotNull]
        public readonly TToken Token;
        
        /// <summary>
        /// The number of occurrences
        /// </summary>
        public readonly long Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCount{TToken}" /> struct.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="count">The count.</param>
        /// <exception cref="System.ArgumentNullException">token</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">count;Count must be positive or zero.</exception>
        public TokenCount(TToken token, long count)
        {
            if (ReferenceEquals(token, null)) throw new ArgumentNullException("token");
            if (count < 0) throw new ArgumentOutOfRangeException("count", count, "Count must be positive or zero.");

            Token = token;
            Count = count;
        }
    }
}
