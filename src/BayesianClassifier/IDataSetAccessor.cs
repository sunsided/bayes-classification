using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Interface IDataSetAccessor
    /// </summary>
    public interface IDataSetAccessor : IEnumerable<TokenCount> 
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
        int OccurrenceThreshold { get; }

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
        double PercentageThreshold { get; }

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
        IClass Class { get; }

        /// <summary>
        /// Gets the <see cref="TokenInformation{IToken}" /> with the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <returns>TokenInformation&lt;IToken&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        TokenInformation<IToken> this[IToken token, double alpha] { get; }

        /// <summary>
        /// Gets the number of occurrences of the given token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Int64.</returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        /// <seealso cref="GetPercentage" />
        /// <seealso cref="GetStats" />
        long GetCount([NotNull] IToken token);

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
        /// <seealso cref="GetStats" />
        double GetPercentage([NotNull] IToken token, double alpha);

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
        TokenStats GetStats([NotNull] IToken token, double alpha);

        /// <summary>
        /// Calculates the minimal smoothed value.
        /// </summary>
        /// <param name="tokenCount">The count.</param>
        /// <param name="totalCount">The total count.</param>
        /// <param name="alpha">The alpha.</param>
        /// <returns>System.Double.</returns>
        double LapaceSmoothing(long tokenCount, long totalCount, double? alpha = 0);
    }
}