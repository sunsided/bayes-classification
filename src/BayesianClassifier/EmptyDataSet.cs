﻿using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Class EmptyDataSet. This class cannot be inherited.
    /// </summary>
    internal sealed class EmptyDataSet : IDataSet
    {
        /// <summary>
        /// Gets or sets the occurrence threshold.
        /// <para>
        /// Any token occurrence lower than the threshold will be assumed to be zero,
        /// resulting in the assumption that the given token was not seen during training.
        /// </para>
        /// </summary>
        /// <value>The occurrence threshold.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Value must be greater than or equal to zero.</exception>
        public int OccurrenceThreshold { get; set; }

        /// <summary>
        /// Gets or sets the percentage threshold.
        /// <para>
        /// Any per-class token percentage lower than the threshold will be assumed to be zero,
        /// resulting in the assumption that the given token was not seen during training.
        /// </para>
        /// </summary>
        /// <value>The occurrence threshold.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Value must be greater than or equal to zero.</exception>
        /// <exception cref="NotFiniteNumberException">value;Value must be finite.</exception>
        public double PercentageThreshold { get; set; }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<TokenCount> GetEnumerator()
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
        public IClass Class { get; private set; }

        /// <summary>
        /// Gets the <see cref="TokenInformation{IToken}"/> with the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <returns>TokenInformation&lt;IToken&gt;.</returns>
        public TokenInformation<IToken> this[IToken token, double alpha = 0D]
        {
            get { return new TokenInformation<IToken>(token, 0L, 0D); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyDataSet"/> class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <exception cref="System.ArgumentNullException">class</exception>
        public EmptyDataSet([NotNull] IClass @class)
        {
            if (ReferenceEquals(null, @class)) throw new ArgumentNullException("class");
            Class = @class;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Int64.</returns>
        /// <seealso cref="GetPercentage" />
        public long GetCount(IToken token)
        {
            return 0L;
        }

        /// <summary>
        /// Gets the percentage.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="alpha">The alpha.</param>
        /// <returns>System.Double.</returns>
        /// <seealso cref="GetCount" />
        public double GetPercentage(IToken token, double alpha = 0)
        {
            return 0D;
        }

        /// <summary>
        /// Gets the approximated percentage of the given
        /// <see cref="IToken" /> in this data set
        /// by determining its occurrence count over the whole population.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        /// <seealso cref="GetCount" />
        public TokenStats GetStats([NotNull] IToken token, double alpha)
        {
            return new TokenStats();
        }

        /// <summary>
        /// Adds the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        /// <exception cref="System.InvalidOperationException">Adding data to the empty data set is not allowed.</exception>
        public void AddToken(IToken token, params IToken[] additionalTokens)
        {
            throw new InvalidOperationException("Adding data to the empty data set is not allowed.");
        }

        /// <summary>
        /// Adds the token.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <exception cref="System.InvalidOperationException">Adding data to the empty data set is not allowed.</exception>
        public void AddToken(IEnumerable<IToken> tokens)
        {
            throw new InvalidOperationException("Adding data to the empty data set is not allowed.");
        }

        /// <summary>
        /// Removes the token once.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        public void RemoveTokenOnce(IToken token, params IToken[] additionalTokens)
        {
        }

        /// <summary>
        /// Removes the token once.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        public void RemoveTokenOnce(IEnumerable<IToken> tokens)
        {
        }

        /// <summary>
        /// Purges the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        public void PurgeToken(IToken token, params IToken[] additionalTokens)
        {
        }

        /// <summary>
        /// Purges the token.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        public void PurgeToken(IEnumerable<IToken> tokens)
        {
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
        }

        /// <summary>
        /// Calculates the minimal smoothed value.
        /// </summary>
        /// <param name="tokenCount">The count.</param>
        /// <param name="totalCount">The total count.</param>
        /// <param name="alpha">The alpha.</param>
        /// <returns>System.Double.</returns>
        public double LapaceSmoothing(long tokenCount, long totalCount, double? alpha = 0)
        {
            return 0;
        }
    }
}
