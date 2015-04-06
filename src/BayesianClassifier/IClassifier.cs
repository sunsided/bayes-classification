using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Interface IClassifier
    /// </summary>
    public interface IClassifier
    {
        /// <summary>
        /// Gets or sets the probability correction.
        /// </summary>
        /// <value>The probability correction.</value>
        [CanBeNull]
        IProbabilityCorrection ProbabilityCorrection { get; set; }

        /// <summary>
        /// Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.
        /// <para>
        /// Laplace smoothing is required in the context or rare (i.e. untrained) tokens or tokens
        /// that do not appear in some classes. With smoothing disabled, these tokens result
        /// in a zero probability for the whole class. To counter that, a positive ("alpha")
        /// value for smoothing can be set.
        /// </para>
        /// </summary>
        double SmoothingAlpha { get; set; }
        
        /// <summary>
        /// Gets or sets the messages norm length.
        /// </summary>
        /// <value>The norm length of a message.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Value must be greater than or equal to zero.</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        double NormLength { get; set; }

        /// <summary>
        /// Gets or sets the occurrence threshold.
        /// <para>
        /// Any token occurrence lower than the threshold will be assumed to be zero,
        /// resulting in the assumption that the given token was not seen during training.
        /// </para>
        /// </summary>
        /// <value>The occurrence threshold.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Value must be greater than or equal to zero.</exception>
        int OccurrenceThreshold { get; set; }

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
        double PercentageThreshold { get; set; }

        /// <summary>
        /// Calculates the probability of having the <see cref="IClass"/> 
        /// given the occurrence of the <see cref="IToken"/>.
        /// </summary>
        /// <param name="classUnderTest">The class under test.</param>
        /// <param name="token">The token.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied, setting to <see langword="null"/> defaults to the values set in <see cref="SmoothingAlpha"/>.</param>
        /// <returns>System.Double.</returns>
        double CalculateProbability([NotNull] IClass classUnderTest, [NotNull] IToken token, double? alpha = null);

        /// <summary>
        /// Calculates the probability of having the 
        /// <see cref="IClass" />
        /// given the occurrence of the 
        /// <see cref="IToken" />.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied, setting to <see langword="null"/> defaults to the values set in <see cref="SmoothingAlpha"/>.</param>
        /// <returns>System.Double.</returns>
        [NotNull]
        IEnumerable<ConditionalProbability> CalculateProbabilities([NotNull] IToken token, double? alpha = null);

        /// <summary>
        /// Calculates the probability of having the
        /// <see cref="IClass" />
        /// given the occurrence of the
        /// <see cref="IToken" />.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied, setting to <see langword="null"/> defaults to the values set in <see cref="SmoothingAlpha"/>.</param>
        /// <returns>System.Double.</returns>
        [NotNull]
        IEnumerable<CombinedConditionalProbability> CalculateProbabilities([NotNull] ICollection<IToken> tokens, double? alpha = null);
    }
}