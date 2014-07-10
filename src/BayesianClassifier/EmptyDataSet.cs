using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Class EmptyDataSet. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="TClass">The type of the t class.</typeparam>
    /// <typeparam name="TToken">The type of the t token.</typeparam>
    internal sealed class EmptyDataSet<TClass, TToken> : IDataSet<TClass, TToken> 
        where TClass: IClass
        where TToken: IToken
    {
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<TokenCount<TToken>> GetEnumerator()
        {
            yield break;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the token count.
        /// </summary>
        /// <value>The token count.</value>
        public long TokenCount { get { return 0; } }

        /// <summary>
        /// Gets the size of the set.
        /// </summary>
        /// <value>The size of the set.</value>
        public long SetSize { get { return 0; } }

        /// <summary>
        /// Gets the class.
        /// </summary>
        /// <value>The class.</value>
        public TClass Class { get; private set; }

        /// <summary>
        /// Gets the <see cref="TokenInformation{TToken}"/> with the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>TokenInformation&lt;TToken&gt;.</returns>
        public TokenInformation<TToken> this[TToken token]
        {
            get { return new TokenInformation<TToken>(token, 0L, 0D); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyDataSet{TClass, TToken}"/> class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <exception cref="System.ArgumentNullException">class</exception>
        public EmptyDataSet([NotNull] TClass @class)
        {
            if (ReferenceEquals(null, @class)) throw new ArgumentNullException("class");
            Class = @class;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Int64.</returns>
        public long GetCount(TToken token)
        {
            return 0L;
        }

        /// <summary>
        /// Gets the percentage.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Double.</returns>
        public double GetPercentage(TToken token)
        {
            return 0D;
        }

        /// <summary>
        /// Adds the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        /// <exception cref="System.InvalidOperationException">Adding data to the empty data set is not allowed.</exception>
        public void AddToken(TToken token, params TToken[] additionalTokens)
        {
            throw new InvalidOperationException("Adding data to the empty data set is not allowed.");
        }

        /// <summary>
        /// Adds the token.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <exception cref="System.InvalidOperationException">Adding data to the empty data set is not allowed.</exception>
        public void AddToken(IEnumerable<TToken> tokens)
        {
            throw new InvalidOperationException("Adding data to the empty data set is not allowed.");
        }

        /// <summary>
        /// Removes the token once.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        public void RemoveTokenOnce(TToken token, params TToken[] additionalTokens)
        {
        }

        /// <summary>
        /// Removes the token once.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        public void RemoveTokenOnce(IEnumerable<TToken> tokens)
        {
        }

        /// <summary>
        /// Purges the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        public void PurgeToken(TToken token, params TToken[] additionalTokens)
        {
        }

        /// <summary>
        /// Purges the token.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        public void PurgeToken(IEnumerable<TToken> tokens)
        {
        }
    }
}
